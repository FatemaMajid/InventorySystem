namespace InventorySystem.Application.Common.Interfaces;

public interface IComparisonExportService
{
    Task<byte[]> ExportExcelAsync(
        int sessionId,
        string? itemCode,
        string? itemName,
        int? categoryId,
        int? unitId,
        string? status,
        string sortBy,
        bool descending,
        string language,
        CancellationToken cancellationToken);

    Task<byte[]> ExportPdfAsync(
        int sessionId,
        string? itemCode,
        string? itemName,
        int? categoryId,
        int? unitId,
        string? status,
        string sortBy,
        bool descending,
        string language,
        CancellationToken cancellationToken);
}