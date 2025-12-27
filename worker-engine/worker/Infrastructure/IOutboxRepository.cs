using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worker.Models; // adjust namespace to your entities location
using Worker.Engines; // adjust namespace to your engines location

namespace Worker.Infrastructure
{
    public interface IOutboxRepository
    {
        Task<List<OutboxMessage>> GetPendingAsync(int limit = 100);
        Task<OutboxMessage?> GetByIdAsync(object id); // or Task<OutboxMessage?> GetByIdAsync(long id);
        Task<bool> MarkSentAsync(long id, DateTime sentAt);
        Task IncrementTriesAsync(long id);
        Task EnqueueAsync(string topic, string payload, string? key = null);
    }
}
