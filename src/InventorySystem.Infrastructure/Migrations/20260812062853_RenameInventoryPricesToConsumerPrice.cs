using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameInventoryPricesToConsumerPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerPriceBefore",
                table: "InventoryDetails",
                newName: "ConsumerPriceBefore");

            migrationBuilder.RenameColumn(
                name: "CustomerPriceAfter",
                table: "InventoryDetails",
                newName: "ConsumerPriceAfter");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConsumerPriceBefore",
                table: "InventoryDetails",
                newName: "CustomerPriceBefore");

            migrationBuilder.RenameColumn(
                name: "ConsumerPriceAfter",
                table: "InventoryDetails",
                newName: "CustomerPriceAfter");
        }
    }
}
