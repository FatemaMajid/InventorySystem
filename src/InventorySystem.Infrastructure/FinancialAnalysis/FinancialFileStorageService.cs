using InventorySystem.Application.Common.Interfaces;

namespace InventorySystem.Infrastructure.FinancialAnalysis;

public class FinancialFileStorageService : IFinancialFileStorageService
{
    private readonly string _rootPath;

    public FinancialFileStorageService(){
        _rootPath = Path.GetFullPath(
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "FinancialUploads"));
    }

    public async Task<string> SaveAsync(
        byte[] content,
        string storedFileName,
        int year,
        int month,
        CancellationToken cancellationToken){
        if (content is null || content.Length == 0)
            throw new ArgumentException("File content is required.", nameof(content));

        if (string.IsNullOrWhiteSpace(storedFileName) ||
            storedFileName != Path.GetFileName(storedFileName) ||
            storedFileName.Contains('/') ||
            storedFileName.Contains('\\') ||
            !Guid.TryParseExact(
                Path.GetFileNameWithoutExtension(storedFileName),
                "N",
                out _) ||
            !string.Equals(
                Path.GetExtension(storedFileName),
                ".xlsx",
                StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Invalid stored file name.", nameof(storedFileName));

        if (year < 1 || year > 9999 || month < 1 || month > 12)
            throw new ArgumentOutOfRangeException(nameof(year), "Invalid storage date.");

        var directory = Path.Combine(
            _rootPath,
            year.ToString("0000"),
            month.ToString("00"));

        var fullPath = Path.GetFullPath(
            Path.Combine(directory, storedFileName));

        EnsureInsideRoot(fullPath);

        Directory.CreateDirectory(directory);

        var created = false;

        try
        {
            await using (var stream = new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                FileOptions.Asynchronous))
            {
                created = true;
                await stream.WriteAsync(content, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            return fullPath;
        }
        catch
        {
            if (created)
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            throw;
        }
    }

    public Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken){
        if (string.IsNullOrWhiteSpace(storagePath))
            return Task.CompletedTask;

        if (!Path.IsPathFullyQualified(storagePath))
            throw new InvalidOperationException("Storage path must be absolute.");

        var fullPath = Path.GetFullPath(storagePath);
        EnsureInsideRoot(fullPath);

        cancellationToken.ThrowIfCancellationRequested();

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    private void EnsureInsideRoot(string fullPath){
        var rootWithSeparator =
            Path.TrimEndingDirectorySeparator(_rootPath) +
            Path.DirectorySeparatorChar;

        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        if (!fullPath.StartsWith(rootWithSeparator, comparison))
            throw new InvalidOperationException(
                "File path is outside the financial uploads directory.");
    }
}