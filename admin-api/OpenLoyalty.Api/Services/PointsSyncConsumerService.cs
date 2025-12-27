using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace OpenLoyalty.Api.Services
{
    public class PointsSyncConsumerService : BackgroundService
    {
        private readonly ILogger<PointsSyncConsumerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ConsumerConfig _consumerConfig;

        public PointsSyncConsumerService(
            ILogger<PointsSyncConsumerService> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ConsumerConfig consumerConfig)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            // Separate group for points sync to avoid offset conflicts
            _consumerConfig = new ConsumerConfig(consumerConfig)
            {
                GroupId = "api-points-sync-group"
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PointsSyncConsumerService starting.");
            await Task.Yield();

            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
            consumer.Subscribe("wallet.points.added");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result != null)
                    {
                        await ProcessMessage(result.Message.Value, stoppingToken);
                        consumer.Commit(result);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming wallet.points.added message.");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            _logger.LogInformation("PointsSyncConsumerService stopping.");
        }

        private async Task ProcessMessage(string payload, CancellationToken ct)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var envelope = JsonSerializer.Deserialize<PointsEnvelope>(payload, options);
                
                if (envelope != null && envelope.Payload != null)
                {
                    var p = envelope.Payload;
                    if (string.IsNullOrEmpty(p.CustomerId)) return;

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<LoyaltyDbContext>();
                    
                    // Robust Member Lookup
                    var member = await db.Members
                        .Include(m => m.Wallets)
                        .Where(m => m.ExternalId == p.CustomerId || m.Id.ToString() == p.CustomerId || m.Email == p.CustomerId)
                        .FirstOrDefaultAsync(ct);

                    if (member == null)
                    {
                        _logger.LogWarning("PointsSync: Member {CustomerId} not found. Skipping.", p.CustomerId);
                        return;
                    }

                    // Get or Create Points Wallet
                    var wallet = member.Wallets.FirstOrDefault(w => w.Type == "points");
                    if (wallet == null)
                    {
                        wallet = new Wallet
                        {
                            Id = Guid.NewGuid(),
                            MemberId = member.Id,
                            Type = "points",
                            Balance = 0,
                            // Status = "active", // Removed: Property doesn't exist on Wallet
                            Currency = "PTS",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        db.Wallets.Add(wallet);
                    }

                    // Update Balance
                    wallet.Balance += p.Points;
                    wallet.UpdatedAt = DateTime.UtcNow;
                    
                    // Create Wallet Log
                    var log = new WalletLog
                    {
                        Id = Guid.NewGuid(),
                        WalletId = wallet.Id,
                        MemberId = member.Id, 
                        Direction = "IN",
                        Amount = p.Points,
                        BalanceAfter = wallet.Balance,
                        ReasonCode = p.Reason ?? "Points Earned",
                        TransactionId = Guid.TryParse(p.TransactionId, out var txId) ? txId : null,
                        CreatedAt = DateTime.UtcNow,
                        WalletType = "points" // Added required property
                    };
                    db.WalletLogs.Add(log);

                    // Create Activity Log for Dashboard
                    var activityLog = new ActivityLog
                    {
                        Id = Guid.NewGuid(),
                        Type = "Points Earned",
                        Description = $"{member.FirstName} {member.LastName} earned {p.Points} points.",
                        Variant = "default",
                        CreatedAt = DateTime.UtcNow
                    };
                    db.ActivityLogs.Add(activityLog);

                    await db.SaveChangesAsync(ct);
                    
                    _logger.LogInformation("Added {Points} points to Member {MemberId} (Wallet {WalletId}). New Balance: {Balance}", p.Points, member.Id, wallet.Id, wallet.Balance);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing points payload: {Payload}", payload);
            }
        }

        private class PointsEnvelope
        {
            [System.Text.Json.Serialization.JsonPropertyName("event_type")]
            public string? EventType { get; set; }
            public PointsPayload? Payload { get; set; }
        }

        private class PointsPayload
        {
            public string? CustomerId { get; set; }
            public string? TransactionId { get; set; }
            public decimal Points { get; set; }
            public string? Source { get; set; }
            public string? Reason { get; set; }
        }
    }
}
