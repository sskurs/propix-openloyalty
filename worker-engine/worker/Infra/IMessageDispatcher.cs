using System.Threading; using System.Threading.Tasks; using Confluent.Kafka;
namespace Worker.Infra { public interface IMessageDispatcher { Task<bool> DispatchAsync(ConsumeResult<string,string> msg, CancellationToken ct); } }

