using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenLoyalty.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;

        public CampaignsController(LoyaltyDbContext context)
        {
            _context = context;
        }

        // GET: api/Campaigns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaigns()
        {
            return await _context.Campaigns.AsNoTracking().ToListAsync();
        }

        // GET: api/Campaigns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Campaign>> GetCampaign(Guid id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null) return NotFound();
            return campaign;
        }

        // POST: api/Campaigns
        [HttpPost]
        public async Task<ActionResult<Campaign>> PostCampaign(CreateCampaignDto createCampaignDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newCampaign = new Campaign
            {
                Name = createCampaignDto.Name,
                Description = createCampaignDto.Description,
                Status = createCampaignDto.Status,
                ValidFrom = createCampaignDto.ValidFrom,
                ValidTo = createCampaignDto.ValidTo
            };

            _context.Campaigns.Add(newCampaign);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCampaign), new { id = newCampaign.Id }, newCampaign);
        }

        // PUT: api/Campaigns/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaign(Guid id, Campaign campaign)
        {
            if (id != campaign.Id) return BadRequest();
            _context.Entry(campaign).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Campaigns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(Guid id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null) return NotFound();
            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
