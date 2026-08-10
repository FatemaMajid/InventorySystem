using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string CategoryCode { get; set; } = string.Empty;

    public string CategoryNameArabic { get; set; } = string.Empty;

    public string? CategoryNameEnglish { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<Item> Items { get; set; } = new List<Item>();
}