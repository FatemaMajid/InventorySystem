namespace InventorySystem.Application.Common.Excel.Validation;

public class ExcelValidationError
{
    public int RowNumber { get; set; }

    public string? ItemCode { get; set; }

    public string MessageKey { get; set; } = string.Empty;
}