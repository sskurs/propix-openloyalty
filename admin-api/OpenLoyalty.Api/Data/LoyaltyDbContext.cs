using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Models;
using System;

namespace OpenLoyalty.Api.Data
{
    public class LoyaltyDbContext : DbContext
    {
        public LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<Tier> Tiers { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<WalletLog> WalletLogs { get; set; }
        public DbSet<MemberTierHistory> MemberTierHistories { get; set; }
        public DbSet<RewardRedemption> RewardRedemptions { get; set; }
        public DbSet<MemberCoupon> MemberCoupons { get; set; }
        public DbSet<CampaignLog> CampaignLogs { get; set; }
        public DbSet<CampaignCondition> CampaignConditions { get; set; }
        public DbSet<CampaignReward> CampaignRewards { get; set; }
        public DbSet<CampaignUsage> CampaignUsages { get; set; }
        public DbSet<MemberAchievement> MemberAchievements { get; set; }
        public DbSet<TimelineEvent> TimelineEvents { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<EarningRule> EarningRules { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
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
                e.Property(t => t.Name).IsRequired();
            });

            // Member Configuration
            modelBuilder.Entity<Member>(e =>
            {
                e.ToTable("member");
                e.HasKey(m => m.Id);
                e.HasIndex(m => m.Email).IsUnique();
                e.HasIndex(m => m.Phone).IsUnique();
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

            // Transaction Configuration
            modelBuilder.Entity<Transaction>(e =>
            {
                e.ToTable("transaction");
                e.HasOne(t => t.Member)
                 .WithMany(m => m.Transactions)
                 .HasForeignKey(t => t.MemberId);
            });

            // Simple table name mappings
            modelBuilder.Entity<EarningRule>().ToTable("earning_rules");
            modelBuilder.Entity<Campaign>().ToTable("campaigns");
            modelBuilder.Entity<CampaignCondition>().ToTable("campaign_conditions");
            modelBuilder.Entity<CampaignReward>().ToTable("campaign_rewards");
            modelBuilder.Entity<CampaignUsage>().ToTable("campaign_usage");
            modelBuilder.Entity<OutboxMessage>().ToTable("outbox_messages");
            modelBuilder.Entity<Store>().ToTable("store");
            modelBuilder.Entity<Channel>().ToTable("channel");
            modelBuilder.Entity<Reward>().ToTable("reward");
            modelBuilder.Entity<Coupon>().ToTable("coupon");
            modelBuilder.Entity<Achievement>().ToTable("achievement");
            modelBuilder.Entity<WalletLog>().ToTable("wallet_log");
            modelBuilder.Entity<MemberTierHistory>().ToTable("member_tier_history");
            modelBuilder.Entity<RewardRedemption>().ToTable("reward_redemption");
            modelBuilder.Entity<MemberCoupon>(e =>
            {
                e.ToTable("member_coupons");
                e.HasKey(mc => mc.Id);
                e.Property(mc => mc.MemberId).IsRequired();
                e.Property(mc => mc.CouponCode).IsRequired();
            });
            modelBuilder.Entity<CampaignLog>().ToTable("campaign_log");
            modelBuilder.Entity<MemberAchievement>().ToTable("member_achievement");
            modelBuilder.Entity<TimelineEvent>().ToTable("timeline_event");
            modelBuilder.Entity<ActivityLog>().ToTable("activity_logs");
            modelBuilder.Entity<CampaignExecution>().ToTable("campaign_executions");

            // --- Seed Data ---
            var tierId = new Guid("11111111-1111-1111-1111-111111111111");
            var fixedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Tier>().HasData(
                new Tier { Id = tierId, Name = "Bronze", Code = "BRONZE", CreatedAt = fixedDate, UpdatedAt = fixedDate, Multiplier = 1.0m, ThresholdValue = 0 }
            );

            var memberId = new Guid("55555555-5555-5555-5555-555555555555");
            modelBuilder.Entity<Member>().HasData(
                new 
                {
                    Id = memberId, 
                    FirstName = "Sample", 
                    LastName = "User", 
                    Email = "user@example.com", 
                    JoinDate = fixedDate, 
                    Status = "active", 
                    TierId = tierId, 
                    CreatedAt = fixedDate, 
                    UpdatedAt = fixedDate,
                    ExternalId = "CUST-1001",
                    Phone = (string?)null,
                    DateOfBirth = (DateTime?)null,
                    Gender = (string?)null
                }
            );
        }
    }
}
