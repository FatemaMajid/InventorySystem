using InventorySystem.Application.Common.Excel;
using InventorySystem.Application.Common.Localization;

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

        ValidateBranches(result, branches, headerRow);
        ValidateStores(result, stores, headerRow);

        result.ValidRows =
            result.Errors.Count == 0
                ? rows.Count
                : 0;

        return result;
    }

    private static void ValidateBranches(
        ExcelValidationResult result,
        IReadOnlyList<string> branches,
        int headerRow)
    {
        if (branches.Count == 0)
        {
            AddError(
                result,
                headerRow,
                LocalizationKeys.BranchRequired);

            return;
        }

        if (branches.Count > 1)
        {
            AddError(
                result,
                headerRow,
                LocalizationKeys.MultipleBranches);
        }
    }

    private static void ValidateStores(
        ExcelValidationResult result,
        IReadOnlyList<string> stores,
        int headerRow)
    {
        if (stores.Count == 0)
        {
            AddError(
                result,
                headerRow,
                LocalizationKeys.StoreRequired);

            return;
        }

        if (stores.Count > 1)
        {
            AddError(
                result,
                headerRow,
                LocalizationKeys.MultipleStores);
        }
    }

    private static void AddError(
        ExcelValidationResult result,
        int rowNumber,
        string messageKey)
    {
        result.Errors.Add(new ExcelValidationError
        {
            RowNumber = rowNumber,
            MessageKey = messageKey
        });
    }
}