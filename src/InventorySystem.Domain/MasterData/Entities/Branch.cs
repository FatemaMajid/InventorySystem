using InventorySystem.Domain.Common;
namespace InventorySystem.Domain.Entities;
public class Branch : BaseAuditableEntity
{
    public string BranchCode { get; set; } = string.Empty;
    public string BranchNameArabic { get; set; } = string.Empty;
    public string? BranchNameEnglish { get; set; } 
    public string? Address { get; set; } 
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    //Navigation Properties
    public ICollection<Store> Stores { get; set; } = new List<Store>();
    
}