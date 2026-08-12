using InventorySystem.Domain.Enums;

namespace InventorySystem.Application.Features.InventorySessions.Import.Confirm;

public class InventoryImportConfirmRequest
{
    public Stream FileStream { get; set; } = Stream.Null;

    public string? FileName { get; set; }

    public InventoryType InventoryType { get; set; }
}