using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using OpenLoyalty.Api.Jobs;
using OpenLoyalty.Api.Data; // AdminDbContext
using Microsoft.Extensions.DependencyInjection;
using System;

namespace OpenLoyalty.Api.Controllers
{
    [ApiController]
    [Route("internal")]
    public class InternalController : ControllerBase
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<InternalController> _logger;

        public InternalController(IServiceProvider sp, ILogger<InternalController> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        [HttpPost("publish-scheduled-rules")]
        public async Task<IActionResult> PublishScheduledRules()
        {
            // Create a scope so we can reuse RuleSchedulerJob logic (which depends on LoyaltyDbContext)
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LoyaltyDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<RuleSchedulerJob>>();
            var job = new RuleSchedulerJob(db, logger);

            // Execute the job logic once
            await job.Execute(null!); // null is ok because we do not use the context passed in

            _logger.LogInformation("Manual publish-scheduled-rules invoked by {User}", User?.Identity?.Name ?? "unknown");
            return Accepted(new { status = "ok", message = "Scheduled rules processed."});
        }
    }
}
