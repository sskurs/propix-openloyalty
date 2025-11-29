using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using System.Linq;

namespace OpenLoyalty.Api.Services
{
    public class PointsEngine
    {
        private readonly LoyaltyDbContext _context;

        public PointsEngine(LoyaltyDbContext context)
        {
            _context = context;
        }

        public int CalculatePoints(Member member, decimal transactionAmount)
        {
            if (member == null || member.Tier == null)
            {
                return (int)transactionAmount; 
            }

            var multiplier = member.Tier.Multiplier;
            return (int)(transactionAmount * multiplier);
        }

        public void ApplyTierUpgrade(Member member)
        {
            var tiers = _context.Tiers.OrderBy(t => t.ThresholdValue).ToList();
            Tier? newTier = null;

            // This logic will need to be updated to use the new Wallet/Transaction model
            // For now, it's just a placeholder to resolve build errors

            if (newTier != null && newTier.Id != member.TierId)
            {
                member.TierId = newTier.Id;
            }
        }
    }
}
