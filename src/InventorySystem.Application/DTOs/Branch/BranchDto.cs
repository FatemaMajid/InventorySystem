namespace InventorySystem.Application.DTOs.Branch;

public class BranchDto
{
    public int Id { get; set; }

    public string BranchCode { get; set; } = string.Empty;

    public string BranchNameArabic { get; set; } = string.Empty;

    public string? BranchNameEnglish { get; set; }

    public bool IsActive { get; set; }
}