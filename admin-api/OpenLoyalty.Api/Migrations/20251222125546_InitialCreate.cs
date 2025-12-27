using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoyalty.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "achievement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Criteria = table.Column<string>(type: "text", nullable: false),
                    RewardRules = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "channel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "coupon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    MaxUses = table.Column<int>(type: "integer", nullable: true),
                    MaxUsesPerMember = table.Column<int>(type: "integer", nullable: true),
                    MinOrderValue = table.Column<decimal>(type: "numeric", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "earning_rules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    EventKey = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    ActivateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CronExpression = table.Column<string>(type: "text", nullable: true),
                    ConditionJson = table.Column<string>(type: "text", nullable: true),
                    PointsJson = table.Column<string>(type: "text", nullable: true),
                    LimitsJson = table.Column<string>(type: "text", nullable: true),
                    TimeWindowJson = table.Column<string>(type: "text", nullable: true),
                    SegmentsJson = table.Column<string>(type: "text", nullable: true),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_earning_rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Topic = table.Column<string>(type: "text", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: true),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Tries = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tier",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Multiplier = table.Column<decimal>(type: "numeric", nullable: false),
                    ThresholdValue = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "member",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    JoinDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TierId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_member_tier_TierId",
                        column: x => x.TierId,
                        principalTable: "tier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reward",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    StockQty = table.Column<int>(type: "integer", nullable: true),
                    CostPoints = table.Column<decimal>(type: "numeric", nullable: true),
                    CostCashback = table.Column<decimal>(type: "numeric", nullable: true),
                    MinTierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reward_tier_MinTierId",
                        column: x => x.MinTierId,
                        principalTable: "tier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "member_achievement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievementId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_achievement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_member_achievement_achievement_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "achievement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_member_achievement_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member_coupon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    CouponId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsedCount = table.Column<int>(type: "integer", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_coupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_member_coupon_coupon_CouponId",
                        column: x => x.CouponId,
                        principalTable: "coupon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_member_coupon_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member_tier_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    TierId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ToDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ChangeReason = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_tier_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_member_tier_history_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_member_tier_history_tier_TierId",
                        column: x => x.TierId,
                        principalTable: "tier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transaction_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wallet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wallet_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign_log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EventType = table.Column<string>(type: "text", nullable: true),
                    RewardType = table.Column<string>(type: "text", nullable: true),
                    RewardValue = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_campaign_log_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_campaign_log_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_campaign_log_transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transaction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "reward_redemption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RewardId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PointsSpent = table.Column<decimal>(type: "numeric", nullable: true),
                    CashbackSpent = table.Column<decimal>(type: "numeric", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reward_redemption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reward_redemption_channel_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "channel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_reward_redemption_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reward_redemption_reward_RewardId",
                        column: x => x.RewardId,
                        principalTable: "reward",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reward_redemption_transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transaction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "wallet_log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Direction = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "numeric", nullable: false),
                    ReasonCode = table.Column<string>(type: "text", nullable: false),
                    WalletType = table.Column<string>(type: "text", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet_log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wallet_log_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_wallet_log_transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transaction",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_wallet_log_wallet_WalletId",
                        column: x => x.WalletId,
                        principalTable: "wallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "timeline_event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    WalletLogId = table.Column<Guid>(type: "uuid", nullable: true),
                    TierFromId = table.Column<Guid>(type: "uuid", nullable: true),
                    TierToId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeline_event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_timeline_event_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_timeline_event_tier_TierFromId",
                        column: x => x.TierFromId,
                        principalTable: "tier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_timeline_event_tier_TierToId",
                        column: x => x.TierToId,
                        principalTable: "tier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_timeline_event_transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transaction",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_timeline_event_wallet_log_WalletLogId",
                        column: x => x.WalletLogId,
                        principalTable: "wallet_log",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "tier",
                columns: new[] { "Id", "Code", "CreatedAt", "Multiplier", "Name", "ThresholdValue", "UpdatedAt" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "BRONZE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1.0m, "Bronze", 0m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "member",
                columns: new[] { "Id", "CreatedAt", "DateOfBirth", "Email", "ExternalId", "FirstName", "Gender", "JoinDate", "LastName", "Phone", "Status", "TierId", "UpdatedAt" },
                values: new object[] { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "user@example.com", null, "Sample", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "User", null, "active", new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_campaign_log_CampaignId",
                table: "campaign_log",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_log_MemberId",
                table: "campaign_log",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_log_TransactionId",
                table: "campaign_log",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_member_Email",
                table: "member",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_Phone",
                table: "member",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_TierId",
                table: "member",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_member_achievement_AchievementId",
                table: "member_achievement",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_member_achievement_MemberId",
                table: "member_achievement",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_member_coupon_CouponId",
                table: "member_coupon",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_member_coupon_MemberId",
                table: "member_coupon",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_member_tier_history_MemberId",
                table: "member_tier_history",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_member_tier_history_TierId",
                table: "member_tier_history",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_reward_MinTierId",
                table: "reward",
                column: "MinTierId");

            migrationBuilder.CreateIndex(
                name: "IX_reward_redemption_ChannelId",
                table: "reward_redemption",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_reward_redemption_MemberId",
                table: "reward_redemption",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_reward_redemption_RewardId",
                table: "reward_redemption",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_reward_redemption_TransactionId",
                table: "reward_redemption",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_tier_Code",
                table: "tier",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_timeline_event_MemberId",
                table: "timeline_event",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_timeline_event_TierFromId",
                table: "timeline_event",
                column: "TierFromId");

            migrationBuilder.CreateIndex(
                name: "IX_timeline_event_TierToId",
                table: "timeline_event",
                column: "TierToId");

            migrationBuilder.CreateIndex(
                name: "IX_timeline_event_TransactionId",
                table: "timeline_event",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_timeline_event_WalletLogId",
                table: "timeline_event",
                column: "WalletLogId");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_MemberId",
                table: "transaction",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_wallet_MemberId_Type",
                table: "wallet",
                columns: new[] { "MemberId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wallet_log_MemberId",
                table: "wallet_log",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_wallet_log_TransactionId",
                table: "wallet_log",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_wallet_log_WalletId",
                table: "wallet_log",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_log");

            migrationBuilder.DropTable(
                name: "earning_rules");

            migrationBuilder.DropTable(
                name: "member_achievement");

            migrationBuilder.DropTable(
                name: "member_coupon");

            migrationBuilder.DropTable(
                name: "member_tier_history");

            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "reward_redemption");

            migrationBuilder.DropTable(
                name: "store");

            migrationBuilder.DropTable(
                name: "timeline_event");

            migrationBuilder.DropTable(
                name: "campaigns");

            migrationBuilder.DropTable(
                name: "achievement");

            migrationBuilder.DropTable(
                name: "coupon");

            migrationBuilder.DropTable(
                name: "channel");

            migrationBuilder.DropTable(
                name: "reward");

            migrationBuilder.DropTable(
                name: "wallet_log");

            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "wallet");

            migrationBuilder.DropTable(
                name: "member");

            migrationBuilder.DropTable(
                name: "tier");
        }
    }
}
