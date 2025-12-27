using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Customer.Api.Data;
using OpenLoyalty.Customer.Api.Models.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenLoyalty.Customer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;

        public CustomerController(LoyaltyDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest("Email is required");

            var searchEmail = email.Trim().ToLower();
            Console.WriteLine($"[GetProfile] Searching for email: '{searchEmail}' (Original: '{email}')");

            var member = await _context.Members
                .Include(m => m.Tier)
                .Include(m => m.Wallets)
                .FirstOrDefaultAsync(m => m.Email.ToLower() == searchEmail);

            if (member == null)
            {
                Console.WriteLine($"[GetProfile] Member not found for email: '{searchEmail}'");
                return NotFound($"Member not found: {searchEmail}");
            }

            return Ok(new MemberProfileDto
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                Phone = member.Phone,
                Status = member.Status,
                JoinDate = member.JoinDate,
                TierName = member.Tier?.Name ?? "None",
                Wallets = member.Wallets.Select(w => new WalletDto { Type = w.Type, Balance = w.Balance }).ToList()
            });
        }

        [HttpGet("points/balance")]
        public async Task<IActionResult> GetPointsBalance([FromQuery] Guid memberId)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.MemberId == memberId && w.Type == "points");
            
            return Ok(new { balance = wallet?.Balance ?? 0 });
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] Guid memberId)
        {
            var logs = await _context.WalletLogs
                .Where(l => l.MemberId == memberId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            var events = logs.Select(l => new
            {
                id = l.Id,
                title = l.Direction == "IN" ? "Points Earned" : "Points Redeemed",
                description = l.ReasonCode ?? (l.Direction == "IN" ? "Earned from transaction" : "Redeemed for reward"),
                eventType = l.Direction == "IN" ? "POINT_EARN" : "REWARD_REDEEM",
                occurredAt = l.CreatedAt,
                metadata = new { points = l.Amount }
            });
            
            return Ok(events);
        }

        [HttpGet("rewards")]
        public async Task<IActionResult> GetRewards()
        {
            var rewards = await _context.Rewards
                .Where(r => r.IsActive)
                .ToListAsync();
            return Ok(rewards);
        }

        [HttpGet("campaigns")]
        public async Task<IActionResult> GetCampaigns()
        {
            var now = DateTime.UtcNow;
            var campaigns = await _context.Campaigns
                .AsNoTracking()
                .Where(c => c.Status == "ACTIVE" && c.StartAt <= now && (c.EndAt == null || c.EndAt >= now))
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.Status,
                    Type = "Standard",
                    ValidTo = c.EndAt
                })
                .ToListAsync();
            return Ok(campaigns);
        }

        [HttpGet("coupons")]
        public async Task<IActionResult> GetCoupons([FromQuery] string memberId)
        {
            if (string.IsNullOrWhiteSpace(memberId)) return BadRequest("MemberId is required");

            var coupons = await _context.MemberCoupons
                .Where(c => c.MemberId == memberId)
                .OrderByDescending(c => c.AssignedAt)
                .ToListAsync();

            return Ok(coupons);
        }
    }
}
