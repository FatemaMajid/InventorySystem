namespace InventorySystem.Application.Features.FinancialAnalysis.Interfaces;

public interface IFinancialPeriodExportService
{
    Task<byte[]> ExportExcelAsync(
        int financialPeriodId,
        string language,
        string mode,
        int? branchId,
        CancellationToken cancellationToken);
}
