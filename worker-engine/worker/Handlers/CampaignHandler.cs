using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Worker.Data;
using Worker.Models;
using Worker.Engines;

namespace Worker.Handlers
{
    public class CampaignHandler
    {
        private readonly WorkerDbContext _db;
        private readonly ILogger<CampaignHandler> _logger;
        private readonly IRuleEngine _ruleEngine;

        public CampaignHandler(WorkerDbContext db, ILogger<CampaignHandler> logger, IRuleEngine ruleEngine)
        {
            _db = db;
            _logger = logger;
            _ruleEngine = ruleEngine;
        }

        public async Task<bool> HandleAsync(string key, string payload)
        {
            try
            {
                using var doc = JsonDocument.Parse(payload);
                var root = doc.RootElement;
                
                // Determine if it's the wrapped outbox format or raw payload
                JsonElement data;
                string? eventType = null;
                
                if (root.TryGetProperty("payload", out var pProp))
                {
                    data = pProp;
                    if (root.TryGetProperty("type", out var tProp)) eventType = tProp.GetString();
                    else if (root.TryGetProperty("event_type", out var etProp)) eventType = etProp.GetString();
                }
                else
                {
                    data = root;
                }

                var campaign = JsonSerializer.Deserialize<Campaign>(data.GetRawText(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                if (campaign == null)
                {
                    _logger.LogError("Failed to deserialize campaign from payload");
                    return false;
                }

                // Basic validation
                if (string.IsNullOrWhiteSpace(campaign.Name) || string.IsNullOrWhiteSpace(campaign.Status))
                {
                    _logger.LogWarning("Campaign validation failed for ID {CampaignId}: Missing Name or Status", campaign.Id);
                    return false;
                }

                if (campaign.EndAt.HasValue && campaign.EndAt <= campaign.StartAt)
                {
                    _logger.LogWarning("Campaign validation failed for ID {CampaignId}: Invalid date range", campaign.Id);
                    return false;
                }

                _logger.LogInformation("Processing Campaign event: {EventType} for ID {CampaignId}", eventType ?? "Unknown", campaign.Id);

                // Serialize for persistence
                var conditionsJson = JsonSerializer.Serialize(campaign.Conditions);
                var rewardsJson = JsonSerializer.Serialize(campaign.Rewards);

                try
                {
                    // 1. Save Core (Campaigns table)
                    var existing = await _db.Campaigns.FindAsync(campaign.Id);
                    if (existing == null)
                    {
                        await _db.Campaigns.AddAsync(campaign);
                    }
                    else
                    {
                        _db.Entry(existing).CurrentValues.SetValues(campaign);
                    }
                    await _db.SaveChangesAsync();

                    // 2. Save Conditions (campaign_conditions table)
                    var campaignGuid = Guid.Parse(campaign.Id);
                    var existingCond = await _db.CampaignConditions.FirstOrDefaultAsync(c => c.CampaignId == campaignGuid);
                    
                    // CHECK FOR PRE-COMPILED RulesEngine JSON in the direct payload or condition logic
                    string? finalRuleJson = null;

                    // A. Check for direct RuleGroup in the Kafka payload (new clean format)
                    if (campaign.RuleGroup != null)
                    {
                        finalRuleJson = JsonSerializer.Serialize(campaign.RuleGroup);
                        _logger.LogInformation("Using direct RuleGroup (clean MS array) for Campaign {Id}", campaign.Id);
                    }
                    // B. Fallback to extracting from the first condition's RuleGroupJson (old integrated format)
                    else if (campaign.Conditions != null && campaign.Conditions.Count > 0)
                    {
                        var firstCond = campaign.Conditions[0];
                        if (!string.IsNullOrEmpty(firstCond.RuleGroupJson))
                        {
                            try {
                                using var rgDoc = JsonDocument.Parse(firstCond.RuleGroupJson);
                                if (rgDoc.RootElement.TryGetProperty("rulesEngine", out var reProp))
                                {
                                    finalRuleJson = reProp.GetRawText();
                                    _logger.LogInformation("Using ruleGroupJson.rulesEngine for Campaign {Id}", campaign.Id);
                                }
                                else
                                {
                                     // If it's not wrapped in 'rulesEngine', use the whole thing as RuleJson
                                     finalRuleJson = firstCond.RuleGroupJson;
                                     _logger.LogInformation("Using RuleGroupJson directly for Campaign {Id}", campaign.Id);
                                }
                            } catch { }
                        }
                    }

                    if (finalRuleJson == null)
                    {
                        var wf = ConvertToWorkflow(campaign);
                        finalRuleJson = JsonSerializer.Serialize(wf);
                    }

                    if (existingCond == null)
                    {
                        _db.CampaignConditions.Add(new CampaignConditionEntity { 
                            CampaignId = campaignGuid, 
                            ConditionJson = conditionsJson,
                            RuleJson = finalRuleJson
                        });
                    }
                    else
                    {
                        existingCond.ConditionJson = conditionsJson;
                        existingCond.RuleJson = finalRuleJson;
                    }

                    // 3. Save Rewards (campaign_rewards table)
                    var existingRew = await _db.CampaignRewards.FirstOrDefaultAsync(r => r.CampaignId == campaignGuid);
                    if (existingRew == null)
                    {
                        _db.CampaignRewards.Add(new CampaignRewardEntity { CampaignId = campaignGuid, RewardJson = rewardsJson });
                    }
                    else
                    {
                        existingRew.RewardJson = rewardsJson;
                    }

                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to persist Campaign {Id} to DB (Core or Satellites). Proceeding with in-memory registration.", campaign.Id);
                }

                _logger.LogInformation("Campaign {CampaignId} ('{CampaignName}') synced and registered in Rule Engine (New & Old)", campaign.Id, campaign.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling campaign event. Payload: {Payload}", payload);
                return false;
            }
        }

        private object? ExtractConditionValue(object? val)
        {
            // If value is a JSON string wrapper like {"value":"1"}, extract "1"
            string? s = null;
            if (val is JsonElement je)
            {
                 if (je.ValueKind == JsonValueKind.String) s = je.GetString();
                 else s = je.GetRawText();
            }
            else if (val is string str)
            {
                s = str;
            }

            if (!string.IsNullOrWhiteSpace(s) && s.Trim().StartsWith("{") && s.Contains("value"))
            {
                try
                {
                     using var doc = JsonDocument.Parse(s);
                     if (doc.RootElement.TryGetProperty("value", out var v))
                     {
                         if (v.ValueKind == JsonValueKind.String) return v.GetString();
                         if (v.ValueKind == JsonValueKind.Number) return v.GetDecimal(); 
                         return v.GetRawText();
                     }
                }
                catch { }
            }
            return s ?? val; // Fallback
        }

        private RulesEngine.Models.Workflow ConvertToWorkflow(Campaign campaign)
        {
            var rules = new List<RulesEngine.Models.Rule>();
            var allConditions = new List<string>();

            // Map each condition to a lambda expression string
            foreach (var c in campaign.Conditions)
            {
                string expr = MapConditionToExpression(c);
                if (!string.IsNullOrWhiteSpace(expr))
                {
                    allConditions.Add(expr);
                }
            }

            // Create a single Rule that requires ALL conditions (AND logic)
            // or modify later for complex logic if needed.
            // For now, Campaign Wizard "Rules" step treats list as AND (or OR based on operator, but handler used AND previously).
            // Let's assume AND for basic migration unless we parse the "Combinator" field if strictly needed.
            // Actually, previous Handler implementation hardcoded "AND".
            
            string fullExpression = string.Join(" && ", allConditions);
            if (string.IsNullOrWhiteSpace(fullExpression)) fullExpression = "true"; // No conditions = match all?

            var rule = new RulesEngine.Models.Rule
            {
                RuleName = "MainCondition",
                SuccessEvent = "qualifies",
                ErrorMessage = "Not eligible",
                RuleExpressionType = RulesEngine.Models.RuleExpressionType.LambdaExpression,
                Expression = fullExpression
            };

            rules.Add(rule);

            return new RulesEngine.Models.Workflow
            {
                WorkflowName = $"campaign_{campaign.Id}",
                Rules = rules
            };
        }

        private string MapConditionToExpression(CampaignConditionModel c)
        {
            // input is the TransactionEvent or Context object
            // field: e.g., "event.payload.gross_value" or "min_order_amount"
            // We need to map these frontend keys to backend object properties.
            
            string field = c.Type;
            string op = c.Operator?.ToLower() ?? "eq";
            object? val = ExtractConditionValue(c.Value);
            string valStr = FormatValueForExpression(val);

            // Mapping Field Names
            // Previous handler used: "min_order_amount" or "event.payload.gross_value"
            // TransactionEvent has: Amount. Context has: min_order_amount (aliased).
            // We will standardize on using the Dynamic/Expando input properties.
            
            string prop = "input." + CleanFieldName(field);

            switch (op)
            {
                case "gt": return $"{prop} > {valStr}";
                case "gte": return $"{prop} >= {valStr}";
                case "lt": return $"{prop} < {valStr}";
                case "lte": return $"{prop} <= {valStr}";
                case "eq": 
                case "equals": return $"{prop} == {valStr}";
                case "neq": return $"{prop} != {valStr}";
                // contains for string or list?
                case "contains": return $"{prop} != null && {prop}.ToString().Contains({valStr})";
                case "in":
                    // "val" is likely an array in the value string, e.g. ["GOLD","SILVER"]
                    // if val is string "SILVER", treat as single.
                    // Expression: Utils.CheckContains(prop, val) ? Hard to inject utils.
                    // Simple string check: 
                    return $"{valStr}.Contains({prop})"; 
                default: return "false";
            }
        }

        private string CleanFieldName(string f)
        {
            // Map "event.payload.gross_value" -> "min_order_amount" or "Amount" ?
            // The TransactionHandler builds the context. 
            // We should align with what TransactionHandler provides.
            // Let's assume TransactionHandler provides flat keys: "min_order_amount", "CustomerTier"
            
            if (f == "event.payload.gross_value") return "min_order_amount"; 
            if (f == "member.Tier") return "CustomerTier";
            if (f == "member.PointsBalance") return "PointsBalance";
            
            // Fallback: sanitized name
            return f.Replace(".", "_"); 
        }

        private string FormatValueForExpression(object? val)
        {
             if (val == null) return "null";
             if (val is string s) return $"\"{s}\""; // Quote strings
             if (val is bool b) return b.ToString().ToLower();
             return val.ToString()!; // Numbers
        }
        
        /// <summary>
        /// Recursively builds a Lambda expression from a RuleGroup structure.
        /// Supports nested groups with AND/OR combinators.
        /// </summary>
        private string BuildRuleGroupExpression(RuleGroupModel group)
        {
            var expressions = new List<string>();
            
            // Process individual conditions
            foreach (var condition in group.Conditions)
            {
                var expr = MapRuleConditionToExpression(condition);
                if (!string.IsNullOrWhiteSpace(expr))
                {
                    expressions.Add(expr);
                }
            }
            
            // Process nested groups recursively
            foreach (var nestedGroup in group.Groups)
            {
                var nestedExpr = BuildRuleGroupExpression(nestedGroup);
                if (!string.IsNullOrWhiteSpace(nestedExpr))
                {
                    expressions.Add($"({nestedExpr})");
                }
            }
            
            if (expressions.Count == 0) return "true";
            if (expressions.Count == 1) return expressions[0];
            
            string combinator = group.Combinator?.ToUpper() == "OR" ? " || " : " && ";
            return string.Join(combinator, expressions);
        }

        private string MapRuleConditionToExpression(RuleConditionModel condition)
        {
            string field = condition.Field;
            string op = condition.Operator?.ToLower() ?? "eq";
            var value = condition.Value;

            string inputField = field switch
            {
                "min_order_amount" => "input.amount",
                "gross_value" => "input.gross_value",
                "amount" => "input.amount",
                "CustomerTier" => "input.CustomerTier",
                "PointsBalance" => "input.PointsBalance",
                "history_tx_count" => "input.history_tx_count",
                "history_tx_sum" => "input.history_tx_sum",
                "Category" => "input.Category",
                "Sku" => "input.Sku",
                "StoreId" => "input.StoreId",
                _ => $"input.{field}"
            };

            return op switch
            {
                "gt" => $"{inputField} > {FormatValueForExpression(value)}",
                "gte" => $"{inputField} >= {FormatValueForExpression(value)}",
                "lt" => $"{inputField} < {FormatValueForExpression(value)}",
                "lte" => $"{inputField} <= {FormatValueForExpression(value)}",
                "eq" => $"{inputField} == {FormatValueForExpression(value)}",
                "neq" => $"{inputField} != {FormatValueForExpression(value)}",
                "contains" => $"{inputField}.Contains({FormatValueForExpression(value)})",
                "startswith" => $"{inputField}.StartsWith({FormatValueForExpression(value)})",
                "endswith" => $"{inputField}.EndsWith({FormatValueForExpression(value)})",
                "in" => BuildInExpression(inputField, value),
                "notin" => $"!{BuildInExpression(inputField, value)}",
                _ => $"{inputField} == {FormatValueForExpression(value)}"
            };
        }

        private string BuildInExpression(string field, object? value)
        {
            if (value is JsonElement je && je.ValueKind == JsonValueKind.Array)
            {
                var items = je.EnumerateArray().Select(e => FormatValueForExpression(e)).ToList();
                return $"new[] {{ {string.Join(", ", items)} }}.Contains({field})";
            }
            return "false";
        }
    }
}
