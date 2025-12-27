using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace Worker.Infra
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<MessageDispatcher> _log;
        public MessageDispatcher(IServiceProvider sp, ILogger<MessageDispatcher> log)
        {
            _sp = sp;
            _log = log;
        }
        public async Task<bool> DispatchAsync(
            ConsumeResult<string, string> msg,
            CancellationToken ct
        )
        {
            try
            {
                using var doc = JsonDocument.Parse(msg.Message.Value);
                var type = doc.RootElement.GetProperty("type").GetString();
                _log.LogInformation("Dispatching message type: {Type}", type);
                if (type != null && type.StartsWith("earning_rule"))
                {
                    using var s = _sp.CreateScope();
                    var h =
                        s.ServiceProvider.GetRequiredService<Worker.Handlers.RuleUpdateHandler>();
                    return await h.HandleAsync(msg, ct);
                }
                if (type == "campaign.created" || type == "campaign.updated")
                {
                    using var s = _sp.CreateScope();
                    var h = s.ServiceProvider.GetRequiredService<Worker.Handlers.CampaignHandler>();
                    return await h.HandleAsync(msg.Key, msg.Message.Value);
                }
                if (type == "transaction.created")
                {
                    using var s = _sp.CreateScope();
                    var h = s.ServiceProvider.GetRequiredService<Worker.Handlers.TransactionHandler>();
                    return await h.HandleAsync(msg, ct);
                }
                if (type == "wallet.points.added")
                {
                    using var s = _sp.CreateScope();
                    var h = s.ServiceProvider.GetRequiredService<Worker.Handlers.PointsBalanceHandler>();
                    await h.HandleAsync(msg, ct);
                    return true;
                }
                _log.LogWarning("No handler found for message type: {Type}", type);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error dispatching message");
                return true;
            }
        }
    }
}

