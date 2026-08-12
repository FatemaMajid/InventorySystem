using InventorySystem.Application.Common.Excel.Preview;

namespace InventorySystem.Application.Features.InventorySessions.Import.Preview;

public class InventoryImportPreviewResponse
{
    public bool IsValid { get; set; }

    public InventoryImportPreview Preview { get; set; } =
        new();
}