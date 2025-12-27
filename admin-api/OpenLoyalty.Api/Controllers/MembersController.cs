using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenLoyalty.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;

        public MembersController(LoyaltyDbContext context)
        {
            _context = context;
        }

        // GET: api/Members/by-email/test@example.com
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<MemberDetailsDto>> GetMemberByEmail(string email)
        {
            var member = await _context.Members
                .AsNoTracking()
                .Include(m => m.Tier)
                .Include(m => m.Wallets)
                .Select(m => new MemberDetailsDto
                {
                    Id = m.Id,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Email = m.Email,
                    Phone = m.Phone,
                    Gender = m.Gender,
                    DateOfBirth = m.DateOfBirth,
                    JoinDate = m.JoinDate,
                    Status = m.Status,
                    ExternalId = m.ExternalId,
                    TierName = m.Tier != null ? m.Tier.Name : null,
                    Wallets = m.Wallets.Select(w => new WalletDto { Id = w.Id, Type = w.Type, Balance = w.Balance, Currency = w.Currency }).ToList()
                })
                .FirstOrDefaultAsync(m => m.Email == email);

            if (member == null)
            {
                return NotFound();
            }

            return Ok(member);
        }

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberListDto>>> GetMembers()
        {
            return await _context.Members
                .AsNoTracking()
                .Include(m => m.Tier)
                .Include(m => m.Wallets)
                .Select(m => new MemberListDto
                {
                    Id = m.Id,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Email = m.Email,
                    Phone = m.Phone,
                    ExternalId = m.ExternalId,
                    TierName = m.Tier != null ? m.Tier.Name : null,
                    PointsBalance = m.Wallets.Where(w => w.Type == "points").Sum(w => w.Balance),
                    Status = m.Status,
                    JoinDate = m.JoinDate
                })
                .ToListAsync();
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDetailsDto>> GetMember(Guid id)
        {
            var member = await _context.Members
                .AsNoTracking()
                .Include(m => m.Tier)
                .Include(m => m.Wallets)
                .Select(m => new MemberDetailsDto
                {
                    Id = m.Id,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Email = m.Email,
                    Phone = m.Phone,
                    Gender = m.Gender,
                    DateOfBirth = m.DateOfBirth,
                    JoinDate = m.JoinDate,
                    Status = m.Status,
                    ExternalId = m.ExternalId,
                    TierName = m.Tier != null ? m.Tier.Name : null,
                    Wallets = m.Wallets.Select(w => new WalletDto { Id = w.Id, Type = w.Type, Balance = w.Balance, Currency = w.Currency }).ToList()
                })
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return NotFound();
            }

            return Ok(member);
        }

        // GET: api/Members/5/wallets
        [HttpGet("{id}/wallets")]
        public async Task<ActionResult<IEnumerable<WalletDto>>> GetMemberWallets(Guid id)
        {
            var wallets = await _context.Wallets
                .Where(w => w.MemberId == id)
                .Select(w => new WalletDto { Id = w.Id, Type = w.Type, Balance = w.Balance, Currency = w.Currency })
                .ToListAsync();
            
            return Ok(wallets);
        }

        // GET: api/Members/5/timeline
        [HttpGet("{id}/timeline")]
        public Task<ActionResult<IEnumerable<object>>> GetMemberTimeline(Guid id)
        {
            // This is a placeholder. In a real application, you would query 
            // a dedicated audit or events table.
            var events = new List<object>
            {
                new { id = 1001, eventType = "POINT_EARN", title = "Points earned", description = "+200 pts from Order #1234", createdAt = DateTime.UtcNow },
                new { id = 1000, eventType = "TIER_CHANGE", title = "Tier changed", description = "SILVER -> GOLD", createdAt = DateTime.UtcNow.AddDays(-1) },
            };
            return Task.FromResult<ActionResult<IEnumerable<object>>>(Ok(events));
        }

        // GET: api/Members/5/expiring-points
        [HttpGet("{id}/expiring-points")]
        public Task<ActionResult<IEnumerable<object>>> GetMemberExpiringPoints(Guid id)
        {
            // Placeholder data
            var expiring = new List<object>
            {
                new { expiryDate = DateTime.UtcNow.AddDays(30), points = 200, source = "Campaign: Diwali Booster" },
                new { expiryDate = DateTime.UtcNow.AddDays(45), points = 300, source = "Base earn" },
            };
            return Task.FromResult<ActionResult<IEnumerable<object>>>(Ok(expiring));
        }

        // GET: api/Members/5/usage-history
        [HttpGet("{id}/usage-history")]
        public async Task<ActionResult<IEnumerable<MemberUsageHistoryDto>>> GetMemberUsageHistory(Guid id)
        {
            var usage = await _context.CampaignUsages
                .AsNoTracking()
                .Include(u => u.Campaign)
                .Where(u => u.CustomerId == id)
                .Select(u => new MemberUsageHistoryDto
                {
                    Id = u.Id,
                    CampaignName = u.Campaign != null ? u.Campaign.Name : "Deleted Campaign",
                    UsageCount = u.UsageCount,
                    LastUsedAt = u.LastUsedAt
                })
                .ToListAsync();

            return Ok(usage);
        }

        // GET: api/Members/5/redemption-history
        [HttpGet("{id}/redemption-history")]
        public async Task<ActionResult<IEnumerable<MemberRedemptionHistoryDto>>> GetMemberRedemptionHistory(Guid id)
        {
            var redemptions = await _context.RewardRedemptions
                .AsNoTracking()
                .Include(r => r.Reward)
                .Where(r => r.MemberId == id)
                .Select(r => new MemberRedemptionHistoryDto
                {
                    Id = r.Id,
                    RewardName = r.Reward != null ? r.Reward.Name : "Deleted Reward",
                    PointsSpent = r.PointsSpent,
                    CashbackSpent = r.CashbackSpent,
                    Quantity = r.Quantity,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return Ok(redemptions);
        }

        // POST: api/Members
        [HttpPost]
        public async Task<ActionResult<MemberDetailsDto>> PostMember(CreateMemberDto createMemberDto)
        {
            if (!ModelState.IsValid || !createMemberDto.HasAgreedToTerms)
            {
                return BadRequest("Invalid data provided or terms not agreed.");
            }

            if (await _context.Members.AnyAsync(m => m.Email == createMemberDto.Email))
            {
                return Conflict("A member with this email address already exists.");
            }

            var bronzeTier = await _context.Tiers.FirstOrDefaultAsync(t => t.Code == "BRONZE");
            if (bronzeTier == null)
            {
                return StatusCode(500, "Default 'Bronze' tier not found. Please seed the database.");
            }

            var newMember = new Member
            {
                Id = Guid.NewGuid(),
                FirstName = createMemberDto.FirstName,
                LastName = createMemberDto.LastName,
                Email = createMemberDto.Email,
                Phone = createMemberDto.PhoneNumber,
                Gender = createMemberDto.Gender,
                DateOfBirth = createMemberDto.DateOfBirth,
                JoinDate = DateTime.UtcNow,
                Status = "active",
                TierId = bronzeTier.Id,
                ExternalId = createMemberDto.ExternalId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var pointsWallet = new Wallet
            {
                Id = Guid.NewGuid(),
                MemberId = newMember.Id,
                Type = "points",
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var cashbackWallet = new Wallet
            {
                Id = Guid.NewGuid(),
                MemberId = newMember.Id,
                Type = "cashback",
                Balance = 0,
                Currency = "USD",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Members.Add(newMember);
            _context.Wallets.Add(pointsWallet);
            _context.Wallets.Add(cashbackWallet);

            // Produce member.registered event via Outbox
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid().ToString(),
                Key = newMember.Id.ToString(),
                Topic = "member.registered",
                Payload = JsonSerializer.Serialize(new
                {
                    MemberId = newMember.Id,
                    FirstName = newMember.FirstName,
                    LastName = newMember.LastName,
                    Email = newMember.Email,
                    JoinDate = newMember.JoinDate
                }),
                CreatedAt = DateTime.UtcNow,
                Status = "pending"
            };
            _context.OutboxMessages.Add(outboxMessage);

            await _context.SaveChangesAsync();

            var memberDto = new MemberDetailsDto
            {
                Id = newMember.Id,
                FirstName = newMember.FirstName,
                LastName = newMember.LastName,
                Email = newMember.Email,
                JoinDate = newMember.JoinDate,
                Status = newMember.Status,
                TierName = bronzeTier.Name,
                Wallets = new List<WalletDto>
                {
                    new WalletDto { Id = pointsWallet.Id, Type = pointsWallet.Type, Balance = pointsWallet.Balance, Currency = pointsWallet.Currency },
                    new WalletDto { Id = cashbackWallet.Id, Type = cashbackWallet.Type, Balance = cashbackWallet.Balance, Currency = cashbackWallet.Currency }
                }
            };

            return CreatedAtAction(nameof(GetMember), new { id = newMember.Id }, memberDto);
        }
        // PUT: api/Members/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(Guid id, Member member)
        {
            if (id != member.Id) return BadRequest();

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Members.AnyAsync(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }
    }
}
