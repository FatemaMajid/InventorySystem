using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferAccountFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "TransferRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "TransferRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_FinancialUploadId_AccountCode",
                table: "TransferRecords",
                columns: new[] { "FinancialUploadId", "AccountCode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TransferRecords_FinancialUploadId_AccountCode",
                table: "TransferRecords");

            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "TransferRecords");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "TransferRecords");
        }
    }
}
