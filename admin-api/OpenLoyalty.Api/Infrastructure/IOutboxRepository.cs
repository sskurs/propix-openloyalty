using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenLoyalty.Api.Models;

namespace OpenLoyalty.Api.Infrastructure
{
    public interface IOutboxRepository
    {
        /// <summary>
        /// Add a new outbox message (persist immediately).
        /// </summary>
        Task EnqueueAsync(OutboxMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Read a batch of pending messages ordered by creation time.
        /// </summary>
        Task<IList<OutboxMessage>> GetPendingAsync(int maxBatchSize = 50, CancellationToken cancellationToken = default);

        /// <summary>
        /// Mark a message as in 'sending' state and increment tries.
        /// </summary>
        Task MarkAsSendingAsync(string messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Mark a message as sent (set status = 'sent' and SentAt).
        /// </summary>
        Task MarkAsSentAsync(string messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Mark a message as failed (set status = 'failed' and increment tries).
        /// </summary>
        Task MarkAsFailedAsync(string messageId, string? error = null, CancellationToken cancellationToken = default);
    }
}
