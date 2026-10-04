using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialAnalysisModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancialUploads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialPeriodId = table.Column<int>(type: "int", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StoragePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidationMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialUploads_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialUploads_FinancialPeriods_FinancialPeriodId",
                        column: x => x.FinancialPeriodId,
                        principalTable: "FinancialPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancialAccountRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialUploadId = table.Column<int>(type: "int", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Assets = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Liabilities = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RowOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAccountRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialAccountRows_FinancialUploads_FinancialUploadId",
                        column: x => x.FinancialUploadId,
                        principalTable: "FinancialUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettlementRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialUploadId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EntryNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsMarked = table.Column<bool>(type: "bit", nullable: false),
                    IsMatched = table.Column<bool>(type: "bit", nullable: false),
                    RowOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettlementRows_FinancialUploads_FinancialUploadId",
                        column: x => x.FinancialUploadId,
                        principalTable: "FinancialUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettlementSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialUploadId = table.Column<int>(type: "int", nullable: false),
                    PeriodDebit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PeriodCredit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PeriodBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettlementSummaries_FinancialUploads_FinancialUploadId",
                        column: x => x.FinancialUploadId,
                        principalTable: "FinancialUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialUploadId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RowOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferRecords_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRecords_FinancialUploads_FinancialUploadId",
                        column: x => x.FinancialUploadId,
                        principalTable: "FinancialUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAccountRows_FinancialUploadId_AccountCode",
                table: "FinancialAccountRows",
                columns: new[] { "FinancialUploadId", "AccountCode" });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAccountRows_FinancialUploadId_RowOrder",
                table: "FinancialAccountRows",
                columns: new[] { "FinancialUploadId", "RowOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_Year_Month",
                table: "FinancialPeriods",
                columns: new[] { "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialUploads_BranchId",
                table: "FinancialUploads",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialUploads_FileHash",
                table: "FinancialUploads",
                column: "FileHash");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialUploads_FinancialPeriodId_FileType_BranchId_Gender",
                table: "FinancialUploads",
                columns: new[] { "FinancialPeriodId", "FileType", "BranchId", "Gender" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialUploads_FinancialPeriodId_FileType_BranchId_Gender_IsActive",
                table: "FinancialUploads",
                columns: new[] { "FinancialPeriodId", "FileType", "BranchId", "Gender", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_SettlementRows_FinancialUploadId_RowOrder",
                table: "SettlementRows",
                columns: new[] { "FinancialUploadId", "RowOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_SettlementRows_FinancialUploadId_TransactionDate",
                table: "SettlementRows",
                columns: new[] { "FinancialUploadId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SettlementSummaries_FinancialUploadId",
                table: "SettlementSummaries",
                column: "FinancialUploadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_BranchId",
                table: "TransferRecords",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_FinancialUploadId_BranchId",
                table: "TransferRecords",
                columns: new[] { "FinancialUploadId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_FinancialUploadId_RowOrder",
                table: "TransferRecords",
                columns: new[] { "FinancialUploadId", "RowOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialAccountRows");

            migrationBuilder.DropTable(
                name: "SettlementRows");

            migrationBuilder.DropTable(
                name: "SettlementSummaries");

            migrationBuilder.DropTable(
                name: "TransferRecords");

            migrationBuilder.DropTable(
                name: "FinancialUploads");

            migrationBuilder.DropTable(
                name: "FinancialPeriods");
        }
    }
}
