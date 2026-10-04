namespace InventorySystem.Application.Features.Inventory.InventorySessions.Import.Preview;

public class InventoryImportPreviewRequest
{
    public Stream FileStream { get; set; } = Stream.Null;

    public string? FileName { get; set; }
}