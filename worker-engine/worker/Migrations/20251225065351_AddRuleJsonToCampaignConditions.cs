using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Worker.Migrations
{
    /// <inheritdoc />
    public partial class AddRuleJsonToCampaignConditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberCoupons_Coupons_CouponId",
                table: "MemberCoupons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Outbox",
                table: "Outbox");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionEvents",
                table: "TransactionEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberCoupons",
                table: "MemberCoupons");

            migrationBuilder.DropIndex(
                name: "IX_MemberCoupons_CouponId",
                table: "MemberCoupons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EarningRules",
                table: "EarningRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Coupons",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "MemberCoupons");

            migrationBuilder.DropColumn(
                name: "UsedCount",
                table: "MemberCoupons");

            migrationBuilder.RenameTable(
                name: "Outbox",
                newName: "outbox");

            migrationBuilder.RenameTable(
                name: "TransactionEvents",
                newName: "transaction_events");

            migrationBuilder.RenameTable(
                name: "MemberCoupons",
                newName: "member_coupons");

            migrationBuilder.RenameTable(
                name: "EarningRules",
                newName: "worker_earning_rules");

            migrationBuilder.RenameTable(
                name: "Coupons",
                newName: "worker_coupons");

            migrationBuilder.RenameColumn(
                name: "LastUsedAt",
                table: "member_coupons",
                newName: "RedeemedAt");

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "member_coupons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_outbox",
                table: "outbox",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transaction_events",
                table: "transaction_events",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_member_coupons",
                table: "member_coupons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_worker_earning_rules",
                table: "worker_earning_rules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_worker_coupons",
                table: "worker_coupons",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "campaign_conditions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConditionJson = table.Column<string>(type: "jsonb", nullable: false),
                    RuleJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_conditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "campaign_executions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<string>(type: "text", nullable: false),
                    TransactionId = table.Column<string>(type: "text", nullable: false),
                    RewardType = table.Column<string>(type: "text", nullable: false),
                    ExecutionResultJson = table.Column<string>(type: "jsonb", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_executions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "campaign_rewards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    RewardJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_rewards", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_conditions");

            migrationBuilder.DropTable(
                name: "campaign_executions");

            migrationBuilder.DropTable(
                name: "campaign_rewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_outbox",
                table: "outbox");

            migrationBuilder.DropPrimaryKey(
                name: "PK_worker_earning_rules",
                table: "worker_earning_rules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_worker_coupons",
                table: "worker_coupons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transaction_events",
                table: "transaction_events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_member_coupons",
                table: "member_coupons");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "member_coupons");

            migrationBuilder.RenameTable(
                name: "outbox",
                newName: "Outbox");

            migrationBuilder.RenameTable(
                name: "worker_earning_rules",
                newName: "EarningRules");

            migrationBuilder.RenameTable(
                name: "worker_coupons",
                newName: "Coupons");

            migrationBuilder.RenameTable(
                name: "transaction_events",
                newName: "TransactionEvents");

            migrationBuilder.RenameTable(
                name: "member_coupons",
                newName: "MemberCoupons");

            migrationBuilder.RenameColumn(
                name: "RedeemedAt",
                table: "MemberCoupons",
                newName: "LastUsedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "CouponId",
                table: "MemberCoupons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "UsedCount",
                table: "MemberCoupons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Outbox",
                table: "Outbox",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EarningRules",
                table: "EarningRules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Coupons",
                table: "Coupons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionEvents",
                table: "TransactionEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberCoupons",
                table: "MemberCoupons",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCoupons_CouponId",
                table: "MemberCoupons",
                column: "CouponId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberCoupons_Coupons_CouponId",
                table: "MemberCoupons",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
