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

            // Item Code
            if (string.IsNullOrWhiteSpace(row.ItemCode))
            {
                AddError(
                    result,
                    excelRowNumber,
                    row.ItemCode,
                    LocalizationKeys.ItemCodeRequired);

                hasError = true;
            }
            else if (!itemCodes.Add(row.ItemCode.Trim()))
            {
                AddError(
                    result,
                    excelRowNumber,
                    row.ItemCode,
                    LocalizationKeys.DuplicateItemCode);

                hasError = true;
            }

            // Item Name 1
            if (string.IsNullOrWhiteSpace(row.ItemName1))
            {
                AddError(
                    result,
                    excelRowNumber,
                    row.ItemCode,
                    LocalizationKeys.ItemNameRequired);

                hasError = true;
            }

            // Quantity
            if (!row.Quantity.HasValue)
            {
                AddError(
                    result,
                    excelRowNumber,
                    row.ItemCode,
                    LocalizationKeys.QuantityRequired);

                hasError = true;
            }
            else if (row.Quantity.Value < 0)
            {
                AddError(
                    result,
                    excelRowNumber,
                    row.ItemCode,
                    LocalizationKeys.NegativeQuantity);

                hasError = true;
            }

            // Price
            if (!row.Price.HasValue)
            {
                AddError(
                    result,
                    excelRowNumber,
                    row.ItemCode,
                    LocalizationKeys.PriceRequired);

                hasError = true;
            }
            else if (row.Price.Value < 0)
            {
                AddError(
                    result,
                    excelRowNumber,
                    row.ItemCode,
                    LocalizationKeys.NegativePrice);

                hasError = true;
            }

            if (!hasError)
            {
                result.ValidRows++;
            }
        }

        return result;
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