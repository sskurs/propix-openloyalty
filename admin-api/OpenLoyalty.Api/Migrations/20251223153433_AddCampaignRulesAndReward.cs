using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoyalty.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignRulesAndReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "activity_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Variant = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_logs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_logs");

            migrationBuilder.DropColumn(
                name: "RewardJson",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "RulesJson",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "campaigns");
        }
    }
}
