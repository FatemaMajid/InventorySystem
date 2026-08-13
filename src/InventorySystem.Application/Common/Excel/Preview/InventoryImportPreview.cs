namespace InventorySystem.Application.Common.Excel.Preview;

public class InventoryImportPreview
{
    public string? Branch { get; set; }
    public string? Store { get; set; }

    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int ErrorRows { get; set; }

    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }

    public List<InventoryImportPreviewRow> Rows { get; set; } = new();
}