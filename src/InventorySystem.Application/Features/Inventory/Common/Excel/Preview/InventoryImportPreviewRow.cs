namespace InventorySystem.Application.Common.Excel.Preview;

public class InventoryImportPreviewRow
{
    public int RowNumber { get; set; }

    public string ItemCode { get; set; } = string.Empty;
    public string ItemName1 { get; set; } = string.Empty;
    public string? ItemName2 { get; set; }

    public string? Category { get; set; }
    public string? Unit { get; set; }

    public string? Branch { get; set; }
    public string? Store { get; set; }

    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? TotalValue { get; set; }

    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}