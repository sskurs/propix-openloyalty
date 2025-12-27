using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoyalty.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignAudience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudienceJson",
                table: "campaigns",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudienceJson",
                table: "campaigns");
        }
    }
}
