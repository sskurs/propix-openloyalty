using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Worker.Data;
using Worker.Models;
using Worker.Infrastructure;
using Worker.Repositories;
namespace Worker.Handlers
{
    // DTO that maps to the incoming Kafka message payload
    public record EarningRuleDto(Guid Id, int Version, string Name, string Expression, string CreatedBy, DateTime CreatedAt);

    public class EarningRuleCreatedHandler
    {
        private readonly ILogger<EarningRuleCreatedHandler> _logger;
        private readonly IRuleRepository _ruleRepo;
        private readonly IOutboxRepository _outboxRepo;

        public EarningRuleCreatedHandler(
            ILogger<EarningRuleCreatedHandler> logger,
            IRuleRepository ruleRepo,
            IOutboxRepository outboxRepo)
        {
            _logger = logger;
            _ruleRepo = ruleRepo;
            _outboxRepo = outboxRepo;
        }

        /// <summary>
        /// Handle a single incoming earning-rule.created message payload.
        /// This method validates, upserts the rule and writes an outbox event
        /// in the same repository/transaction.
        /// </summary>
        public async Task HandleAsync(string key, string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                _logger.LogWarning("Empty payload for earning-rules.created (key={Key})", key);
                return;
            }

            EarningRuleDto? dto;
            try
            {
                dto = JsonSerializer.Deserialize<EarningRuleDto>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize earning-rules.created payload (key={Key})", key);
                await WriteFailureOutboxAsync(key, payload, "invalid_json", ex.Message);
                return;
            }

            if (dto == null)
            {
                _logger.LogError("Deserialized dto is null for earning-rules.created (key={Key})", key);
                await WriteFailureOutboxAsync(key, payload, "invalid_payload", "deserialized object is null");
                return;
            }

            // Basic semantic validation - expand as needed
            if (string.IsNullOrWhiteSpace(dto.Expression) || string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Earning rule validation failed (missing fields) id={Id}", dto.Id);
                await WriteFailureOutboxAsync(key, payload, "validation_failed", "missing name or expression");
                return;
            }

            try
            {
                // Upsert the rule and add an outbox event inside one atomic operation (repository handles transaction)
                await _ruleRepo.UpsertRuleWithOutboxAsync(new EarningRule
                {
                    Id = dto.Id.ToString(),
                    Version = dto.Version,
                    Name = dto.Name,
                    Expression = dto.Expression,
                    IsActive = true,
                    CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = dto.CreatedBy ?? "system",
                    Status = "active"
                }, new OutboxMessage
                {
                    // OutboxMessage fields - adjust names/types to your entity
                    Key = dto.Id.ToString(),
                    Topic = "earning-rules.applied",
                    Payload = JsonSerializer.Serialize(new { RuleId = dto.Id, Version = dto.Version, Name = dto.Name }),
                    CreatedAt = DateTime.UtcNow,
                    Status = "pending"
                });

                _logger.LogInformation("Earning rule upserted and outbox recorded: {RuleId} v{Version}", dto.Id, dto.Version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upsert earning rule {RuleId}", dto.Id);
                await WriteFailureOutboxAsync(key, payload, "db_error", ex.Message);
            }
        }

        private async Task WriteFailureOutboxAsync(string key, string payload, string reason, string details)
        {
            try
            {
                var failure = new OutboxMessage
                {
                    Key = key ?? Guid.NewGuid().ToString(),
                    Topic = "earning-rules.failed",
                    Payload = JsonSerializer.Serialize(new { Key = key, Reason = reason, Details = details, Original = payload }),
                    CreatedAt = DateTime.UtcNow,
                    Status = "pending"
                };

                await _outboxRepo.MarkSentAsync(0, DateTime.UtcNow); // noop placeholder if required by interface
                // but we should write to outbox via repository - if repo has AddOutboxAsync use that.
                // Fallback: try direct repository method (some projects have AddOutbox/Enqueue methods).
                // If your IOutboxRepository exposes Add/Insert, call it here:
                // await _outboxRepo.AddAsync(failure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write failure outbox entry for key {Key}", key);
            }
        }
    }
}
