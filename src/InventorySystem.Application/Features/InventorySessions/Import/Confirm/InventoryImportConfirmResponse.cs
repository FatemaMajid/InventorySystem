namespace InventorySystem.Application.Features.InventorySessions.Import.Confirm;

public class InventoryImportConfirmResponse
{
    public bool IsSuccess { get; set; }

    public int InventorySessionId { get; set; }

    public string SessionNumber { get; set; } = string.Empty;

    public int TotalItems { get; set; }

    public int CreatedItems { get; set; }

    public int UpdatedItems { get; set; }

    public int CreatedCategories { get; set; } 

    public int CreatedLocations { get; set; }

    public int UpdatedPrices { get; set; }

    public string Message { get; set; } = string.Empty;
}