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
    public class SegmentsController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;

        public SegmentsController(LoyaltyDbContext context)
        {
            _context = context;
        }

        // GET: api/Segments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Segment>>> GetSegments()
        {
            // In a real app, you would project to a DTO and calculate member counts.
            // For now, returning the raw entity.
            return await _context.Segments.AsNoTracking().ToListAsync();
        }

        // GET: api/Segments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Segment>> GetSegment(Guid id)
        {
            var segment = await _context.Segments.FindAsync(id);

            if (segment == null)
            {
                return NotFound();
            }

            return segment;
        }

        // POST: api/Segments
        [HttpPost]
        public async Task<ActionResult<Segment>> PostSegment(CreateSegmentDto createSegmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newSegment = new Segment
            {
                Name = createSegmentDto.Name,
                Description = createSegmentDto.Description,
                Type = createSegmentDto.Type,
                ConditionsJson = JsonSerializer.Serialize(createSegmentDto.Conditions, new JsonSerializerOptions { WriteIndented = false }),
            };

            _context.Segments.Add(newSegment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSegment), new { id = newSegment.Id }, newSegment);
        }
    }
}
