using StackExchange.Redis;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class RedisSnapshotProvider : ISnapshotProvider {
    private readonly ConnectionMultiplexer _redis;
    private readonly WorkerDbContext _db;

    public RedisSnapshotProvider(IConfiguration cfg, WorkerDbContext db) {
        _redis = ConnectionMultiplexer.Connect(cfg["Redis:Connection"]);
        _db = db;
    }

    private IDatabase Db => _redis.GetDatabase();

    public async Task<UserSnapshot?> GetSnapshotAsync(string userId) {
        var raw = await Db.StringGetAsync($"snapshot:{userId}");
        if (!raw.IsNullOrEmpty) 
            return JsonConvert.DeserializeObject<UserSnapshot>(raw!);

        await RefreshSnapshotAsync(userId);
        raw = await Db.StringGetAsync($"snapshot:{userId}");
        return raw.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<UserSnapshot>(raw!);
    }

    public async Task RefreshSnapshotAsync(string userId) {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return;

        var snap = new UserSnapshot(userId, user.TotalSpent, user.OrdersCount, user.LastPurchaseAt, user.Email ?? "");
        await Db.StringSetAsync($"snapshot:{userId}", JsonConvert.SerializeObject(snap));
    }
}
