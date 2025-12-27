using System; using System.Threading; using System.Threading.Tasks; using Microsoft.Extensions.Hosting; using Microsoft.Extensions.Logging; using Microsoft.Extensions.Configuration; using Microsoft.Extensions.DependencyInjection; using Confluent.Kafka;
namespace Worker.Services {
  public class KafkaConsumerService : BackgroundService {
    private readonly ILogger<KafkaConsumerService> _logger; private readonly IConfiguration _cfg; private readonly IServiceScopeFactory _sc; private IConsumer<string,string> _consumer;
    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConfiguration cfg, IServiceScopeFactory sc){
      _logger=logger; _cfg=cfg; _sc=sc;
      _logger.LogInformation("KafkaConsumerService: Constructor called.");
      try {
        var conf=new ConsumerConfig{ BootstrapServers=_cfg["Kafka:BootstrapServers"], GroupId=_cfg["Kafka:ConsumerGroup"] ?? "worker-group", AutoOffsetReset=AutoOffsetReset.Earliest};
        _consumer=new ConsumerBuilder<string,string>(conf).Build();
        _logger.LogInformation("KafkaConsumerService: Consumer built successfully.");
      } catch (Exception ex) {
         _logger.LogError(ex, "KafkaConsumerService: Failed to build consumer in constructor.");
         throw;
      }
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken){
      _logger.LogInformation("KafkaConsumerService: ExecuteAsync called.");
      Task.Run(()=>ConsumeLoop(stoppingToken),stoppingToken); return Task.CompletedTask;
    }
    private async Task ConsumeLoop(CancellationToken ct){
      _logger.LogInformation("KafkaConsumerService: ConsumeLoop starting.");
      var topics = (_cfg["Kafka:SubscribeTopics"] ?? "rules.updates").Split(',',StringSplitOptions.RemoveEmptyEntries);
      _logger.LogInformation("KafkaConsumerService subscribing to: {Topics}", string.Join(", ", topics));
      _consumer.Subscribe(topics);
      while(!ct.IsCancellationRequested){
        try{
          var cr=_consumer.Consume(ct);
          using(var scope=_sc.CreateScope()){
            var disp=scope.ServiceProvider.GetRequiredService<Worker.Infra.IMessageDispatcher>();
            var ok=await disp.DispatchAsync(cr, ct);
            if(ok) _consumer.Commit(cr);
          }
        }catch(Exception ex){ _logger.LogError(ex,"consume error"); await Task.Delay(1000, ct); }
      }
    }
  }
}

