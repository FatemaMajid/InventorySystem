using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.Entities;

public class Unit : BaseAuditableEntity
{
    public string UnitCode { get; set; } = string.Empty;
    public string UnitNameArabic { get; set; } = string.Empty;
    public string? UnitNameEnglish { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Item> Items { get; set; } = new List<Item>();
}