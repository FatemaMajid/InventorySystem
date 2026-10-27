using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventorySessionIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InventoryDetails_InventorySessionId",
                table: "InventoryDetails",
                column: "InventorySessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryDetails_InventorySessionId",
                table: "InventoryDetails");
        }
    }
}
