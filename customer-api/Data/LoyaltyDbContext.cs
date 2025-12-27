using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Customer.Api.Models;

namespace OpenLoyalty.Customer.Api.Data
{
    public class LoyaltyDbContext : DbContext
    {
        public LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<Tier> Tiers { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<TimelineEvent> TimelineEvents { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<MemberCoupon> MemberCoupons { get; set; }
        public DbSet<WalletLog> WalletLogs { get; set; }
        
        // Campaign Completion Entities
        public DbSet<MemberPoints> MemberPoints { get; set; }
        public DbSet<PointsTransaction> PointsTransactions { get; set; }
        public DbSet<CampaignEnrollment> CampaignEnrollments { get; set; }
        public DbSet<CampaignExecution> CampaignExecutions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tier Configuration
            modelBuilder.Entity<Tier>(e =>
            {
                e.ToTable("tier");
                e.HasKey(t => t.Id);
                e.HasIndex(t => t.Code).IsUnique();
            });

            // Member Configuration
            modelBuilder.Entity<Member>(e =>
            {
                e.ToTable("member");
                e.HasKey(m => m.Id);
                e.HasIndex(m => m.Email).IsUnique();
                e.HasOne(m => m.Tier)
                 .WithMany(t => t.Members)
                 .HasForeignKey(m => m.TierId);
            });

            // Wallet Configuration
            modelBuilder.Entity<Wallet>(e =>
            {
                e.ToTable("wallet");
                e.HasKey(w => w.Id);
                e.HasIndex(w => new { w.MemberId, w.Type }).IsUnique();
                e.HasOne(w => w.Member)
                 .WithMany(m => m.Wallets)
                 .HasForeignKey(w => w.MemberId);
            });

            // Table name mappings to match Admin API
            modelBuilder.Entity<Campaign>().ToTable("campaigns");
            modelBuilder.Entity<OutboxMessage>().ToTable("outbox_messages");
            modelBuilder.Entity<Reward>().ToTable("reward");
            modelBuilder.Entity<TimelineEvent>().ToTable("timeline_event");
            modelBuilder.Entity<MemberCoupon>().ToTable("member_coupons");
            modelBuilder.Entity<WalletLog>().ToTable("wallet_logs");
            
            // Campaign Completion Tables
            modelBuilder.Entity<MemberPoints>().ToTable("member_points");
            modelBuilder.Entity<PointsTransaction>().ToTable("points_transactions");
            modelBuilder.Entity<CampaignEnrollment>().ToTable("campaign_enrollments");
            modelBuilder.Entity<CampaignExecution>().ToTable("campaign_executions");
        }
    }
}
