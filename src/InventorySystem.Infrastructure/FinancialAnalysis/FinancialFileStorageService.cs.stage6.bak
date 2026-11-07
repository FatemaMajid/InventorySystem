using InventorySystem.Application.Common.Interfaces;

namespace InventorySystem.Infrastructure.FinancialAnalysis;

public class FinancialFileStorageService
    : IFinancialFileStorageService
{
    private readonly string _rootPath;

    public FinancialFileStorageService()
    {
        _rootPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "FinancialUploads");
    }

    public async Task<string> SaveAsync(
        byte[] content,
        string storedFileName,
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var directory = Path.Combine(
            _rootPath,
            year.ToString(),
            month.ToString("00"));

        Directory.CreateDirectory(directory);

        var fullPath = Path.Combine(
            directory,
            storedFileName);

        await File.WriteAllBytesAsync(
            fullPath,
            content,
            cancellationToken);

        return fullPath;
    }

    public Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(storagePath) &&
            File.Exists(storagePath))
        {
            File.Delete(storagePath);
        }

        return Task.CompletedTask;
    }
}
