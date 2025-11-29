using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Models;
using System;

namespace OpenLoyalty.Api.Data
{
    public class LoyaltyDbContext : DbContext
    {
        public LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> options) : base(options) { }

        // Define all DbSets for the new schema
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
        public DbSet<Segment> Segments { get; set; } // Added Segment DbSet
        public DbSet<WalletLog> WalletLogs { get; set; }
        public DbSet<MemberTierHistory> MemberTierHistories { get; set; }
        public DbSet<RewardRedemption> RewardRedemptions { get; set; }
        public DbSet<MemberCoupon> MemberCoupons { get; set; }
        public DbSet<CampaignLog> CampaignLogs { get; set; }
        public DbSet<MemberAchievement> MemberAchievements { get; set; }
        public DbSet<TimelineEvent> TimelineEvents { get; set; }

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
                e.HasOne(m => m.Tier).WithMany(t => t.Members).HasForeignKey(m => m.TierId);
            });

            // Wallet Configuration
            modelBuilder.Entity<Wallet>(e =>
            {
                e.ToTable("wallet");
                e.HasKey(w => w.Id);
                e.HasIndex(w => new { w.MemberId, w.Type }).IsUnique();
                e.HasOne(w => w.Member).WithMany(m => m.Wallets).HasForeignKey(w => w.MemberId);
            });

            // Segment Configuration
            modelBuilder.Entity<Segment>(e => e.ToTable("segments"));

            // Other entity configurations
            modelBuilder.Entity<Store>(e => e.ToTable("store"));
            modelBuilder.Entity<Channel>(e => e.ToTable("channel"));
            modelBuilder.Entity<Reward>(e => e.ToTable("reward"));
            modelBuilder.Entity<Coupon>(e => e.ToTable("coupon"));
            modelBuilder.Entity<Campaign>(e => e.ToTable("campaign"));
            modelBuilder.Entity<Achievement>(e => e.ToTable("achievement"));
            modelBuilder.Entity<WalletLog>(e => e.ToTable("wallet_log"));
            modelBuilder.Entity<MemberTierHistory>(e => e.ToTable("member_tier_history"));
            modelBuilder.Entity<RewardRedemption>(e => e.ToTable("reward_redemption"));
            modelBuilder.Entity<MemberCoupon>(e => e.ToTable("member_coupon"));
            modelBuilder.Entity<CampaignLog>(e => e.ToTable("campaign_log"));
            modelBuilder.Entity<MemberAchievement>(e => e.ToTable("member_achievement"));
            modelBuilder.Entity<TimelineEvent>(e => e.ToTable("timeline_event"));

            // Transaction Configuration
            modelBuilder.Entity<Transaction>(e =>
            {
                e.ToTable("transaction");
                e.HasOne(t => t.Member).WithMany(m => m.Transactions).HasForeignKey(t => t.MemberId);
            });

            // --- Seed Data (with fixed, deterministic values) ---
            var tierId = new Guid("11111111-1111-1111-1111-111111111111");
            var fixedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Tier>().HasData(
                new Tier { Id = tierId, Name = "Bronze", Code = "BRONZE", CreatedAt = fixedDate, UpdatedAt = fixedDate }
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
                    ExternalId = (string?)null,
                    Phone = (string?)null,
                    DateOfBirth = (DateTime?)null,
                    Gender = (string?)null
                }
            );
        }
    }
}
