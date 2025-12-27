using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace OpenLoyalty.Api.Services
{
    public class CouponSyncConsumerService : BackgroundService
    {
        private readonly ILogger<CouponSyncConsumerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ConsumerConfig _consumerConfig;

        public CouponSyncConsumerService(
            ILogger<CouponSyncConsumerService> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ConsumerConfig consumerConfig)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            // Create a copy and change GroupId for sync
            _consumerConfig = new ConsumerConfig(consumerConfig)
            {
                GroupId = "api-coupon-sync-group"
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CouponSyncConsumerService starting.");
            await Task.Yield();

            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
            consumer.Subscribe("member.coupon.issued");

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
                    _logger.LogError(ex, "Error consuming member.coupon.issued message.");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            _logger.LogInformation("CouponSyncConsumerService stopping.");
        }

        private async Task ProcessMessage(string payload, CancellationToken ct)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var envelope = JsonSerializer.Deserialize<CouponIssuedEnvelope>(payload, options);
                
                if (envelope != null && envelope.Payload != null)
                {
                    var p = envelope.Payload;
                    if (string.IsNullOrEmpty(p.CustomerId) || string.IsNullOrEmpty(p.CouponCode)) return;

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<LoyaltyDbContext>();
                    
                    // Optional: Verify member exists in main DB
                    // Relaxed check: ExternalId OR Id (as string) OR Email
                    var member = await db.Members.AnyAsync(m => 
                        m.ExternalId == p.CustomerId || 
                        m.Id.ToString() == p.CustomerId ||
                        m.Email == p.CustomerId
                    , ct);

                    if (!member)
                    {
                        // Fallback: Try to find by partial match if needed, but logging warning for now.
                        _logger.LogWarning("Member with identifier {CustomerId} not found in main DB (checked ExternalId, Id, Email). Skipping sync.", p.CustomerId);
                        return;
                    }

                    // Check for existing MemberCoupon (Idempotency)
                    bool exists = await db.MemberCoupons.AnyAsync(mc => mc.MemberId == p.CustomerId && mc.CouponCode == p.CouponCode && mc.AssignedAt >= DateTime.UtcNow.AddMinutes(-10), ct);
                    if (exists)
                    {
                         _logger.LogInformation("MemberCoupon for {CustomerId} / {CouponCode} already exists recently. Skipping.", p.CustomerId, p.CouponCode);
                         return;
                    }

                    // 4. Save MemberCoupon
                    var mc = new MemberCoupon
                    {
                        Id = Guid.NewGuid(),
                        MemberId = p.CustomerId,
                        CouponCode = p.CouponCode,
                        AssignedAt = DateTime.UtcNow,
                        Status = "active"
                    };

                    db.MemberCoupons.Add(mc);
                    await db.SaveChangesAsync(ct);
                    
                    _logger.LogInformation("Successfully synchronized coupon {CouponCode} for member {CustomerId}", p.CouponCode, p.CustomerId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing coupon issued payload.");
            }
        }

        private class CouponIssuedEnvelope
        {
            public string? Event_Type { get; set; }
            public CouponIssuedPayload? Payload { get; set; }
        }

        private class CouponIssuedPayload
        {
            public string? CustomerId { get; set; }
            public string? CouponCode { get; set; }
            public Guid CouponId { get; set; }
            public Guid MemberCouponId { get; set; }
        }
    }
}
