using Worker.Models; // For RuleAction
using System.Text.Json; // and other imports... for simplicity, I'll rely on existing ones being there if I don't touch them.
// But replace_file_content requires context.
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Worker.Engines;
using Worker.Infrastructure;
using Worker.Data;

namespace Worker.Strategies
{
    public class PointsRewardStrategy : IRewardStrategy
    {
        private readonly WorkerDbContext _db;
        private readonly IOutboxRepository _outbox;
        private readonly ILogger<PointsRewardStrategy> _logger;

        public PointsRewardStrategy(WorkerDbContext db, IOutboxRepository outbox, ILogger<PointsRewardStrategy> logger)
        {
            _db = db;
            _outbox = outbox;
            _logger = logger;
        }

        public bool CanHandle(string actionType)
        {
            return actionType == "addPoints" || 
                   actionType == "points-multiplier" || 
                   actionType == "bonus-points";
        }

        public async Task ExecuteAsync(RuleAction action, string userId, string txId, string ruleId)
        {
            // Calculate points (simplified logic assuming 'points' param exists)
            // Real logic might need access to transaction amount if multiplier
            // For now, mirroring existing logic which just grabs "points" from params
            
            decimal points = 0;
            if (action.Parameters.ContainsKey("points"))
            {
                var val = action.Parameters["points"];
                if (val is JsonElement je && je.ValueKind == JsonValueKind.Number)
                {
                    points = je.GetDecimal();
                } 
                else if (val is int i) points = i;
                else if (val is long l) points = l;
                else if (val is double d) points = (decimal)d;
            }

            var pointsPayload = new
            {
                type = "wallet.points.added", // Added for MessageDispatcher routing
                event_type = "PointsAdded",
                aggregate_type = "Wallet",
                aggregate_id = userId,
                userId = userId, // Added for PointsBalanceHandler
                points = points, // Added for PointsBalanceHandler
                campaignId = ruleId, // Added for PointsBalanceHandler
                transactionId = txId, // Added for PointsBalanceHandler
                timestamp = DateTime.UtcNow, // Added for PointsBalanceHandler
                payload = new
                {
                    CustomerId = userId,
                    TransactionId = txId,
                    Source = "RuleAction:" + action.ActionType,
                    Points = points,
                    Reason = "Campaign/Rule matched"
                }
            };

            await _outbox.EnqueueAsync("wallet.points.added", JsonSerializer.Serialize(pointsPayload), userId);
            
            // Audit Log
            if (Guid.TryParse(ruleId, out var campId))
            {
                var audit = new Worker.Models.CampaignExecutionEntity
                {
                    CampaignId = campId,
                    MemberId = userId,
                    TransactionId = txId,
                    RewardType = "POINTS",
                    ExecutedAt = DateTime.UtcNow,
                    ExecutionResultJson = JsonSerializer.Serialize(new { Points = points, Source = action.ActionType })
                };
                await _db.CampaignExecutions.AddAsync(audit);
                await _db.SaveChangesAsync();
            }
            
            _logger.LogInformation("PointsStrategy: Enqueued {Points} points for User {User} (Campaign {RuleId})", points, userId, ruleId);
        }
    }
}
