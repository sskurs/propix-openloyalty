using Microsoft.EntityFrameworkCore;
namespace Worker.Data {
  public class WorkerDbContext : DbContext {
    public WorkerDbContext(DbContextOptions<WorkerDbContext> opts): base(opts){}
    public DbSet<Worker.Models.EarningRule> EarningRules => Set<Worker.Models.EarningRule>();
    public DbSet<Worker.Models.TransactionEvent> TransactionEvents => Set<Worker.Models.TransactionEvent>();
    public DbSet<Worker.Models.Campaign> Campaigns => Set<Worker.Models.Campaign>();
    public DbSet<Worker.Models.OutboxMessage> Outbox => Set<Worker.Models.OutboxMessage>();
    public DbSet<Worker.Models.Coupon> Coupons => Set<Worker.Models.Coupon>();
    public DbSet<Worker.Models.MemberCoupon> MemberCoupons => Set<Worker.Models.MemberCoupon>();
    
    public DbSet<Worker.Models.CampaignConditionEntity> CampaignConditions => Set<Worker.Models.CampaignConditionEntity>();
    public DbSet<Worker.Models.CampaignRewardEntity> CampaignRewards => Set<Worker.Models.CampaignRewardEntity>();
    public DbSet<Worker.Models.CampaignExecutionEntity> CampaignExecutions => Set<Worker.Models.CampaignExecutionEntity>();
    
    // Campaign Completion Entities
    public DbSet<Worker.Models.MemberPoints> MemberPoints => Set<Worker.Models.MemberPoints>();
    public DbSet<Worker.Models.PointsTransaction> PointsTransactions => Set<Worker.Models.PointsTransaction>();
    public DbSet<Worker.Models.CampaignEnrollment> CampaignEnrollments => Set<Worker.Models.CampaignEnrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Worker.Models.EarningRule>().ToTable("worker_earning_rules");
        modelBuilder.Entity<Worker.Models.TransactionEvent>().ToTable("transaction_events");
        modelBuilder.Entity<Worker.Models.OutboxMessage>().ToTable("outbox");
        modelBuilder.Entity<Worker.Models.Coupon>().ToTable("worker_coupons");
        modelBuilder.Entity<Worker.Models.Campaign>().ToTable("campaigns");
        modelBuilder.Entity<Worker.Models.MemberCoupon>().ToTable("member_coupons");
        
        modelBuilder.Entity<Worker.Models.CampaignConditionEntity>().ToTable("campaign_conditions");
        modelBuilder.Entity<Worker.Models.CampaignRewardEntity>().ToTable("campaign_rewards");
        modelBuilder.Entity<Worker.Models.CampaignExecutionEntity>().ToTable("campaign_executions");
        
        // Campaign Completion Tables
        modelBuilder.Entity<Worker.Models.MemberPoints>().ToTable("member_points");
        modelBuilder.Entity<Worker.Models.PointsTransaction>().ToTable("points_transactions");
        modelBuilder.Entity<Worker.Models.CampaignEnrollment>().ToTable("campaign_enrollments");
    }
  }
}

