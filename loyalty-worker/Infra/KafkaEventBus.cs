using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

public class KafkaEventBus : IEventBus {
    private readonly IConfiguration _cfg;
    private readonly ConsumerConfig _conf;
    private readonly string _topic;

    public KafkaEventBus(IConfiguration cfg) {
        _cfg = cfg;
        _conf = new ConsumerConfig {
            BootstrapServers = cfg["Kafka:BootstrapServers"],
            GroupId = cfg["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };
        _topic = cfg["Kafka:Topic"] ?? "events";
    }

    public async Task ConsumeAsync(Func<string, string, Task> onMessage, CancellationToken ct) {
        using var consumer = new ConsumerBuilder<Ignore, string>(_conf).Build();
        consumer.Subscribe(_topic);

        try {
            while (!ct.IsCancellationRequested) {
                var cr = consumer.Consume(ct);
                if (cr != null) {
                    await onMessage(cr.Message.Key ?? "", cr.Message.Value);
                }
            }
        }
        catch (OperationCanceledException) {}
        finally {
            consumer.Close();
        }
    }
}
