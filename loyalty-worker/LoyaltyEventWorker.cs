using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

public class LoyaltyEventWorker : BackgroundService {
    private readonly IEventBus _bus;
    private readonly IServiceProvider _sp;
    private readonly ILogger<LoyaltyEventWorker> _logger;

    public LoyaltyEventWorker(IEventBus bus, IServiceProvider sp, ILogger<LoyaltyEventWorker> logger) {
        _bus = bus;
        _sp = sp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Loyalty worker running (.NET 9)");

        await _bus.ConsumeAsync(async (key, message) => {
            try {
                var ev = JObject.Parse(message);
                if (ev["type"]!.ToString() == "transaction.created")
                    await ProcessTransaction(ev);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error handling message");
            }
        }, stoppingToken);
    }

    private async Task ProcessTransaction(JObject evt) {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkerDbContext>();
        var engine = scope.ServiceProvider.GetRequiredService<IRuleEngine>();
        var points = scope.ServiceProvider.GetRequiredService<IPointsService>();
        var snapProvider = scope.ServiceProvider.GetRequiredService<ISnapshotProvider>();

        var p = evt["payload"]!;
        var userId = p["user_id"]!.ToString();
        var gross = p["gross_value"]!.Value<decimal>();
        var txId = evt["id"]?.ToString() ?? Guid.NewGuid().ToString();

        if (!await db.Transactions.AnyAsync(t => t.Id == txId)) {
            db.Transactions.Add(new Transaction {
                Id = txId,
                UserId = userId,
                GrossValue = gross
            });

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null) {
                user.TotalSpent += gross;
                user.OrdersCount += 1;
                user.LastPurchaseAt = DateTime.UtcNow;
            }
            await db.SaveChangesAsync();
        }

        await snapProvider.RefreshSnapshotAsync(userId);
        var snap = await snapProvider.GetSnapshotAsync(userId);

        var rules = await db.EarningRules.Where(r => r.Active).ToListAsync();
        foreach (var rule in rules) {
            if (engine.EvaluateRule(rule.ConditionJson, new JObject { ["payload"] = p }, snap!)) {
                var pts = engine.ComputePoints(rule.PointsJson, new JObject { ["payload"] = p }, snap!);
                if (pts > 0) await points.ApplyPointsAsync(userId, rule.Id, pts, txId);
            }
        }
    }
}
