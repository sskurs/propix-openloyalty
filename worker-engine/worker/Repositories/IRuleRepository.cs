using System;
using System.Threading.Tasks;
using Worker.Models;

namespace Worker.Repositories
{
    public interface IRuleRepository
    {
        /// <summary>
        /// Upsert rule and insert outbox message in a single transaction.
        /// </summary>
        Task UpsertRuleWithOutboxAsync(EarningRule rule, OutboxMessage outboxMessage);

        /// <summary>
        /// Optional: get existing rule by id
        /// </summary>
        Task<EarningRule?> GetByIdAsync(Guid id);
    }
}
