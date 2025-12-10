using Microsoft.EntityFrameworkCore;

public class WorkerDbContext : DbContext {
    public WorkerDbContext(DbContextOptions<WorkerDbContext> opts) : base(opts) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<EarningRule> EarningRules => Set<EarningRule>();
    public DbSet<EarningRuleUsage> EarningRuleUsages => Set<EarningRuleUsage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Transaction>().HasKey(t => t.Id);
        modelBuilder.Entity<EarningRule>().HasKey(r => r.Id);
        modelBuilder.Entity<EarningRuleUsage>().HasKey(u => u.Id);

        modelBuilder.Entity<Transaction>().HasIndex(t => t.UserId);
        modelBuilder.Entity<EarningRuleUsage>().HasIndex(u => u.UserId);
    }
}
