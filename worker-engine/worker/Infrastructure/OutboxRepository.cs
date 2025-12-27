using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Worker.Data;           // adjust to your project's DbContext namespace
using Worker.Models;  // adjust for OutboxMessage entity

namespace Worker.Infrastructure
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly WorkerDbContext _db;

        public OutboxRepository(WorkerDbContext db)
        {
            _db = db;
        }

        public async Task<List<OutboxMessage>> GetPendingAsync(int limit = 100)
        {
            return await _db.Outbox
                .Where(o => o.Status == "pending")
                .OrderBy(o => o.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<OutboxMessage?> GetByIdAsync(object id)
        {
            if (id == null) return null;

            long idLong;
            switch (id)
            {
                case long l:
                    idLong = l;
                    break;
                case int i:
                    idLong = i;
                    break;
                case string s when long.TryParse(s, out var parsed):
                    idLong = parsed;
                    break;
                default:
                    return null;
            }

            return await _db.Outbox.FirstOrDefaultAsync(o => o.Id == idLong);
        }

        public async Task<bool> MarkSentAsync(long id, DateTime sentAt)
        {
            var msg = await _db.Outbox.FirstOrDefaultAsync(o => o.Id == id);
            if (msg == null) return false;
            msg.Status = "sent";
            msg.SentAt = sentAt;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task IncrementTriesAsync(long id)
        {
            var msg = await _db.Outbox.FirstOrDefaultAsync(o => o.Id == id);
            if (msg == null) return;
            msg.Tries++;
            await _db.SaveChangesAsync();
        }

        public async Task EnqueueAsync(string topic, string payload, string? key = null)
        {
            var msg = new OutboxMessage
            {
                Topic = topic,
                Payload = payload,
                Key = key,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                Tries = 0
            };
            await _db.Outbox.AddAsync(msg);
            await _db.SaveChangesAsync();
            Console.WriteLine($"[OutboxRepository] Enqueued message to topic '{topic}' with status '{msg.Status}'");
        }
    }
}
