using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Worker.Handlers;

namespace Worker.Services
{
    public class CampaignConsumerService : BackgroundService
    {
        private readonly ILogger<CampaignConsumerService> _logger;
        private readonly IServiceProvider _services;
        private readonly ConsumerConfig _consumerConfig;
        private IConsumer<string, string>? _consumer;

        private readonly string[] _topics = { "campaign.created" };

        public CampaignConsumerService(ILogger<CampaignConsumerService> logger, IServiceProvider services, ConsumerConfig consumerConfig)
        {
            _logger = logger;
            _services = services;
            _consumerConfig = consumerConfig;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting CampaignConsumerService subscribing to {Topics}", string.Join(", ", _topics));
            var builder = new ConsumerBuilder<string, string>(_consumerConfig);
            _consumer = builder.Build();
            _consumer.Subscribe(_topics);
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_consumer == null) return Task.CompletedTask;

            // Offload the blocking Consume call to a background thread
            return Task.Run(async () =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var cr = _consumer.Consume(stoppingToken); // Blocking
                            if (cr == null) continue;

                            string key = cr.Message.Key ?? Guid.NewGuid().ToString();
                            string value = cr.Message.Value ?? string.Empty;

                            _logger.LogInformation("Consumed campaign message from {Topic}", cr.Topic);

                            using var scope = _services.CreateScope();
                            var handler = scope.ServiceProvider.GetRequiredService<CampaignHandler>();
                            
                            var success = await handler.HandleAsync(key, value);
                            
                            if (success)
                            {
                                _consumer.Commit(cr);
                            }
                        }
                        catch (ConsumeException cex)
                        {
                            _logger.LogError(cex, "Consume error in CampaignConsumerService");
                        }
                        catch (OperationCanceledException) { }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Unexpected error in CampaignConsumerService loop");
                        }
                    }
                }
                finally
                {
                    _consumer.Close();
                    _consumer.Dispose();
                }
            }, stoppingToken);
        }
    }
}
