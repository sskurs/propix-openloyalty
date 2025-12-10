using Microsoft.EntityFrameworkCore;

public class PointsService : IPointsService {
    private readonly WorkerDbContext _db;
    private readonly ISnapshotProvider _snap;

    public PointsService(WorkerDbContext db, ISnapshotProvider snap) {
        _db = db;
        _snap = snap;
    }

    public async Task<int> ApplyPointsAsync(string userId, string ruleId, int points, string? eventId) {
        using var tx = await _db.Database.BeginTransactionAsync();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return 0;

        user.Balance += points;

        _db.EarningRuleUsages.Add(new EarningRuleUsage {
            RuleId = ruleId,
            UserId = userId,
            Points = points,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        await _snap.RefreshSnapshotAsync(userId);

        return points;
    }
}
