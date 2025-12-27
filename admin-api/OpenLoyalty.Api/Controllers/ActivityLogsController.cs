using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenLoyalty.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityLogsController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;

        public ActivityLogsController(LoyaltyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetActivityLogs([FromQuery] int limit = 10)
        {
            return await _context.ActivityLogs
                .OrderByDescending(a => a.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}
