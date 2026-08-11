namespace InventorySystem.Application.Common.Excel.Validation;

public class ExcelValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<ExcelValidationError> Errors { get; } = new();

    public int ValidRows { get; set; }

    public int TotalRows { get; set; }
}