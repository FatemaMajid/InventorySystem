namespace InventorySystem.Application.Common.Interfaces;

public interface IFinancialFileStorageService
{
    Task<string> SaveAsync(
        byte[] content,
        string storedFileName,
        int year,
        int month,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken);
}
