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

        ws.RightToLeft = Ar(language);

        ws.Cell(1,1).Value = T("Title",language);
        ws.Range(1,1,1,17).Merge();
        ws.Cell(1,1).Style.Font.Bold = true;
        ws.Cell(1,1).Style.Font.FontSize = 18;

        var headers = new[]
        {
            "No","Code","Name1","Name2","Category","Unit",
            "BeforeQty","AfterQty","QtyDiff","QtyPercent",
            "BeforePrice","AfterPrice","BeforeValue","AfterValue",
            "ValueDiff","Status","Description"
        };

        for(var i=0;i<headers.Length;i++)
        {
            ws.Cell(3,i+1).Value = T(headers[i],language);
            ws.Cell(3,i+1).Style.Font.Bold = true;
        }

        var row = 4;
        var number = 1;

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
            ws.Cell(row,16).Value = x.Status;
            ws.Cell(row,17).Value = x.Description ?? "";
            row++;
        }

        ws.SheetView.FreezeRows(3);
        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

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