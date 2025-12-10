using System.Threading.Tasks;

public interface ISnapshotProvider {
    Task<UserSnapshot?> GetSnapshotAsync(string userId);
    Task RefreshSnapshotAsync(string userId);
}

public record UserSnapshot(string UserId, decimal TotalSpent, int OrdersCount, DateTime? LastPurchaseAt, string Email);
