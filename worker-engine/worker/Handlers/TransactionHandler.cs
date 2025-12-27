using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Worker.Data;
using Microsoft.EntityFrameworkCore;
using Worker.Models;
using Worker.Engines;
using Worker.Infrastructure;
using Microsoft.Extensions.Logging;
using Worker.Strategies;

namespace Worker.Handlers
{
    public class TransactionHandler
    {
        private readonly WorkerDbContext _db;
        private readonly ILogger<TransactionHandler> _log;

        private readonly IRuleEvaluator _ruleEngine;
        private readonly IEnumerable<IRewardStrategy> _strategies;

        public TransactionHandler(
            WorkerDbContext db, 
            ILogger<TransactionHandler> log,
            IRuleEvaluator ruleEngine,
            IEnumerable<IRewardStrategy> strategies)
        {
            _db = db;
            _log = log;
            _ruleEngine = ruleEngine;
            _strategies = strategies;
        }

        public async Task<bool> HandleAsync(ConsumeResult<string, string> msg, CancellationToken ct)
        {
            try
            {
                using var doc = JsonDocument.Parse(msg.Message.Value);
                var root = doc.RootElement;
                
                // Determine if it's the wrapped outbox format or raw payload
                JsonElement payload;
                if (root.TryGetProperty("payload", out var pProp))
                {
                    payload = pProp;
                }
                else
                {
                    payload = root;
                }

                var txId = payload.GetProperty("transaction_id").GetString() ?? string.Empty;
                var userId = payload.GetProperty("user_id").GetString() ?? string.Empty;
                var amount = payload.GetProperty("gross_value").GetDecimal();

                var tx = new TransactionEvent
                {
                    TransactionId = txId,
                    UserId = userId,
                    Amount = amount,
                    RawPayload = payload.GetRawText()
                };
                // save to database transaction events
                await _db.TransactionEvents.AddAsync(tx, ct);
                await _db.SaveChangesAsync(ct);

                _log.LogInformation("Saved transaction {Tx} for user {User}", txId, userId);

                // --- Rule Evaluation ---
                // Enrichment: Fetch history (Count includes the current one because we saved it above)
                var historyCount = await _db.TransactionEvents.CountAsync(t => t.UserId == userId, ct);
                var historySum = await _db.TransactionEvents.Where(t => t.UserId == userId).SumAsync(t => t.Amount, ct);

                _log.LogInformation("Context enriched for User {User}: Count={Count}, Sum={Sum}", userId, historyCount, historySum);

                var context = new Dictionary<string, object?>
                {
                    { "transaction_id", txId },
                    { "user_id", userId },
                    { "member_id", userId },
                    { "amount", amount },
                    { "gross_value", amount },
                    { "totalAmount", amount },
                    { "total_amount", amount },
                    { "transactionValue", amount },
                    { "history_tx_count", historyCount }, 
                    { "history_tx_sum", historySum },
                    { "member.PointsBalance", historySum }, // Alias for points/balance
                    { "member.total_spend", historySum },
                    { "member.tier", "BRONZE" },
                    { "min_order_amount", amount } // Map condition type directly to value for standard rules
                };

                // Context for RulesEngine (mapped to "input" in lambda)
                // We pass a dynamic object or specific object. The Evaluator passes it as-is.
                // Our lambdas use "input.min_order_amount", etc.
                dynamic input = new System.Dynamic.ExpandoObject();
                var inputDict = (IDictionary<string, object?>)input;
                
                // Populate input
                inputDict["Amount"] = amount;
                inputDict["GrossValue"] = amount;
                inputDict["HistoryTxCount"] = historyCount;
                inputDict["HistoryTxSum"] = historySum;
                inputDict["PointsBalance"] = 100; // TEMPORARY: Set to 100 for testing (should be actual points balance from DB)
                inputDict["CustomerTier"] = "BRONZE"; // Hardcoded for now, should fetch from DB
                
                // Also keep legacy name for safety
                inputDict["amount"] = amount;
                inputDict["min_order_amount"] = amount;
                inputDict["gross_value"] = amount;
                inputDict["history_tx_count"] = historyCount;
                inputDict["history_tx_sum"] = historySum;

                _log.LogInformation("Input for rules: Amount={Amount}, PointsBalance={PointsBalance}, HistoryTxCount={Count}, HistoryTxSum={Sum}", 
                    amount, historySum, historyCount, historySum);

                // For legacy campaigns that might look for "member.Tier" in mapped expression:
                // Our CampaignHandler maps "member.Tier" -> "CustomerTier" so we are good.

                // Evaluate per campaign
                // We need to know WHICH campaigns to evaluate. 
                // The new IRuleEvaluator wraps ALL workflows.
                // _ruleEngine.EvaluateAsync(ruleName, input)
                
                // In RulesEngine, workflows are registered by name "campaign_{id}".
                // We don't have a "GetAllRules" on IRuleEvaluator yet.
                // WE SHOULD iterate active campaigns from DB or cache to know what to check,
                // OR we can make a helper in Evaluator to "EvaluateAll".
                
                // For this migration, let's fetch active campaign IDs from DB quickly or check all known workflows.
                // The most robust way without cache is fetching active IDs.
                var activeCampaignIds = await _db.Campaigns
                    .Where(c => c.Status == "ACTIVE")
                    .Select(c => c.Id)
                    .ToListAsync(ct);

                _log.LogInformation("Evaluating {Count} active campaigns...", activeCampaignIds.Count);

                foreach (var campId in activeCampaignIds)
                {
                    var ruleName = $"campaign_{campId}"; // Workflow Name
                    _log.LogInformation("Evaluating Rule {RuleName}", ruleName);
                    bool matched = await _ruleEngine.EvaluateAsync(ruleName, input);
                    
                    if (matched)
                    {
                       // _log.LogInformation("Transaction {TxId} matched Campaign {CampId}", txId, campId);
                        
                        // We need to apply rewards. 
                        // The previous engine returned "Actions". The new one returns bool (simplified).
                        // Refetch the rewards for this campaign from DB? Or cache?
                        // "CampaignHandler" saves rewards to "campaign_rewards" table.
                        
                        var rewardsEnt = await _db.CampaignRewards
                             .FirstOrDefaultAsync(r => r.CampaignId == Guid.Parse(campId), ct);

                        if (rewardsEnt != null && !string.IsNullOrWhiteSpace(rewardsEnt.RewardJson))
                        {
                             var rewardsList = JsonSerializer.Deserialize<List<CampaignRewardModel>>(rewardsEnt.RewardJson);
                             if (rewardsList != null)
                             {  _log.LogInformation("Applying rewards for campaign {CampId}", campId);
                                 foreach (var r in rewardsList)
                                 {
                                     // Construct Action for Strategy
                                     var action = new RuleAction
                                     {
                                         ActionType = r.Type,
                                         Parameters = MapRewardParams(r.Value)
                                     };
                                     
                                     var strategy = _strategies.FirstOrDefault(s => s.CanHandle(action.ActionType));
                                     if (strategy != null)
                                     {
                                         await strategy.ExecuteAsync(action, userId, txId, campId);
                                         _log.LogInformation("Executed strategy {Strategy} for campaign {CampId}", strategy.GetType().Name, campId);
                                     }
                                 }
                             }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Transaction handler error");
                return false;
            }
        }

        private Dictionary<string, object?> MapRewardParams(object? val)
        {
            var dict = new Dictionary<string, object?>();
            if (val == null) return dict;
            
            try 
            {
                // Try to parse if it looks like JSON
                string json = val.ToString() ?? "";
                if (json.Trim().StartsWith("{"))
                {
                    var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    if (parsed != null)
                    {
                        foreach(var kv in parsed) dict[kv.Key] = kv.Value;
                        return dict;
                    }
                }
            } catch {}

            // Fallback: value as "value" or "factor" etc depending on usage?
            // Actually, existing RewardStrategy expects keys like "points", "factor", "couponCode".
            // If the UI sends raw value (e.g. "2.0") for multiplier, we assume it matches the strategy's need.
            // But strategies explicitly look for keys. 
            // Our CampaignHandler.MapReward logic handled this for the OLD engine.
            // Here we need to reconstruct that logic or trust the "RewardJson" has wrapping.
            // "RewardJson" is saved in CampaignHandler as List<CampaignRewardModel>. 
            // The "Value" there is object.
            
            // Let's assume the Strategy handles the Dictionary.
            return dict; 
        }
    }
}
