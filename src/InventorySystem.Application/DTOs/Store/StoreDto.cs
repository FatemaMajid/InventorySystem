namespace InventorySystem.Application.DTOs.Store;

public class StoreDto
{
    public int Id { get; set; }

    public string StoreCode { get; set; } = string.Empty;

    public string StoreNameArabic { get; set; } = string.Empty;

    public string? StoreNameEnglish { get; set; }

    public string BranchCode { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}