namespace InventorySystem.Application.Common.Interfaces;

public interface IDashboardExportService
{
    Task<byte[]> ExportExcelAsync(
        int sessionId,
        string language,
        CancellationToken cancellationToken);

    Task<byte[]> ExportPdfAsync(
        int sessionId,
        string language,
        CancellationToken cancellationToken);
}