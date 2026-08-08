namespace InventorySystem.Application.DTOs.Branch;

public class CreateBranchDto
{
    public string BranchCode { get; set; } = string.Empty;

    public string BranchNameArabic { get; set; } = string.Empty;

    public string? BranchNameEnglish { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

   // public string? ManagerName { get; set; }

    public bool IsActive { get; set; } = true;
}