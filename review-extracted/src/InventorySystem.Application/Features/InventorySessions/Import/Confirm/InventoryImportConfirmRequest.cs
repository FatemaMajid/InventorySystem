using InventorySystem.Domain.Enums;

namespace InventorySystem.Application.Features.InventorySessions.Import.Confirm;

public class InventoryImportConfirmRequest
{
    public Stream BeforeFileStream { get; set; } = Stream.Null;
    public string? BeforeFileName { get; set; }

    public Stream AfterFileStream { get; set; } = Stream.Null;
    public string? AfterFileName { get; set; }

    public InventoryType InventoryType { get; set; }
}