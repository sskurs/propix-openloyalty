using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Worker.Data;
using Worker.Models;
using Worker.Infrastructure;

namespace Worker.Handlers
{
    public class MemberRegisteredHandler
    {
        private readonly WorkerDbContext _db;
        private readonly ILogger<MemberRegisteredHandler> _log;
        private readonly IOutboxRepository _outboxRepo;

        public MemberRegisteredHandler(WorkerDbContext db, ILogger<MemberRegisteredHandler> log, IOutboxRepository outboxRepo)
        {
            _db = db;
            _log = log;
            _outboxRepo = outboxRepo;
        }

        public async Task<bool> HandleAsync(ConsumeResult<string, string> msg, CancellationToken ct)
        {
            try
            {
                using var doc = JsonDocument.Parse(msg.Message.Value);
                var root = doc.RootElement;
                
                var memberId = root.GetProperty("MemberId").GetGuid();
                var firstName = root.GetProperty("FirstName").GetString();
                var lastName = root.GetProperty("LastName").GetString();

                _log.LogInformation("Processing member registration for {FirstName} {LastName} ({MemberId})", firstName, lastName, memberId);

                // Simulation: Award welcome points and log to system activity
                // In a real scenario, we might upsert member data to the worker db here if needed.

                var activityPayload = JsonSerializer.Serialize(new
                {
                    Type = "New Member",
                    Description = $"{firstName} {lastName} joined the program!",
                    Variant = "secondary",
                    CreatedAt = DateTime.UtcNow
                });

                var outboxMsg = new OutboxMessage
                {
                    Topic = "system.activity",
                    Payload = activityPayload,
                    CreatedAt = DateTime.UtcNow,
                    Status = "pending"
                };

                _db.Outbox.Add(outboxMsg);
                await _db.SaveChangesAsync(ct);

                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error handling member registration");
                return false;
            }
        }
    }
}
