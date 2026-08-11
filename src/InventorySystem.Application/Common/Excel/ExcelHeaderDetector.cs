using ClosedXML.Excel;

namespace InventorySystem.Application.Common.Excel;

public static class ExcelHeaderDetector
{
    private static readonly string[] RequiredColumns =
    {
        "ItemCode",
        "ItemName1",
        "Quantity",
        "Price"
    };

    public static (int HeaderRow, Dictionary<string, int> Columns)
        Detect(IXLWorksheet worksheet)
    {
        var usedRange = worksheet.RangeUsed();

        if (usedRange == null)
            throw new InvalidOperationException("ملف Excel فارغ.");

        foreach (var row in usedRange.Rows())
        {
            var columns = new Dictionary<string, int>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var cell in row.Cells())
            {
                var columnName = ExcelColumnMapper.Map(cell.GetString());

                if (columnName != null &&
                    !columns.ContainsKey(columnName))
                {
                    columns[columnName] = cell.Address.ColumnNumber;
                }
            }

            var hasRequiredColumns = RequiredColumns.All(
                required => columns.ContainsKey(required));

            if (hasRequiredColumns)
            {
                return (
                    row.RowNumber(),
                    columns
                );
            }
        }

        throw new InvalidOperationException(
            "لم يتم العثور على صف عناوين صالح في ملف Excel. " +
            "تأكد من وجود الأعمدة المطلوبة: رقم الصنف، اسم الصنف، الكمية، السعر.");
    }
}
