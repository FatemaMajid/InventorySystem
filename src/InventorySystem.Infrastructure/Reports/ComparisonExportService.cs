// using ClosedXML.Excel;
// using InventorySystem.Application.Common.Interfaces;
// using InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryComparisonExport;
// using MediatR;
// using QuestPDF.Fluent;
// using QuestPDF.Helpers;
// using QuestPDF.Infrastructure;

// namespace InventorySystem.Infrastructure.Reports;

// public sealed class ComparisonExportService : IComparisonExportService
// {
//     private readonly ISender _sender;

//     public ComparisonExportService(ISender sender)
//     {
//         _sender = sender;
//     }

//     private static bool Ar(string language) =>
//         language.Equals("ar",StringComparison.OrdinalIgnoreCase);

//     private static string T(string key,string language)
//     {
//         var ar = new Dictionary<string,string>
//         {
//             ["Title"]="نتائج المقارنة",
//             ["No"]="#",
//             ["Code"]="رمز المادة",
//             ["Name1"]="اسم المادة",
//             ["Name2"]="اسم المادة 2",
//             ["Category"]="التصنيف",
//             ["Unit"]="الوحدة",
//             ["BeforeQty"]="الكمية قبل",
//             ["AfterQty"]="الكمية بعد",
//             ["QtyDiff"]="فرق الكمية",
//             ["QtyPercent"]="نسبة الفرق",
//             ["BeforePrice"]="سعر المستهلك قبل",
//             ["AfterPrice"]="سعر المستهلك بعد",
//             ["BeforeValue"]="القيمة قبل",
//             ["AfterValue"]="القيمة بعد",
//             ["ValueDiff"]="فرق القيمة",
//             ["Status"]="الحالة",
//             ["Description"]="الوصف"
//         };

//         var en = new Dictionary<string,string>
//         {
//             ["Title"]="Comparison Results",
//             ["No"]="#",
//             ["Code"]="Item Code",
//             ["Name1"]="Item Name",
//             ["Name2"]="Item Name 2",
//             ["Category"]="Category",
//             ["Unit"]="Unit",
//             ["BeforeQty"]="Quantity Before",
//             ["AfterQty"]="Quantity After",
//             ["QtyDiff"]="Quantity Difference",
//             ["QtyPercent"]="Difference %",
//             ["BeforePrice"]="Consumer Price Before",
//             ["AfterPrice"]="Consumer Price After",
//             ["BeforeValue"]="Value Before",
//             ["AfterValue"]="Value After",
//             ["ValueDiff"]="Value Difference",
//             ["Status"]="Status",
//             ["Description"]="Description"
//         };

//         return (Ar(language) ? ar : en).GetValueOrDefault(key,key);
//     }

//     private async Task<IReadOnlyList<InventoryComparisonExportItem>> GetData(
//         int sessionId,
//         string? itemCode,
//         string? itemName,
//         int? categoryId,
//         int? unitId,
//         string? status,
//         string sortBy,
//         bool descending,
//         CancellationToken ct)
//     {
//         return await _sender.Send(
//             new GetInventoryComparisonExportQuery(
//                 sessionId,
//                 itemCode,
//                 itemName,
//                 categoryId,
//                 unitId,
//                 status,
//                 sortBy,
//                 descending),
//             ct);
//     }

//     public async Task<byte[]> ExportExcelAsync(
//         int sessionId,
//         string? itemCode,
//         string? itemName,
//         int? categoryId,
//         int? unitId,
//         string? status,
//         string sortBy,
//         bool descending,
//         string language,
//         CancellationToken cancellationToken)
//     {
//         var data = await GetData(
//             sessionId,itemCode,itemName,categoryId,unitId,status,
//             sortBy,descending,cancellationToken);

//         using var workbook = new XLWorkbook();
//         var ws = workbook.Worksheets.Add(
//             Ar(language) ? "المقارنة" : "Comparison");

//         ws.RightToLeft = Ar(language);

//         ws.Cell(1,1).Value = T("Title",language);
//         ws.Range(1,1,1,17).Merge();
//         ws.Cell(1,1).Style.Font.Bold = true;
//         ws.Cell(1,1).Style.Font.FontSize = 18;

//         var headers = new[]
//         {
//             "No","Code","Name1","Name2","Category","Unit",
//             "BeforeQty","AfterQty","QtyDiff","QtyPercent",
//             "BeforePrice","AfterPrice","BeforeValue","AfterValue",
//             "ValueDiff","Status","Description"
//         };

//         for(var i=0;i<headers.Length;i++)
//         {
//             ws.Cell(3,i+1).Value = T(headers[i],language);
//             ws.Cell(3,i+1).Style.Font.Bold = true;
//         }

//         var row = 4;
//         var number = 1;

//         foreach(var x in data)
//         {
//             ws.Cell(row,1).Value = number++;
//             ws.Cell(row,2).Value = x.ItemCode;
//             ws.Cell(row,3).Value = x.ItemName1;
//             ws.Cell(row,4).Value = x.ItemName2 ?? "";
//             ws.Cell(row,5).Value = x.CategoryName;
//             ws.Cell(row,6).Value = x.UnitName;
//             ws.Cell(row,7).Value = x.QuantityBefore;
//             ws.Cell(row,8).Value = x.QuantityAfter;
//             ws.Cell(row,9).Value = x.QuantityDifference;
//             ws.Cell(row,10).Value = x.DifferencePercentage;
//             ws.Cell(row,11).Value = x.ConsumerPriceBefore;
//             ws.Cell(row,12).Value = x.ConsumerPriceAfter;
//             ws.Cell(row,13).Value = x.BeforeValue;
//             ws.Cell(row,14).Value = x.AfterValue;
//             ws.Cell(row,15).Value = x.ValueDifference;
//             ws.Cell(row,16).Value = x.Status;
//             ws.Cell(row,17).Value = x.Description ?? "";
//             row++;
//         }

//         ws.SheetView.FreezeRows(3);
//         ws.Columns().AdjustToContents();

//         using var stream = new MemoryStream();
//         workbook.SaveAs(stream);
//         return stream.ToArray();
//     }

//     public async Task<byte[]> ExportPdfAsync(
//         int sessionId,
//         string? itemCode,
//         string? itemName,
//         int? categoryId,
//         int? unitId,
//         string? status,
//         string sortBy,
//         bool descending,
//         string language,
//         CancellationToken cancellationToken)
//     {
//         var data = await GetData(
//             sessionId,itemCode,itemName,categoryId,unitId,status,
//             sortBy,descending,cancellationToken);

//         QuestPDF.Settings.License = LicenseType.Community;

//         var document = Document.Create(container =>
//         {
//             container.Page(page =>
//             {
//                 page.Size(PageSizes.A4.Landscape());
//                 page.Margin(20);
//                 page.DefaultTextStyle(x =>
//                     x.FontFamily("Arial").FontSize(7));

//                 page.Header()
//                     .AlignCenter()
//                     .Text(T("Title",language))
//                     .Bold()
//                     .FontSize(16);

//                 page.Content().Table(table =>
//                 {
//                     table.ColumnsDefinition(c =>
//                     {
//                         c.ConstantColumn(20);
//                         c.ConstantColumn(55);
//                         c.RelativeColumn(1.5f);
//                         c.RelativeColumn(1.5f);
//                         c.RelativeColumn(1.2f);
//                         c.ConstantColumn(45);
//                         c.ConstantColumn(50);
//                         c.ConstantColumn(50);
//                         c.ConstantColumn(50);
//                         c.ConstantColumn(45);
//                         c.ConstantColumn(55);
//                         c.ConstantColumn(55);
//                         c.ConstantColumn(60);
//                         c.ConstantColumn(60);
//                         c.ConstantColumn(60);
//                         c.ConstantColumn(65);
//                         c.RelativeColumn(1.5f);
//                     });

//                     foreach(var h in new[]
//                     {
//                         "No","Code","Name1","Name2","Category","Unit",
//                         "BeforeQty","AfterQty","QtyDiff","QtyPercent",
//                         "BeforePrice","AfterPrice","BeforeValue","AfterValue",
//                         "ValueDiff","Status","Description"
//                     })
//                         Cell(table,T(h,language),true);

//                     var n = 1;

//                     foreach(var x in data)
//                     {
//                         Cell(table,n++.ToString());
//                         Cell(table,x.ItemCode);
//                         Cell(table,x.ItemName1);
//                         Cell(table,x.ItemName2 ?? "");
//                         Cell(table,x.CategoryName);
//                         Cell(table,x.UnitName);
//                         Cell(table,x.QuantityBefore?.ToString("N2") ?? "");
//                         Cell(table,x.QuantityAfter?.ToString("N2") ?? "");
//                         Cell(table,x.QuantityDifference?.ToString("N2") ?? "");
//                         Cell(table,x.DifferencePercentage?.ToString("N2") ?? "");
//                         Cell(table,x.ConsumerPriceBefore?.ToString("N2") ?? "");
//                         Cell(table,x.ConsumerPriceAfter?.ToString("N2") ?? "");
//                         Cell(table,x.BeforeValue?.ToString("N2") ?? "");
//                         Cell(table,x.AfterValue?.ToString("N2") ?? "");
//                         Cell(table,x.ValueDifference?.ToString("N2") ?? "");
//                         Cell(table,x.Status);
//                         Cell(table,x.Description ?? "");
//                     }
//                 });

//                 page.Footer()
//                     .AlignCenter()
//                     .Text(
//                         $"{data.Count} {T("Code",language)}");
//             });
//         });

//         return document.GeneratePdf();
//     }

//     private static void Cell(
//         TableDescriptor table,
//         string text,
//         bool bold = false)
//     {
//         var cell = table.Cell()
//             .Border(1)
//             .Padding(3);

//         if(bold)
//             cell.Text(text).Bold();
//         else
//             cell.Text(text);
//     }
// }


using ClosedXML.Excel;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryComparisonExport;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventorySystem.Infrastructure.Reports;

public sealed class ComparisonExportService : IComparisonExportService
{
    private readonly ISender _sender;

    public ComparisonExportService(ISender sender)
    {
        _sender = sender;
    }

    private static bool Ar(string language) =>
        language.Equals("ar",StringComparison.OrdinalIgnoreCase);

    private static string T(string key,string language)
    {
        var ar = new Dictionary<string,string>
        {
            ["Title"]="نتائج المقارنة",
            ["No"]="#",
            ["Code"]="رمز المادة",
            ["Name1"]="اسم المادة",
            ["Name2"]="اسم المادة 2",
            ["Category"]="التصنيف",
            ["Unit"]="الوحدة",
            ["BeforeQty"]="الكمية قبل",
            ["AfterQty"]="الكمية بعد",
            ["QtyDiff"]="فرق الكمية",
            ["QtyPercent"]="نسبة الفرق",
            ["BeforePrice"]="سعر المستهلك قبل",
            ["AfterPrice"]="سعر المستهلك بعد",
            ["BeforeValue"]="القيمة قبل",
            ["AfterValue"]="القيمة بعد",
            ["ValueDiff"]="فرق القيمة",
            ["Status"]="الحالة",
            ["Description"]="الوصف"
        };

        var en = new Dictionary<string,string>
        {
            ["Title"]="Comparison Results",
            ["No"]="#",
            ["Code"]="Item Code",
            ["Name1"]="Item Name",
            ["Name2"]="Item Name 2",
            ["Category"]="Category",
            ["Unit"]="Unit",
            ["BeforeQty"]="Quantity Before",
            ["AfterQty"]="Quantity After",
            ["QtyDiff"]="Quantity Difference",
            ["QtyPercent"]="Difference %",
            ["BeforePrice"]="Consumer Price Before",
            ["AfterPrice"]="Consumer Price After",
            ["BeforeValue"]="Value Before",
            ["AfterValue"]="Value After",
            ["ValueDiff"]="Value Difference",
            ["Status"]="Status",
            ["Description"]="Description"
        };

        return (Ar(language) ? ar : en).GetValueOrDefault(key,key);
    }

    private async Task<IReadOnlyList<InventoryComparisonExportItem>> GetData(
        int sessionId,
        string? itemCode,
        string? itemName,
        int? categoryId,
        int? unitId,
        string? status,
        string sortBy,
        bool descending,
        CancellationToken ct)
    {
        return await _sender.Send(
            new GetInventoryComparisonExportQuery(
                sessionId,
                itemCode,
                itemName,
                categoryId,
                unitId,
                status,
                sortBy,
                descending),
            ct);
    }

    public async Task<byte[]> ExportExcelAsync(
        int sessionId,
        string? itemCode,
        string? itemName,
        int? categoryId,
        int? unitId,
        string? status,
        string sortBy,
        bool descending,
        string language,
        CancellationToken cancellationToken)
    {
        var data = await GetData(
            sessionId,itemCode,itemName,categoryId,unitId,status,
            sortBy,descending,cancellationToken);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(
            Ar(language) ? "المقارنة" : "Comparison");

        var rtl = Ar(language);
        ws.RightToLeft = rtl;

        const int colCount = 17;
        const string qtyFormat = "#,##0.00;[Red]-#,##0.00";
        const string percentFormat = "0.00\"%\";[Red]-0.00\"%\"";
        const string currencyFormat = "#,##0.00;[Red]-#,##0.00";

        // --- Title banner ---
        ws.Cell(1,1).Value = T("Title",language);
        ws.Range(1,1,1,colCount).Merge();
        ws.Cell(1,1).Style.Font.Bold = true;
        ws.Cell(1,1).Style.Font.FontSize = 18;
        ws.Cell(1,1).Style.Font.FontColor = XLColor.White;
        ws.Cell(1,1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
        ws.Cell(1,1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(1,1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Row(1).Height = 28;

        // --- Subtitle: item count + generated timestamp ---
        var subtitle = Ar(language)
            ? $"عدد المواد: {data.Count}    |    تاريخ الإنشاء: {DateTime.Now:yyyy-MM-dd HH:mm}"
            : $"Items: {data.Count}    |    Generated: {DateTime.Now:yyyy-MM-dd HH:mm}";
        ws.Cell(2,1).Value = subtitle;
        ws.Range(2,1,2,colCount).Merge();
        ws.Cell(2,1).Style.Font.Italic = true;
        ws.Cell(2,1).Style.Font.FontColor = XLColor.FromHtml("#595959");
        ws.Cell(2,1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // --- Header row ---
        var headers = new[]
        {
            "No","Code","Name1","Name2","Category","Unit",
            "BeforeQty","AfterQty","QtyDiff","QtyPercent",
            "BeforePrice","AfterPrice","BeforeValue","AfterValue",
            "ValueDiff","Status","Description"
        };

        const int headerRow = 4;

        for(var i=0;i<headers.Length;i++)
        {
            var cell = ws.Cell(headerRow,i+1);
            cell.Value = T(headers[i],language);
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }
        ws.Row(headerRow).Height = 30;

        // --- Data rows ---
        var row = headerRow + 1;
        var number = 1;
        decimal beforeValueTotal = 0, afterValueTotal = 0, valueDiffTotal = 0;

        foreach(var x in data)
        {
            ws.Cell(row,1).Value = number++;
            ws.Cell(row,2).Value = x.ItemCode;
            ws.Cell(row,3).Value = x.ItemName1;
            ws.Cell(row,4).Value = x.ItemName2 ?? "";
            ws.Cell(row,5).Value = x.CategoryName;
            ws.Cell(row,6).Value = x.UnitName;
            ws.Cell(row,7).Value = x.QuantityBefore;
            ws.Cell(row,8).Value = x.QuantityAfter;
            ws.Cell(row,9).Value = x.QuantityDifference;
            ws.Cell(row,10).Value = x.DifferencePercentage;
            ws.Cell(row,11).Value = x.ConsumerPriceBefore;
            ws.Cell(row,12).Value = x.ConsumerPriceAfter;
            ws.Cell(row,13).Value = x.BeforeValue;
            ws.Cell(row,14).Value = x.AfterValue;
            ws.Cell(row,15).Value = x.ValueDifference;
            // Translate known status codes (Increase/Decrease/Match/...)
            // to the user's language; T() falls back to the raw value
            // for anything not in the dictionary, so this is always safe.
            ws.Cell(row,16).Value = T(x.Status,language);
            ws.Cell(row,17).Value = x.Description ?? "";

            ws.Cell(row,7).Style.NumberFormat.Format = qtyFormat;
            ws.Cell(row,8).Style.NumberFormat.Format = qtyFormat;
            ws.Cell(row,9).Style.NumberFormat.Format = qtyFormat;
            ws.Cell(row,10).Style.NumberFormat.Format = percentFormat;
            ws.Cell(row,11).Style.NumberFormat.Format = currencyFormat;
            ws.Cell(row,12).Style.NumberFormat.Format = currencyFormat;
            ws.Cell(row,13).Style.NumberFormat.Format = currencyFormat;
            ws.Cell(row,14).Style.NumberFormat.Format = currencyFormat;
            ws.Cell(row,15).Style.NumberFormat.Format = currencyFormat;

            // Color quantity/value diffs by sign so gains/losses are
            // visible at a glance without reading every number.
            ColorBySign(ws.Cell(row,9), x.QuantityDifference);
            ColorBySign(ws.Cell(row,15), x.ValueDifference);

            // Status badge: colored fill on the status cell itself,
            // matching the same palette used across the dashboard.
            var (fill, font) = StatusColors(x.Status);
            var statusCell = ws.Cell(row,16);
            statusCell.Style.Fill.BackgroundColor = fill;
            statusCell.Style.Font.FontColor = font;
            statusCell.Style.Font.Bold = true;
            statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Thin grid + light zebra striping for readability across
            // wide rows, without fighting the status badge color.
            var stripe = number % 2 == 0
                ? XLColor.FromHtml("#F2F6FA")
                : XLColor.White;

            for(var c=1;c<=colCount;c++)
            {
                var cell = ws.Cell(row,c);
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D9D9D9");
                if(c != 9 && c != 15 && c != 16)
                    cell.Style.Fill.BackgroundColor = stripe;
            }

            beforeValueTotal += x.BeforeValue ?? 0;
            afterValueTotal += x.AfterValue ?? 0;
            valueDiffTotal += x.ValueDifference ?? 0;

            row++;
        }

        // --- Totals row ---
        var totalsRow = row;
        ws.Cell(totalsRow,12).Value = Ar(language) ? "الإجمالي" : "Total";
        ws.Cell(totalsRow,13).Value = beforeValueTotal;
        ws.Cell(totalsRow,14).Value = afterValueTotal;
        ws.Cell(totalsRow,15).Value = valueDiffTotal;

        var totalsRange = ws.Range(totalsRow,12,totalsRow,15);
        totalsRange.Style.Font.Bold = true;
        totalsRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#DDEBF7");
        totalsRange.Style.Border.TopBorder = XLBorderStyleValues.Double;
        ws.Cell(totalsRow,13).Style.NumberFormat.Format = currencyFormat;
        ws.Cell(totalsRow,14).Style.NumberFormat.Format = currencyFormat;
        ws.Cell(totalsRow,15).Style.NumberFormat.Format = currencyFormat;
        ColorBySign(ws.Cell(totalsRow,15), valueDiffTotal);

        // --- Layout: autofilter, frozen header + id columns, widths ---
        var dataRange = ws.Range(headerRow,1,Math.Max(headerRow,row-1),colCount);
        dataRange.SetAutoFilter();

        ws.SheetView.Freeze(headerRow, rtl ? 0 : 2);

        var widths = new (int Col,double Width)[]
        {
            (1,6),(2,14),(3,26),(4,22),(5,16),(6,12),
            (7,14),(8,14),(9,14),(10,12),
            (11,14),(12,14),(13,16),(14,16),
            (15,16),(16,16),(17,24)
        };
        foreach(var (col,width) in widths)
            ws.Column(col).Width = width;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void ColorBySign(IXLCell cell, decimal? value)
    {
        if(value is null || value == 0)
        {
            cell.Style.Font.FontColor = XLColor.FromHtml("#595959");
            return;
        }

        cell.Style.Font.FontColor = value > 0
            ? XLColor.FromHtml("#1E7A34")
            : XLColor.FromHtml("#C00000");
        cell.Style.Font.Bold = true;
    }

    private static (XLColor Fill,XLColor Font) StatusColors(string status) =>
        status switch
        {
            "Increase" => (XLColor.FromHtml("#E2EFDA"), XLColor.FromHtml("#1E7A34")),
            "Decrease" => (XLColor.FromHtml("#FCE4E4"), XLColor.FromHtml("#C00000")),
            "Match" => (XLColor.FromHtml("#EDEDED"), XLColor.FromHtml("#595959")),
            "NewlyCounted" => (XLColor.FromHtml("#DDEBF7"), XLColor.FromHtml("#1F4E78")),
            "FullyDepleted" => (XLColor.FromHtml("#FCEACD"), XLColor.FromHtml("#B25E00")),
            _ => (XLColor.White, XLColor.Black)
        };

    public async Task<byte[]> ExportPdfAsync(
        int sessionId,
        string? itemCode,
        string? itemName,
        int? categoryId,
        int? unitId,
        string? status,
        string sortBy,
        bool descending,
        string language,
        CancellationToken cancellationToken)
    {
        var data = await GetData(
            sessionId,itemCode,itemName,categoryId,unitId,status,
            sortBy,descending,cancellationToken);

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.DefaultTextStyle(x =>
                    x.FontFamily("Arial").FontSize(7));

                page.Header()
                    .AlignCenter()
                    .Text(T("Title",language))
                    .Bold()
                    .FontSize(16);

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(20);
                        c.ConstantColumn(55);
                        c.RelativeColumn(1.5f);
                        c.RelativeColumn(1.5f);
                        c.RelativeColumn(1.2f);
                        c.ConstantColumn(45);
                        c.ConstantColumn(50);
                        c.ConstantColumn(50);
                        c.ConstantColumn(50);
                        c.ConstantColumn(45);
                        c.ConstantColumn(55);
                        c.ConstantColumn(55);
                        c.ConstantColumn(60);
                        c.ConstantColumn(60);
                        c.ConstantColumn(60);
                        c.ConstantColumn(65);
                        c.RelativeColumn(1.5f);
                    });

                    foreach(var h in new[]
                    {
                        "No","Code","Name1","Name2","Category","Unit",
                        "BeforeQty","AfterQty","QtyDiff","QtyPercent",
                        "BeforePrice","AfterPrice","BeforeValue","AfterValue",
                        "ValueDiff","Status","Description"
                    })
                        Cell(table,T(h,language),true);

                    var n = 1;

                    foreach(var x in data)
                    {
                        Cell(table,n++.ToString());
                        Cell(table,x.ItemCode);
                        Cell(table,x.ItemName1);
                        Cell(table,x.ItemName2 ?? "");
                        Cell(table,x.CategoryName);
                        Cell(table,x.UnitName);
                        Cell(table,x.QuantityBefore?.ToString("N2") ?? "");
                        Cell(table,x.QuantityAfter?.ToString("N2") ?? "");
                        Cell(table,x.QuantityDifference?.ToString("N2") ?? "");
                        Cell(table,x.DifferencePercentage?.ToString("N2") ?? "");
                        Cell(table,x.ConsumerPriceBefore?.ToString("N2") ?? "");
                        Cell(table,x.ConsumerPriceAfter?.ToString("N2") ?? "");
                        Cell(table,x.BeforeValue?.ToString("N2") ?? "");
                        Cell(table,x.AfterValue?.ToString("N2") ?? "");
                        Cell(table,x.ValueDifference?.ToString("N2") ?? "");
                        Cell(table,x.Status);
                        Cell(table,x.Description ?? "");
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text(
                        $"{data.Count} {T("Code",language)}");
            });
        });

        return document.GeneratePdf();
    }

    private static void Cell(
        TableDescriptor table,
        string text,
        bool bold = false)
    {
        var cell = table.Cell()
            .Border(1)
            .Padding(3);

        if(bold)
            cell.Text(text).Bold();
        else
            cell.Text(text);
    }
}