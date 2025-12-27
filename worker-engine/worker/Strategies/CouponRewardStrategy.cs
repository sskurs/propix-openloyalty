using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Worker.Data;
using Worker.Engines;
using Worker.Infrastructure;
using Worker.Models;

namespace Worker.Strategies
{
    public class CouponRewardStrategy : IRewardStrategy
    {
        private readonly WorkerDbContext _db;
        private readonly IOutboxRepository _outbox;
        private readonly ILogger<CouponRewardStrategy> _logger;

        public CouponRewardStrategy(WorkerDbContext db, IOutboxRepository outbox, ILogger<CouponRewardStrategy> logger)
        {
            _db = db;
            _outbox = outbox;
            _logger = logger;
        }

        public bool CanHandle(string actionType)
        {
            return actionType.Equals("COUPON", StringComparison.OrdinalIgnoreCase) || 
                   actionType.Equals("issueCoupon", StringComparison.OrdinalIgnoreCase);
        }

        public async Task ExecuteAsync(RuleAction action, string userId, string txId, string ruleId)
        {
            // Expect 'couponId' or 'code' in parameters to know WHICH coupon definition to issue
            // For simplicity, let's assume we pass 'couponCode' or 'couponId'
            
            // 1. Find Coupon Definition
            Guid? couponDefId = null;
            if (action.Parameters.TryGetValue("couponId", out var cidObj))
            {
                 if (cidObj is JsonElement je && je.ValueKind == JsonValueKind.String && Guid.TryParse(je.GetString(), out var g))
                     couponDefId = g;
            }

            Coupon? couponDef = null;
            if (couponDefId.HasValue)
            {
                couponDef = await _db.Coupons.FindAsync(couponDefId.Value);
            }
            
            // Fallback: lookup by code if provided
            if (couponDef == null && action.Parameters.TryGetValue("couponCode", out var codeObj))
            {
                var code = codeObj.ToString();
                if (codeObj is JsonElement je) code = je.GetString();

                couponDef = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == code);
            }

            if (couponDef == null)
            {
                _logger.LogWarning("CouponStrategy: Could not find coupon definition for action params");
                return;
            }

            // 2. Issue MemberCoupon
            var memberCoupon = new MemberCoupon
            {
                Id = Guid.NewGuid(),
                MemberId = userId,
                CouponCode = couponDef.Code,
                AssignedAt = DateTime.UtcNow,
                Status = "active"
            };

            await _db.MemberCoupons.AddAsync(memberCoupon);
            
            // 3. Audit Log (CampaignExecutions)
            if (Guid.TryParse(ruleId, out var campId))
            {
                var audit = new CampaignExecutionEntity
                {
                    CampaignId = campId,
                    MemberId = userId,
                    TransactionId = txId,
                    RewardType = "COUPON",
                    ExecutedAt = DateTime.UtcNow,
                    ExecutionResultJson = JsonSerializer.Serialize(new { CouponCode = couponDef.Code, MemberCouponId = memberCoupon.Id })
                };
                await _db.CampaignExecutions.AddAsync(audit);
            }

            await _db.SaveChangesAsync();

            // 4. Emit Event (Optional but good for UI notification)
            var eventPayload = new
            {
                event_type = "CouponIssued",
                aggregate_type = "Member",
                aggregate_id = userId,
                payload = new
                {
                    CustomerId = userId,
                    CouponCode = couponDef.Code,
                    CouponId = couponDef.Id,
                    MemberCouponId = memberCoupon.Id,
                    DiscountValue = couponDef.DiscountValue
                }
            };

            await _outbox.EnqueueAsync("member.coupon.issued", JsonSerializer.Serialize(eventPayload), userId);

            _logger.LogInformation("CouponStrategy: Issued coupon {Code} to User {User} (Campaign {RuleId})", couponDef.Code, userId, ruleId);
        }
    }
}
