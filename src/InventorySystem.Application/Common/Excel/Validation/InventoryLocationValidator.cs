using InventorySystem.Application.Common.Excel;

namespace InventorySystem.Application.Common.Excel.Validation;

public static class InventoryLocationValidator
{
    public static ExcelValidationResult Validate(
        IReadOnlyList<InventoryExcelRow> rows,
        int headerRow)
    {
        var result = new ExcelValidationResult
        {
            TotalRows = rows.Count
        };

        var branches = rows
            .Where(x => !string.IsNullOrWhiteSpace(x.Branch))
            .Select(x => x.Branch!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var stores = rows
            .Where(x => !string.IsNullOrWhiteSpace(x.Store))
            .Select(x => x.Store!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Branch validation
        if (branches.Count == 0)
        {
            result.Errors.Add(new ExcelValidationError
            {
                RowNumber = headerRow,
                MessageKey = "BranchRequired"
            });
        }
        else if (branches.Count > 1)
        {
            result.Errors.Add(new ExcelValidationError
            {
                RowNumber = headerRow,
                MessageKey = "MultipleBranches"
            });
        }

        // Store validation
        if (stores.Count == 0)
        {
            result.Errors.Add(new ExcelValidationError
            {
                RowNumber = headerRow,
                MessageKey = "StoreRequired"
            });
        }
        else if (stores.Count > 1)
        {
            result.Errors.Add(new ExcelValidationError
            {
                RowNumber = headerRow,
                MessageKey = "MultipleStores"
            });
        }

        result.ValidRows =
            result.Errors.Count == 0
                ? rows.Count
                : 0;

        return result;
    }
}