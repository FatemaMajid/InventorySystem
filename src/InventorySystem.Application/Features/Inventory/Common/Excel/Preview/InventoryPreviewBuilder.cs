using InventorySystem.Application.Common.Excel.Validation;

namespace InventorySystem.Application.Common.Excel.Preview;

public static class InventoryPreviewBuilder
{
    public static InventoryImportPreview Build(
        IReadOnlyList<InventoryExcelRow> rows,
        ExcelValidationResult validationResult,
        ExcelValidationResult locationValidationResult,
        int headerRow)
    {
        var preview = new InventoryImportPreview
        {
            TotalRows = rows.Count,
            ValidRows = validationResult.ValidRows,
            ErrorRows =
                validationResult.Errors.Count +
                locationValidationResult.Errors.Count
        };

        preview.Branch = GetSingleValue(
            rows.Select(x => x.Branch));

        preview.Store = GetSingleValue(
            rows.Select(x => x.Store));

        for (var index = 0; index < rows.Count; index++)
        {
            var row = rows[index];
            var excelRowNumber = headerRow + index + 1;

            var rowErrors = validationResult.Errors
                .Where(x => x.RowNumber == excelRowNumber)
                .Select(x => x.MessageKey)
                .ToList();

            var totalValue = CalculateTotalValue(
                row.Quantity,
                row.Price);

            preview.Rows.Add(new InventoryImportPreviewRow
            {
                RowNumber = excelRowNumber,
                ItemCode = row.ItemCode,
                ItemName1 = row.ItemName1,
                ItemName2 = row.ItemName2,
                Category = row.Category,
                Unit = row.Unit,
                Branch = row.Branch,
                Store = row.Store,
                Quantity = row.Quantity,
                Price = row.Price,
                TotalValue = totalValue,
                IsValid =
                    rowErrors.Count == 0 &&
                    locationValidationResult.IsValid,
                Errors = rowErrors
            });

            if (totalValue.HasValue)
            {
                preview.TotalQuantity += row.Quantity ?? 0;
                preview.TotalValue += totalValue.Value;
            }
        }

        return preview;
    }

    private static string? GetSingleValue(
        IEnumerable<string?> values)
    {
        return values
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .SingleOrDefault();
    }

    private static decimal? CalculateTotalValue(
        decimal? quantity,
        decimal? price)
    {
        if (!quantity.HasValue || !price.HasValue)
            return null;

        return quantity.Value * price.Value;
    }
}