using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Worker.Data;
using Worker.Models;

namespace Worker.Repositories
{
    public class RuleRepository : IRuleRepository
    {
        private readonly WorkerDbContext _db;
        private readonly ILogger<RuleRepository> _logger;

        public RuleRepository(WorkerDbContext db, ILogger<RuleRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<EarningRule?> GetByIdAsync(Guid id)
        {
            return await _db.EarningRules.FirstOrDefaultAsync(r => r.Id.ToString() == id.ToString());
        }

        public async Task UpsertRuleWithOutboxAsync(EarningRule rule, OutboxMessage outboxMessage)
        {
            // This method must ensure atomicity: update/insert rule + insert outbox in single TX
            using var tx = await _db.Database.BeginTransactionAsync();

            var existing = await _db.EarningRules.FirstOrDefaultAsync(r => r.Id == rule.Id);

            if (existing == null)
            {
                _db.EarningRules.Add(rule);
            }
            else if (rule.Version > existing.Version)
            {
                existing.Name = rule.Name;
                existing.Expression = rule.Expression;
                existing.Version = rule.Version;
                existing.IsActive = rule.IsActive;
                existing.UpdatedAt = rule.UpdatedAt;
                existing.Status = rule.Status;
            }
            else
            {
                // same or older version - we will still insert outbox that signals applied/ignored if needed
                _logger.LogInformation("Received rule {Id} with version {Version} not greater than existing {ExistingVersion}. Skipping update.", rule.Id, rule.Version, existing.Version);
            }

            // write outbox
            _db.Outbox.Add(outboxMessage);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }
    }
}
