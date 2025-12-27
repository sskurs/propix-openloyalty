using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenLoyalty.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "campaigns",
                type: "bytea",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "campaigns");
        }
    }
}
