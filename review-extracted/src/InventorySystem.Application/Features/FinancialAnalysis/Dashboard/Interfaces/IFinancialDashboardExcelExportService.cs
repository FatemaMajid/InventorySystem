using InventorySystem.Application.DTOs.FinancialAnalysis.DashboardAggregate;

namespace InventorySystem.Application.Features.FinancialAnalysis.Dashboard.Interfaces;

public interface IFinancialDashboardExcelExportService
{
    Task<byte[]> ExportAsync(
        FinancialAnalysisDashboardDto dashboard,
        string language,
        CancellationToken cancellationToken);
}
