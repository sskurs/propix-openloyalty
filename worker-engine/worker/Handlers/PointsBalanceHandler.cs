using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Worker.Data;
using Worker.Models;

namespace Worker.Handlers
{
    public class PointsBalanceHandler
    {
        private readonly ILogger<PointsBalanceHandler> _logger;
        private readonly WorkerDbContext _db;

        public PointsBalanceHandler(ILogger<PointsBalanceHandler> logger, WorkerDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task HandleAsync(ConsumeResult<string, string> message, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Processing wallet.points.added message");

                // Parse the message payload
                var payload = JsonSerializer.Deserialize<PointsAddedMessage>(message.Value);
                if (payload == null)
                {
                    _logger.LogWarning("Failed to deserialize points added message");
                    return;
                }

                _logger.LogInformation(
                    "Points added: UserId={UserId}, Points={Points}, CampaignId={CampaignId}",
                    payload.UserId, payload.Points, payload.CampaignId
                );

                // Get or create member points record
                var memberPoints = await _db.MemberPoints
                    .FirstOrDefaultAsync(m => m.MemberId == payload.UserId, ct);

                if (memberPoints == null)
                {
                    _logger.LogInformation("Creating new member_points record for {UserId}", payload.UserId);
                    memberPoints = new MemberPoints
                    {
                        MemberId = payload.UserId,
                        PointsBalance = 0,
                        LifetimePoints = 0,
                        PointsEarnedThisMonth = 0,
                        PointsRedeemed = 0,
                        Tier = "BRONZE",
                        CreatedAt = DateTime.UtcNow,
                        LastUpdatedAt = DateTime.UtcNow
                    };
                    _db.MemberPoints.Add(memberPoints);
                }

                // Update balance
                var previousBalance = memberPoints.PointsBalance;
                memberPoints.PointsBalance += payload.Points;
                memberPoints.LifetimePoints += payload.Points;
                memberPoints.PointsEarnedThisMonth += payload.Points;
                memberPoints.LastUpdatedAt = DateTime.UtcNow;

                // Update tier based on lifetime points
                memberPoints.Tier = CalculateTier(memberPoints.LifetimePoints);

                // Create transaction record
                var pointsTx = new PointsTransaction
                {
                    MemberId = payload.UserId,
                    TransactionType = "EARNED",
                    Points = payload.Points,
                    BalanceAfter = memberPoints.PointsBalance,
                    CampaignId = payload.CampaignId,
                    OrderId = payload.TransactionId,
                    Description = $"Points earned from campaign {payload.CampaignId}",
                    Metadata = null,
                    CreatedAt = DateTime.UtcNow
                };
                _db.PointsTransactions.Add(pointsTx);

                // Save changes
                await _db.SaveChangesAsync(ct);

                _logger.LogInformation(
                    "Updated points for member {MemberId}: {PreviousBalance} + {Points} = {NewBalance} (Tier: {Tier})",
                    payload.UserId, previousBalance, payload.Points, memberPoints.PointsBalance, memberPoints.Tier
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing wallet.points.added message");
                throw;
            }
        }

        private string CalculateTier(decimal lifetimePoints)
        {
            if (lifetimePoints >= 10000) return "PLATINUM";
            if (lifetimePoints >= 5000) return "GOLD";
            if (lifetimePoints >= 1000) return "SILVER";
            return "BRONZE";
        }

        // DTO for deserializing the message
        private class PointsAddedMessage
        {
            public string UserId { get; set; } = "";
            public decimal Points { get; set; }
            public string? CampaignId { get; set; }
            public string? TransactionId { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
