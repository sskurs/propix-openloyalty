using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using OpenLoyalty.Api.Models;
using OpenLoyalty.Api.Infrastructure;

namespace OpenLoyalty.Api.Services
{
    public class OutboxPublisherService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxPublisherService> _logger;
        private readonly ProducerConfig _producerConfig;
        private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(2);

        public OutboxPublisherService(IServiceProvider serviceProvider, ILogger<OutboxPublisherService> logger, ProducerConfig producerConfig)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _producerConfig = producerConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("->>>>>>>>>>>>>>>>>>OutboxPublisherService starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                        using (var producer = new ProducerBuilder<string, string>(_producerConfig).Build())
                        {
                            var batch = await outboxRepo.GetPendingAsync(50, stoppingToken);
                            if (batch == null || batch.Count == 0)
                            {
                                await Task.Delay(_pollInterval, stoppingToken);
                                continue;
                            }

                            foreach (var msg in batch)
                            {
                                await outboxRepo.MarkAsSendingAsync(msg.Id, stoppingToken);

                                try
                                {
                                    var kafkaMsg = new Message<string?, string> { Key = msg.Key, Value = msg.Payload };
                                    var dr = await producer.ProduceAsync(msg.Topic, kafkaMsg, stoppingToken);

                                    await outboxRepo.MarkAsSentAsync(msg.Id, stoppingToken);
                                    _logger.LogInformation("Outbox message {Id} published to {Topic} partition {Partition} offset {Offset}", msg.Id, dr.Topic, dr.Partition, dr.Offset);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Failed to produce outbox message {Id} to topic {Topic}.", msg.Id, msg.Topic);
                                    await outboxRepo.MarkAsFailedAsync(msg.Id, ex.Message, stoppingToken);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OutboxPublisherService error when fetching/publishing a batch. Retrying shortly.");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("OutboxPublisherService stopping.");
        }
    }
}
