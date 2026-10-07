using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Infrastructure.Migrations
{
    public partial class MakeFinancialPeriodFlexible : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FinancialPeriods_Year_Month",
                table: "FinancialPeriods");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "FinancialPeriods",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "FinancialPeriods",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE FinancialPeriods
                SET
                    StartDate = DATEFROMPARTS([Year], [Month], 1),
                    EndDate = EOMONTH(DATEFROMPARTS([Year], [Month], 1))
            ");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "FinancialPeriods",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "FinancialPeriods",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Year",
                table: "FinancialPeriods");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "FinancialPeriods");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_StartDate",
                table: "FinancialPeriods",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_EndDate",
                table: "FinancialPeriods",
                column: "EndDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FinancialPeriods_StartDate",
                table: "FinancialPeriods");

            migrationBuilder.DropIndex(
                name: "IX_FinancialPeriods_EndDate",
                table: "FinancialPeriods");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "FinancialPeriods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "FinancialPeriods",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE FinancialPeriods
                SET
                    [Year] = YEAR(StartDate),
                    [Month] = MONTH(StartDate)
            ");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "FinancialPeriods",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Month",
                table: "FinancialPeriods",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "FinancialPeriods");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "FinancialPeriods");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriods_Year_Month",
                table: "FinancialPeriods",
                columns: new[] { "Year", "Month" });
        }
    }
}
