using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoyalty.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndStatusToEarningRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "earning_rules");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "earning_rules",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EventKey",
                table: "earning_rules",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "earning_rules",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "earning_rules");

            migrationBuilder.DropColumn(
                name: "EventKey",
                table: "earning_rules");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "earning_rules");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "earning_rules",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
