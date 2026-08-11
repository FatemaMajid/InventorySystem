using ClosedXML.Excel;
using InventorySystem.Application.Common.Excel;
using InventorySystem.Application.Common.Excel.Validation;
using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Tests;

public class ExcelReaderTests
{
    private const string TestFileName =
        "الجرد_نصف_السنوي_العامرية_المخزن_بعد_22_06_2026.xlsx";

    [Fact]
    public void Read_AmiriyaInventoryFile_ShouldShowFirstRow()
    {
        var filePath = GetTestFilePath();

        Assert.True(
            File.Exists(filePath),
            "ملف اختبار الجرد غير موجود.");

        using var stream = File.OpenRead(filePath);

        var rows = ExcelReader.Read(stream);

        Assert.NotEmpty(rows);

        var firstRow = rows.First();

        Console.WriteLine("========== EXCEL TEST ==========");
        Console.WriteLine($"Rows: {rows.Count}");
        Console.WriteLine($"ItemCode: [{firstRow.ItemCode}]");
        Console.WriteLine($"ItemName1: [{firstRow.ItemName1}]");
        Console.WriteLine($"ItemName2: [{firstRow.ItemName2}]");
        Console.WriteLine($"Category: [{firstRow.Category}]");
        Console.WriteLine($"Unit: [{firstRow.Unit}]");
        Console.WriteLine($"Branch: [{firstRow.Branch}]");
        Console.WriteLine($"Store: [{firstRow.Store}]");
        Console.WriteLine($"Quantity: [{firstRow.Quantity}]");
        Console.WriteLine($"Price: [{firstRow.Price}]");
        Console.WriteLine("================================");

        Assert.False(
            string.IsNullOrWhiteSpace(firstRow.ItemCode));

        Assert.False(
            string.IsNullOrWhiteSpace(firstRow.ItemName1));

        Assert.NotNull(firstRow.Quantity);

        Assert.NotNull(firstRow.Price);
    }

    [Fact]
    public void Validate_AmiriyaInventoryFile_ShouldBeValid()
    {
        var filePath = GetTestFilePath();

        Assert.True(
            File.Exists(filePath),
            "ملف اختبار الجرد غير موجود.");

        using var stream = File.OpenRead(filePath);

        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.First();

        var (headerRow, _) =
            ExcelHeaderDetector.Detect(worksheet);

        stream.Position = 0;

        var rows = ExcelReader.Read(stream);

        var result = ExcelValidator.Validate(
            rows,
            headerRow);

        Console.WriteLine("========== VALIDATION TEST ==========");
        Console.WriteLine($"Header Row: {headerRow}");
        Console.WriteLine($"Total Rows: {result.TotalRows}");
        Console.WriteLine($"Valid Rows: {result.ValidRows}");
        Console.WriteLine($"Errors: {result.Errors.Count}");

        foreach (var error in result.Errors)
        {
            Console.WriteLine(
                $"Row: {error.RowNumber} | " +
                $"Item: {error.ItemCode} | " +
                $"Key: {error.MessageKey}");

            Console.WriteLine(
                $"AR: {LocalizationService.Get(
                    error.MessageKey,
                    "ar")}");

            Console.WriteLine(
                $"EN: {LocalizationService.Get(
                    error.MessageKey,
                    "en")}");
        }

        Console.WriteLine("====================================");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateLocation_AmiriyaInventoryFile_ShouldHaveOneBranchAndOneStore()
    {
        var filePath = GetTestFilePath();

        Assert.True(
            File.Exists(filePath),
            "ملف اختبار الجرد غير موجود.");

        using var stream = File.OpenRead(filePath);

        var rows = ExcelReader.Read(stream);

        Assert.NotEmpty(rows);

        using var workbook = new XLWorkbook(filePath);

        var worksheet = workbook.Worksheets.First();

        var (headerRow, _) =
            ExcelHeaderDetector.Detect(worksheet);

        var result = InventoryLocationValidator.Validate(
            rows,
            headerRow);

        Console.WriteLine(
            "========== LOCATION VALIDATION ==========");

        Console.WriteLine(
            $"Header Row: {headerRow}");

        Console.WriteLine(
            $"Total Rows: {result.TotalRows}");

        Console.WriteLine(
            $"Valid Rows: {result.ValidRows}");

        Console.WriteLine(
            $"Errors: {result.Errors.Count}");

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

        Console.WriteLine(
            $"Branches: {branches.Count}");

        foreach (var branch in branches)
        {
            Console.WriteLine(
                $"Branch: [{branch}]");
        }

        Console.WriteLine(
            $"Stores: {stores.Count}");

        foreach (var store in stores)
        {
            Console.WriteLine(
                $"Store: [{store}]");
        }

        foreach (var error in result.Errors)
        {
            Console.WriteLine(
                $"Row: {error.RowNumber} | " +
                $"Key: {error.MessageKey}");

            Console.WriteLine(
                $"AR: {LocalizationService.Get(
                    error.MessageKey,
                    "ar")}");

            Console.WriteLine(
                $"EN: {LocalizationService.Get(
                    error.MessageKey,
                    "en")}");
        }

        Console.WriteLine(
            "=========================================");

        Assert.True(result.IsValid);

        Assert.Single(branches);
        Assert.Single(stores);
    }

    private static string GetTestFilePath()
    {
        return Path.Combine(
            AppContext.BaseDirectory,
            "TestData",
            TestFileName);
    }
}