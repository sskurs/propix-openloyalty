using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;

namespace OpenLoyalty.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiersController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;

        public TiersController(LoyaltyDbContext context)
        {
            _context = context;
        }

        // GET: api/Tiers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tier>>> GetTiers()
        {
            return await _context.Tiers.ToListAsync();
        }

        // GET: api/Tiers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tier>> GetTier(Guid id)
        {
            var tier = await _context.Tiers.FindAsync(id);

            if (tier == null)
            {
                return NotFound();
            }

            return tier;
        }
    }
}
