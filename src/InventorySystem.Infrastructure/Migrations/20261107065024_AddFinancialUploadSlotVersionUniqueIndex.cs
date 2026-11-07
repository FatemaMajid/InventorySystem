using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialUploadSlotVersionUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_FinancialUploads_Slot_Version",
                table: "FinancialUploads",
                columns: new[] { "FinancialPeriodId", "FileType", "BranchId", "Gender", "Version" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_FinancialUploads_Slot_Version",
                table: "FinancialUploads");
        }
    }
}
