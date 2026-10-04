using ClosedXML.Excel;

namespace InventorySystem.Application.Common.Excel;

public static class ExcelReader
{
    public static List<InventoryExcelRow> Read(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);

        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
            throw new InvalidOperationException(
                "ملف Excel لا يحتوي على أي Sheet.");

        var (headerRow, columns) =
            ExcelHeaderDetector.Detect(worksheet);

        var rows = new List<InventoryExcelRow>();

        var lastRow = worksheet.LastRowUsed()?.RowNumber()
                      ?? headerRow;

        for (var rowNumber = headerRow + 1;
             rowNumber <= lastRow;
             rowNumber++)
        {
            var itemCode = GetValue(
                worksheet,
                rowNumber,
                columns,
                "ItemCode");

            // Ignore completely empty rows
            if (string.IsNullOrWhiteSpace(itemCode))
                continue;

            // Ignore total / footer / non-item rows
            if (!IsValidItemCode(itemCode))
                continue;

            var itemName1 = GetValue(
                worksheet,
                rowNumber,
                columns,
                "ItemName1");

            var itemName2 = GetValue(
                worksheet,
                rowNumber,
                columns,
                "ItemName2");

            var category = GetValue(
                worksheet,
                rowNumber,
                columns,
                "Category");

            var unit = GetValue(
                worksheet,
                rowNumber,
                columns,
                "Unit");

            var branch = GetValue(
                worksheet,
                rowNumber,
                columns,
                "Branch");

            var store = GetValue(
                worksheet,
                rowNumber,
                columns,
                "Store");

            var quantity = GetDecimalValue(
                worksheet,
                rowNumber,
                columns,
                "Quantity");

            var price = GetDecimalValue(
                worksheet,
                rowNumber,
                columns,
                "Price");

            rows.Add(new InventoryExcelRow
            {
                ItemCode = itemCode,
                ItemName1 = itemName1 ?? string.Empty,
                ItemName2 = itemName2,
                Category = category,
                Unit = unit,
                Branch = branch,
                Store = store,
                Quantity = quantity,
                Price = price
            });
        }

        return rows;
    }

    private static bool IsValidItemCode(string itemCode)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            return false;

        var value = itemCode.Trim();

        // Item codes must contain digits only.
        // This excludes rows such as:
        // المجموع
        // Total
        // الإجمالي
        return value.All(char.IsDigit);
    }

    private static string? GetValue(
        IXLWorksheet worksheet,
        int rowNumber,
        Dictionary<string, int> columns,
        string columnName)
    {
        if (!columns.TryGetValue(
                columnName,
                out var columnNumber))
        {
            return null;
        }

        var value = worksheet
            .Cell(rowNumber, columnNumber)
            .GetString()
            .Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    private static decimal? GetDecimalValue(
        IXLWorksheet worksheet,
        int rowNumber,
        Dictionary<string, int> columns,
        string columnName)
    {
        if (!columns.TryGetValue(
                columnName,
                out var columnNumber))
        {
            return null;
        }

        var cell = worksheet.Cell(
            rowNumber,
            columnNumber);

        if (cell.IsEmpty())
            return null;

        if (cell.TryGetValue<decimal>(
                out var numericValue))
        {
            return numericValue;
        }

        var text = cell
            .GetString()
            .Trim();

        if (string.IsNullOrWhiteSpace(text))
            return null;

        text = text.Replace(",", "");

        if (decimal.TryParse(
                text,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var result))
        {
            return result;
        }

        return null;
    }
}