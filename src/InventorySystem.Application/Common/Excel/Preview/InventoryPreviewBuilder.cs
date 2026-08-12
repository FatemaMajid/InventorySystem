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

        // Get the single branch from the file.
        preview.Branch = rows
            .Where(x => !string.IsNullOrWhiteSpace(x.Branch))
            .Select(x => x.Branch!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .SingleOrDefault();

        // Get the single store from the file.
        preview.Store = rows
            .Where(x => !string.IsNullOrWhiteSpace(x.Store))
            .Select(x => x.Store!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .SingleOrDefault();

        // Build preview rows.
        for (var index = 0; index < rows.Count; index++)
        {
            var row = rows[index];

            var excelRowNumber =
                headerRow + index + 1;

            var rowErrors = validationResult.Errors
                .Where(x =>
                    x.RowNumber == excelRowNumber)
                .Select(x => x.MessageKey)
                .ToList();

            // Total value = Quantity × Price
            decimal? totalValue = null;

            if (row.Quantity.HasValue &&
                row.Price.HasValue)
            {
                totalValue =
                    row.Quantity.Value *
                    row.Price.Value;
            }

            preview.Rows.Add(
                new InventoryImportPreviewRow
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

                    IsValid = rowErrors.Count == 0,

                    Errors = rowErrors
                });

            if (totalValue.HasValue)
            {
                preview.TotalQuantity +=
                    row.Quantity ?? 0;

                preview.TotalValue +=
                    totalValue.Value;
            }
        }

        return preview;
    }
}