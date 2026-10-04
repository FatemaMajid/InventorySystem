using InventorySystem.Application.Common.Excel;
using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Common.Excel.Validation;

public static class ExcelValidator
{
    public static ExcelValidationResult Validate(
        IReadOnlyList<InventoryExcelRow> rows,
        int headerRow)
    {
        var result = new ExcelValidationResult
        {
            TotalRows = rows.Count
        };

        var itemCodes = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < rows.Count; index++)
        {
            var row = rows[index];
            var excelRowNumber = headerRow + index + 1;
            var hasError = false;

            ValidateItemCode(
                result,
                row,
                excelRowNumber,
                itemCodes,
                ref hasError);

            ValidateItemName(
                result,
                row,
                excelRowNumber,
                ref hasError);

            ValidateQuantity(
                result,
                row,
                excelRowNumber,
                ref hasError);

            ValidatePrice(
                result,
                row,
                excelRowNumber,
                ref hasError);

            if (!hasError)
                result.ValidRows++;
        }

        return result;
    }

    private static void ValidateItemCode(
        ExcelValidationResult result,
        InventoryExcelRow row,
        int rowNumber,
        HashSet<string> itemCodes,
        ref bool hasError)
    {
        if (string.IsNullOrWhiteSpace(row.ItemCode))
        {
            AddError(
                result,
                rowNumber,
                row.ItemCode,
                LocalizationKeys.ItemCodeRequired);

            hasError = true;
            return;
        }

        if (!itemCodes.Add(row.ItemCode.Trim()))
        {
            AddError(
                result,
                rowNumber,
                row.ItemCode,
                LocalizationKeys.DuplicateItemCode);

            hasError = true;
        }
    }

    private static void ValidateItemName(
        ExcelValidationResult result,
        InventoryExcelRow row,
        int rowNumber,
        ref bool hasError)
    {
        if (string.IsNullOrWhiteSpace(row.ItemName1))
        {
            AddError(
                result,
                rowNumber,
                row.ItemCode,
                LocalizationKeys.ItemNameRequired);

            hasError = true;
        }
    }

    private static void ValidateQuantity(
        ExcelValidationResult result,
        InventoryExcelRow row,
        int rowNumber,
        ref bool hasError)
    {
        if (!row.Quantity.HasValue)
        {
            AddError(
                result,
                rowNumber,
                row.ItemCode,
                LocalizationKeys.QuantityRequired);

            hasError = true;
            return;
        }

        if (row.Quantity.Value < 0)
        {
            AddError(
                result,
                rowNumber,
                row.ItemCode,
                LocalizationKeys.NegativeQuantity);

            hasError = true;
        }
    }

    private static void ValidatePrice(
        ExcelValidationResult result,
        InventoryExcelRow row,
        int rowNumber,
        ref bool hasError)
    {
        if (!row.Price.HasValue)
        {
            AddError(
                result,
                rowNumber,
                row.ItemCode,
                LocalizationKeys.PriceRequired);

            hasError = true;
            return;
        }

        if (row.Price.Value < 0)
        {
            AddError(
                result,
                rowNumber,
                row.ItemCode,
                LocalizationKeys.NegativePrice);

            hasError = true;
        }
    }

    private static void AddError(
        ExcelValidationResult result,
        int rowNumber,
        string? itemCode,
        string messageKey)
    {
        result.Errors.Add(new ExcelValidationError
        {
            RowNumber = rowNumber,
            ItemCode = itemCode,
            MessageKey = messageKey
        });
    }
}