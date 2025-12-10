using System;
using System.Threading;
using System.Threading.Tasks;

public interface IEventBus {
    Task ConsumeAsync(Func<string, string, Task> onMessage, CancellationToken ct);
}
