using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Worker.Migrations
{
    /// <inheritdoc />
    public partial class RefactorWorkerCampaignSchemaRelational : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
