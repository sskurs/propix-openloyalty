using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using Worker.Infrastructure;
using Worker.Models;

namespace Worker.Services
{
    public class OutboxPublisherService : BackgroundService
    {
        private readonly ILogger<OutboxPublisherService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ProducerConfig _producerConfig;

        public OutboxPublisherService(ILogger<OutboxPublisherService> logger,
                                      IServiceProvider serviceProvider,
                                      ProducerConfig producerConfig)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _producerConfig = producerConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox publisher starting.");

            // Create the Kafka producer once (thread-safe usage depends on Confluent client)
            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();

            while (!stoppingToken.IsCancellationRequested)
            {    _logger.LogInformation("Outbox publisher starting.222222222");
                try
                {
                    // create a new DI scope for each fetch/process cycle
                    using var scope = _serviceProvider.CreateScope();
                    var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

                    var items = await outboxRepo.GetPendingAsync(100);
                    _logger.LogInformation("Found {Count} pending outbox messages", items.Count);
                    
                    foreach (var item in items)
                    {
                        string key = item.Key ?? item.Id.ToString();
                        string value = item.Payload ?? "{}";
                        var topic = item.Topic ?? "default-topic";
                        
                        var dr = await producer.ProduceAsync(topic, new Message<string,string>{ Key = key, Value = value }, stoppingToken);

                        _logger.LogInformation("Published outbox {Id} to {Topic} offset {Offset}", item.Id, dr.Topic, dr.Offset);

                        await outboxRepo.MarkSentAsync(item.Id, DateTime.UtcNow);
                    }
                }
                catch (OperationCanceledException) { /* shutting down */ }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during outbox publish loop");
                }

                // delay between poll iterations
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.LogInformation("Outbox publisher stopping.");
        }
    }
}
