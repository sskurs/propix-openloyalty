using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenLoyalty.Api.Models;
using OpenLoyalty.Api.Data; // AdminDbContext namespace
using Microsoft.EntityFrameworkCore;

namespace OpenLoyalty.Api.Infrastructure
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly LoyaltyDbContext _db;
        public OutboxRepository(LoyaltyDbContext db) => _db = db;

        public async Task EnqueueAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            message.Status = string.IsNullOrEmpty(message.Status) ? "pending" : message.Status;
            message.CreatedAt = message.CreatedAt == default ? DateTime.UtcNow : message.CreatedAt;
            await _db.OutboxMessages.AddAsync(message, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IList<OutboxMessage>> GetPendingAsync(int maxBatchSize = 50, CancellationToken cancellationToken = default)
        {
            return await _db.OutboxMessages
                .Where(x => x.Status == "pending")
                .OrderBy(x => x.CreatedAt)
                .Take(maxBatchSize)
                .ToListAsync(cancellationToken);
        }

        public async Task MarkAsSendingAsync(string messageId, CancellationToken cancellationToken = default)
        {
            var msg = await _db.OutboxMessages.FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);
            if (msg == null) return;
            msg.Status = "sending";
            msg.Tries += 1;
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsSentAsync(string messageId, CancellationToken cancellationToken = default)
        {
            var msg = await _db.OutboxMessages.FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);
            if (msg == null) return;
            msg.Status = "sent";
            msg.SentAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsFailedAsync(string messageId, string? error = null, CancellationToken cancellationToken = default)
        {
            var msg = await _db.OutboxMessages.FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);
            if (msg == null) return;
            msg.Status = "failed";
            msg.Tries += 1;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
