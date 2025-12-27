using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;

namespace OpenLoyalty.Api.Services
{
    public class ActivityLogConsumerService : BackgroundService
    {
        private readonly ILogger<ActivityLogConsumerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ConsumerConfig _consumerConfig;

        public ActivityLogConsumerService(
            ILogger<ActivityLogConsumerService> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ConsumerConfig consumerConfig)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _consumerConfig = consumerConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ActivityLogConsumerService starting.");
            await Task.Yield();

            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
            consumer.Subscribe("system.activity");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result != null)
                    {
                        await ProcessMessage(result.Message.Value, stoppingToken);
                        consumer.Commit(result);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming system.activity message.");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            _logger.LogInformation("ActivityLogConsumerService stopping.");
        }

        private async Task ProcessMessage(string payload, CancellationToken ct)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var activity = JsonSerializer.Deserialize<ActivityLog>(payload, options);
                
                if (activity != null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<LoyaltyDbContext>();
                    
                    activity.Id = Guid.NewGuid();
                    if (activity.CreatedAt == default) activity.CreatedAt = DateTime.UtcNow;

                    dbContext.ActivityLogs.Add(activity);
                    await dbContext.SaveChangesAsync(ct);
                    
                    _logger.LogInformation("Saved activity log: {Description}", activity.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing activity log payload.");
            }
        }
    }
}
