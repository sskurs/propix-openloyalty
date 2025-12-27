using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoyalty.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorCampaignSchemaRelational : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudienceJson",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "RewardJson",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "RulesJson",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "campaigns");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                table: "campaigns",
                newName: "StartAt");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "campaigns",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "campaigns",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndAt",
                table: "campaigns",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsStackable",
                table: "campaigns",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxPerCustomer",
                table: "campaigns",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxTotalRewards",
                table: "campaigns",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "campaigns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "campaign_conditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConditionType = table.Column<string>(type: "text", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_conditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_campaign_conditions_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign_rewards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    RewardType = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_rewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_campaign_rewards_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign_usage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_usage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_campaign_usage_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaign_conditions_CampaignId",
                table: "campaign_conditions",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_rewards_CampaignId",
                table: "campaign_rewards",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_usage_CampaignId",
                table: "campaign_usage",
                column: "CampaignId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_conditions");

            migrationBuilder.DropTable(
                name: "campaign_rewards");

            migrationBuilder.DropTable(
                name: "campaign_usage");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "EndAt",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "IsStackable",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "MaxPerCustomer",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "MaxTotalRewards",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "campaigns");

            migrationBuilder.RenameColumn(
                name: "StartAt",
                table: "campaigns",
                newName: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "campaigns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<string>(
                name: "AudienceJson",
                table: "campaigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RewardJson",
                table: "campaigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RulesJson",
                table: "campaigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "campaigns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "campaigns",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
