using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoyalty.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRuleGroupJsonToCampaignConditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'member_coupon') THEN
        IF EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name = 'FK_member_coupon_coupon_CouponId') THEN
            ALTER TABLE member_coupon DROP CONSTRAINT ""FK_member_coupon_coupon_CouponId"";
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name = 'FK_member_coupon_member_MemberId') THEN
            ALTER TABLE member_coupon DROP CONSTRAINT ""FK_member_coupon_member_MemberId"";
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name = 'PK_member_coupon') THEN
            ALTER TABLE member_coupon DROP CONSTRAINT ""PK_member_coupon"";
        END IF;
        
        DROP INDEX IF EXISTS ""IX_member_coupon_CouponId"";
        DROP INDEX IF EXISTS ""IX_member_coupon_MemberId"";
        
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'member_coupon' AND column_name = 'CouponId') THEN
            ALTER TABLE member_coupon DROP COLUMN ""CouponId"";
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'member_coupon' AND column_name = 'UsedCount') THEN
            ALTER TABLE member_coupon DROP COLUMN ""UsedCount"";
        END IF;

        ALTER TABLE member_coupon RENAME TO member_coupons;
    END IF;
END $$;
");

            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'member_coupons' AND column_name = 'LastUsedAt') THEN
        ALTER TABLE member_coupons RENAME COLUMN ""LastUsedAt"" TO ""RedeemedAt"";
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'member_coupons' AND column_name = 'CouponCode') THEN
        ALTER TABLE member_coupons ADD ""CouponCode"" text NOT NULL DEFAULT '';
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'member_coupons' AND column_name = 'MemberId') THEN
        ALTER TABLE member_coupons ALTER COLUMN ""MemberId"" TYPE text;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE table_name = 'member_coupons' AND constraint_name = 'PK_member_coupons') THEN
        ALTER TABLE member_coupons ADD CONSTRAINT ""PK_member_coupons"" PRIMARY KEY (""Id"");
    END IF;
END $$;
");

            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS campaign_executions (
    ""Id"" uuid NOT NULL,
    ""RuleId"" text NOT NULL,
    ""TransactionId"" text,
    ""MemberId"" uuid NOT NULL,
    ""ExecutedAt"" timestamp with time zone NOT NULL,
    ""RewardType"" text,
    ""RewardValue"" numeric,
    CONSTRAINT ""PK_campaign_executions"" PRIMARY KEY (""Id"")
);
");

            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'campaign_conditions' AND column_name = 'RuleGroupJson') THEN
        ALTER TABLE campaign_conditions ADD ""RuleGroupJson"" jsonb;
    END IF;
END $$;
");

            migrationBuilder.UpdateData(
                table: "member",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "ExternalId",
                value: "CUST-1001");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_executions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_member_coupons",
                table: "member_coupons");

            migrationBuilder.DropColumn(
                name: "RuleGroupJson",
                table: "campaign_conditions");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "member_coupons");

            migrationBuilder.RenameTable(
                name: "member_coupons",
                newName: "member_coupon");

            migrationBuilder.RenameColumn(
                name: "RedeemedAt",
                table: "member_coupon",
                newName: "LastUsedAt");

            migrationBuilder.AlterColumn<Guid>(
                name: "MemberId",
                table: "member_coupon",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "CouponId",
                table: "member_coupon",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "UsedCount",
                table: "member_coupon",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_member_coupon",
                table: "member_coupon",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "member",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "ExternalId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_member_coupon_CouponId",
                table: "member_coupon",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_member_coupon_MemberId",
                table: "member_coupon",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_member_coupon_coupon_CouponId",
                table: "member_coupon",
                column: "CouponId",
                principalTable: "coupon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_member_coupon_member_MemberId",
                table: "member_coupon",
                column: "MemberId",
                principalTable: "member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
