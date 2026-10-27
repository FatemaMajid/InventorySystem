namespace InventorySystem.Application.Features.InventorySessions.Import.Preview;

public class InventoryImportPreviewRequest
{
    public Stream FileStream { get; set; } = Stream.Null;

    public string? FileName { get; set; }
}