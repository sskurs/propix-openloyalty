using Microsoft.Extensions.Logging;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using Quartz;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OpenLoyalty.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class RuleSchedulerJob : IJob
    {
        private readonly LoyaltyDbContext _db;
        private readonly ILogger<RuleSchedulerJob> _logger;

        public RuleSchedulerJob(LoyaltyDbContext db, ILogger<RuleSchedulerJob> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogWarning("--- RuleSchedulerJob INVOKED BY QUARTZ at {time} ---", DateTime.UtcNow);
            _logger.LogInformation("RuleSchedulerJob is processing time-based earning rules at {time}", DateTime.UtcNow);
            var now = DateTime.UtcNow;

            // 1. Activate rules where activate_at is in the past and status is DRAFT
            var rulesToActivate = await _db.EarningRules
                .Where(r => r.Status == "DRAFT" && r.ActivateAt <= now)
                .ToListAsync();

            foreach (var rule in rulesToActivate)
            {
                rule.Status = "ACTIVE";
                var outboxMessage = new OutboxMessage
                {
                    Topic = "earning-rule.activated",
                    Payload = JsonSerializer.Serialize(new 
                    { 
                        type = "earning_rule.activated",
                        payload = new 
                        { 
                            id = rule.Id, 
                            name = rule.Name, 
                            conditionJson = rule.ConditionJson,
                            pointsJson = rule.PointsJson,
                            activatedAt = now 
                        }
                    })
                };
                _db.OutboxMessages.Add(outboxMessage);
                _logger.LogInformation("Rule {RuleId} activated by scheduler.", rule.Id);
            }

            // 2. Deactivate rules where deactivate_at is in the past and status is ACTIVE
            var rulesToDeactivate = await _db.EarningRules
                .Where(r => r.Status == "ACTIVE" && r.DeactivateAt <= now)
                .ToListAsync();

            foreach (var rule in rulesToDeactivate)
            {
                rule.Status = "EXPIRED";
                 var outboxMessage = new OutboxMessage
                {
                    Topic = "earning-rule.deactivated",
                    Payload = JsonSerializer.Serialize(new { rule.Id, rule.Name, DeactivatedAt = now })
                };
                _db.OutboxMessages.Add(outboxMessage);
                _logger.LogInformation("Rule {RuleId} deactivated by scheduler.", rule.Id);
            }

            // 3. Process rules based on cron expression
            var cronRules = await _db.EarningRules
                .Where(r => r.Status == "ACTIVE" && !string.IsNullOrEmpty(r.CronExpression))
                .ToListAsync();
            
            foreach (var rule in cronRules)
            {
                try
                {
                    if (string.IsNullOrEmpty(rule.CronExpression)) continue;
                    var cron = new CronExpression(rule.CronExpression);
                    if (cron.IsSatisfiedBy(now))
                    {
                        var outboxMessage = new OutboxMessage
                        {
                            Topic = "earning-rule.cron.triggered",
                            Payload = JsonSerializer.Serialize(new { rule.Id, rule.Name, TriggeredAt = now })
                        };
                        _db.OutboxMessages.Add(outboxMessage);
                        _logger.LogInformation("Cron-based rule {RuleId} triggered.", rule.Id);
                    }
                }
                catch(Exception e)
                {
                    _logger.LogWarning(e, "Could not process cron expression for rule {RuleId}", rule.Id);
                }
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("RuleSchedulerJob finished processing.");
        }
    }
}
