using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Worker.Handlers;

namespace Worker.Services
{
    public class EarningRuleConsumerService : BackgroundService
    {
        private readonly ILogger<EarningRuleConsumerService> _logger;
        private readonly IServiceProvider _services;
        private readonly ConsumerConfig _consumerConfig;
        private IConsumer<string, string>? _consumer;

        private const string Topic = "earning-rules.created";

        public EarningRuleConsumerService(ILogger<EarningRuleConsumerService> logger, IServiceProvider services, ConsumerConfig consumerConfig)
        {
            _logger = logger;
            _services = services;
            _consumerConfig = consumerConfig;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting EarningRuleConsumerService");
            var builder = new ConsumerBuilder<string, string>(_consumerConfig);
            _consumer = builder.Build();
            _consumer.Subscribe(Topic);
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_consumer == null)
            {
                _logger.LogError("Kafka consumer not initialized");
                return;
            }

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = _consumer.Consume(stoppingToken); // blocking call

                        if (cr == null) continue;

                        string key = cr.Message.Key ?? Guid.NewGuid().ToString();
                        string value = cr.Message.Value ?? string.Empty;

                        _logger.LogInformation("Consumed message {TopicPartitionOffset} key={Key}", cr.TopicPartitionOffset, key);

                        // Dispatch to a scoped handler
                        using var scope = _services.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<EarningRuleCreatedHandler>();

                        // Do not await inside the same thread if you want parallelism — here we await to preserve ordering per consumer partition
                        await handler.HandleAsync(key, value);

                        // commit offset after successful handling
                        try
                        {
                            _consumer.Commit(cr);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Commit failed for offset {Offset}", cr.TopicPartitionOffset);
                        }
                    }
                    catch (ConsumeException cex)
                    {
                        _logger.LogError(cex, "Consume error");
                        // optionally handle poison messages or move to DLQ if repeated
                    }
                    catch (OperationCanceledException)
                    {
                        // shutting down
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error in consumer loop");
                    }
                }
            }
            finally
            {
                try
                {
                    _consumer.Close();
                    _consumer.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to cleanly shutdown consumer");
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping EarningRuleConsumerService");
            return base.StopAsync(cancellationToken);
        }
    }
}
