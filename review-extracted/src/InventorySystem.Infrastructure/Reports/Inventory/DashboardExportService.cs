using System.Globalization;
using ClosedXML.Excel;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Common.Localization;
using InventorySystem.Application.Features.Inventory.InventorySessions.Queries.GetInventoryDashboard;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventorySystem.Infrastructure.Reports.Inventory;

public sealed class DashboardExportService : IDashboardExportService
{
    private readonly ISender _sender;
    private const string Navy = "17324D";
    private const string Blue = "2563EB";
    private const string Pale = "EFF6FF";
    private const string Light = "F3F6FA";
    private const string Green = "15803D";
    private const string Red = "B91C1C";
    private static readonly CultureInfo NumberCulture = CultureInfo.GetCultureInfo("en-US");

    public DashboardExportService(ISender sender) => _sender = sender;
    private static bool Ar(string? language) => string.Equals(language,"ar",StringComparison.OrdinalIgnoreCase) || (language?.StartsWith("ar-",StringComparison.OrdinalIgnoreCase) ?? false);

    private static string T(string key,bool ar) => LocalizationService.Get("DashboardExport." + key,ar ? "ar" : "en");

    private static string Label(string? value,bool ar)
    {
        if(string.IsNullOrWhiteSpace(value)) return "—";
        var key = value.Trim().Replace(" ","").Replace("_","").Replace("-","");
        return T(key,ar);
    }
    private static string StatusLabel(string? value,bool ar)
    {
        if(string.IsNullOrWhiteSpace(value)) return "—";
        var key = value.Trim().Replace(" ","").Replace("_","").Replace("-","");
        return T("Status" + key,ar);
    }
    private static string N(decimal value) => value.ToString("N2",NumberCulture);
    private static string N(int value) => value.ToString("N0",NumberCulture);
    private static string P(decimal value) => value.ToString("N2",NumberCulture) + "%";
    private static string Date(DateTime value) => value.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture);
    private static string PdfColor(decimal value) => value < 0 ? Red : value > 0 ? Green : Navy;

    public async Task<byte[]> ExportExcelAsync(int sessionId,string language,CancellationToken cancellationToken)
    {
        var d = await _sender.Send(new GetInventoryDashboardQuery(sessionId),cancellationToken);
        var ar = Ar(language);
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(ar ? "لوحة المعلومات" : "Dashboard");
        ws.RightToLeft = ar;
        ws.Style.Font.FontName = "Arial";
        ws.Style.Font.FontSize = 11;
        ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Style.Alignment.Horizontal = ar ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left;
        ws.SheetView.FreezeRows(3);
        ws.Column("A").Width = 3;
        ws.Column("B").Width = 31;
        ws.Column("C").Width = 28;
        ws.Column("D").Width = 4;
        ws.Column("E").Width = 31;
        ws.Column("F").Width = 31;
        ws.Column("G").Width = 25;
        ws.Column("H").Width = 3;
        ws.Range("B2:G3").Merge();
        var title = ws.Cell("B2");
        title.Value = T("Title",ar);
        title.Style.Fill.BackgroundColor = XLColor.FromHtml("#" + Navy);
        title.Style.Font.FontColor = XLColor.White;
        title.Style.Font.Bold = true;
        title.Style.Font.FontSize = 19;
        title.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Rows(2,3).Height = 23;
        ws.Range("B4:G4").Merge();
        ws.Cell("B4").Value = $"{T("SessionNumber",ar)}: {d.Session.SessionNumber}";
        ws.Cell("B4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell("B4").Style.Font.FontColor = XLColor.FromHtml("#" + Blue);
        ws.Row(4).Height = 25;

        ExcelSection(ws,6,T("SessionInfo",ar));
        ExcelPair(ws,7,T("SessionNumber",ar),d.Session.SessionNumber,T("Status",ar),Label(d.Session.Status,ar));
        ExcelPair(ws,8,T("InventoryType",ar),Label(d.Session.InventoryType,ar),T("InventoryDate",ar),Date(d.Session.InventoryDate));
        ExcelPair(ws,9,T("Branch",ar),d.Session.BranchName,T("Store",ar),d.Session.StoreName);
        ExcelPair(ws,10,T("BeforeFile",ar),d.Session.BeforeFileName ?? "—",T("AfterFile",ar),d.Session.AfterFileName ?? "—");
        ws.Row(10).Height = 34;
        ws.Range("C10:C10").Style.Alignment.WrapText = true;
        ws.Range("F10:F10").Style.Alignment.WrapText = true;

        ExcelSection(ws,12,T("Summary",ar));
        ExcelPair(ws,13,T("TotalItems",ar),d.Summary.TotalItems,T("Match",ar),d.Summary.Match);
        ExcelPair(ws,14,T("Increase",ar),d.Summary.Increase,T("Decrease",ar),d.Summary.Decrease);
        ExcelPair(ws,15,T("NewlyCounted",ar),d.Summary.NewlyCounted,T("FullyDepleted",ar),d.Summary.FullyDepleted);
        ExcelPair(ws,16,T("PriceChanged",ar),d.Summary.PriceChanged,T("UnitNotDefined",ar),d.Summary.UnitNotDefined);

        ExcelSection(ws,18,T("Quantity",ar));
        ExcelPair(ws,19,T("QuantityBefore",ar),d.InventoryQuantity.TotalQuantityBefore,T("QuantityAfter",ar),d.InventoryQuantity.TotalQuantityAfter);
        ExcelPair(ws,20,T("QuantityIncrease",ar),d.InventoryQuantity.QuantityIncrease,T("QuantityDecrease",ar),d.InventoryQuantity.QuantityDecrease);
        ExcelPair(ws,21,T("QuantityDifference",ar),d.InventoryQuantity.QuantityDifference,null,null);

        ExcelSection(ws,23,T("Financial",ar));
        ExcelPair(ws,24,T("ValueBefore",ar),d.Financial.TotalValueBefore,T("ValueAfter",ar),d.Financial.TotalValueAfter);
        ExcelPair(ws,25,T("Difference",ar),d.Financial.TotalDifference,T("DifferencePercent",ar),P(d.Financial.DifferencePercentage));
        ExcelDifference(ws,"C21",d.InventoryQuantity.QuantityDifference);
        ExcelDifference(ws,"C25",d.Financial.TotalDifference);
        ExcelDifference(ws,"F25",d.Financial.DifferencePercentage);

        ExcelSection(ws,27,T("Statuses",ar));
        ExcelTableHeader(ws,28,T("Status",ar),T("Count",ar),T("Percentage",ar));
        var row = 29;
        foreach(var item in d.Statuses)
        {
            ExcelTableRow(ws,row++,StatusLabel(item.Status,ar),item.Count,P(item.Percentage));
        }
        if(d.Statuses.Count == 0) ExcelTableRow(ws,row++,T("NoData",ar),"","");

        row += 2;
        ExcelSection(ws,row++,T("Attention",ar));
        ExcelPair(ws,row++,T("Total",ar),d.Attention.Total,T("NewlyCounted",ar),d.Attention.NewlyCounted);
        ExcelPair(ws,row++,T("FullyDepleted",ar),d.Attention.FullyDepleted,T("PriceChanged",ar),d.Attention.PriceChanged);
        ExcelPair(ws,row++,T("UnitNotDefined",ar),d.Attention.UnitNotDefined,null,null);
        ws.Range($"B{row}:G{row}").Merge();
        ws.Cell(row,2).Value = T("AttentionNote",ar);
        ws.Cell(row,2).Style.Font.Italic = true;
        ws.Cell(row,2).Style.Font.FontColor = XLColor.Gray;
        row += 2;

        ExcelSection(ws,row++,T("TopDifferences",ar));
        ExcelTableHeader(ws,row++,T("ItemCode",ar),T("ItemName",ar),T("ValueDifference",ar));
        if(d.TopValueDifferences.Count == 0) ExcelTableRow(ws,row++,T("NoData",ar),"","");
        foreach(var item in d.TopValueDifferences)
        {
            ExcelTableRow(ws,row,item.ItemCode,item.ItemName,item.ValueDifference);
            ws.Cell(row,2).Style.NumberFormat.Format = "@";
            ExcelDifference(ws,$"G{row}",item.ValueDifference);
            row++;
        }
        row += 2;
        ws.Range($"B{row}:G{row}").Merge();
        ws.Cell(row,2).Value = $"{T("Generated",ar)}: {DateTime.Now:dd/MM/yyyy HH:mm}";
        ws.Cell(row,2).Style.Font.FontColor = XLColor.Gray;
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
        ws.PageSetup.FitToPages(1,0);
        ws.PageSetup.SetRowsToRepeatAtTop(2,4);
        ws.PageSetup.PrintAreas.Add($"B2:G{row}");
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void ExcelSection(IXLWorksheet ws,int row,string title)
    {
        var range = ws.Range(row,2,row,7);
        range.Merge();
        range.Style.Fill.BackgroundColor = XLColor.FromHtml("#" + Navy);
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.Bold = true;
        range.Style.Font.FontSize = 12;
        range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Cell(row,2).Value = title;
        ws.Row(row).Height = 28;
    }
    private static void ExcelPair(IXLWorksheet ws,int row,string label1,object? value1,string? label2,object? value2)
    {
        ExcelLabel(ws,row,2,label1);
        ExcelValue(ws,row,3,value1);
        if(label2 != null)
        {
            ExcelLabel(ws,row,5,label2);
            ExcelValue(ws,row,6,value2);
        }
        ws.Row(row).Height = Math.Max(ws.Row(row).Height,26);
    }
    private static void ExcelLabel(IXLWorksheet ws,int row,int col,string text)
    {
        var cell = ws.Cell(row,col);
        cell.Value = text;
        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#" + Light);
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontColor = XLColor.FromHtml("#" + Navy);
        cell.Style.Alignment.WrapText = true;
    }
    private static void ExcelValue(IXLWorksheet ws,int row,int col,object? value)
    {
        var cell = ws.Cell(row,col);
        if(col == 6) ws.Range(row,6,row,7).Merge();
        cell.Value = XLCellValue.FromObject(value ?? "—");
        cell.Style.Alignment.WrapText = true;
        if(value is int || value is decimal || value is long)
        {
            cell.Style.NumberFormat.Format = "#,##0.##;[Red](#,##0.##)";
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
    }
    private static void ExcelDifference(IXLWorksheet ws,string address,decimal value)
    {
        ws.Cell(address).Style.Font.Bold = true;
        ws.Cell(address).Style.Font.FontColor = XLColor.FromHtml("#" + PdfColor(value));
    }
    private static void ExcelTableHeader(IXLWorksheet ws,int row,string a,string b,string c)
    {
        ws.Range(row,2,row,4).Merge();
        ws.Range(row,5,row,6).Merge();
        ws.Cell(row,2).Value = a;
        ws.Cell(row,5).Value = b;
        ws.Cell(row,7).Value = c;
        var range = ws.Range(row,2,row,7);
        range.Style.Fill.BackgroundColor = XLColor.FromHtml("#" + Blue);
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.Bold = true;
        ws.Row(row).Height = 27;
    }
    private static void ExcelTableRow(IXLWorksheet ws,int row,object? a,object? b,object? c)
    {
        ws.Range(row,2,row,4).Merge();
        ws.Range(row,5,row,6).Merge();
        ws.Cell(row,2).Value = XLCellValue.FromObject(a ?? "");
        ws.Cell(row,5).Value = XLCellValue.FromObject(b ?? "");
        ws.Cell(row,7).Value = XLCellValue.FromObject(c ?? "");
        if(row % 2 == 0) ws.Range(row,2,row,7).Style.Fill.BackgroundColor = XLColor.FromHtml("#" + Light);
        ws.Range(row,2,row,7).Style.Alignment.WrapText = true;
        ws.Row(row).Height = 26;
        if(b is int || b is decimal) ws.Cell(row,5).Style.NumberFormat.Format = "#,##0.##";
        if(c is decimal) ws.Cell(row,7).Style.NumberFormat.Format = "#,##0.00;[Red](#,##0.00)";
    }

    public async Task<byte[]> ExportPdfAsync(int sessionId,string language,CancellationToken cancellationToken)
    {
        var d = await _sender.Send(new GetInventoryDashboardQuery(sessionId),cancellationToken);
        var ar = Ar(language);
        QuestPDF.Settings.License = LicenseType.Community;
        var document = Document.Create(root =>
        {
            root.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9).FontColor("#" + Navy));
                page.Header().Column(h =>
                {
                    h.Item().Background("#" + Navy).Padding(14).AlignCenter().Text(T("Title",ar)).FontColor(Colors.White).Bold().FontSize(17);
                    h.Item().PaddingTop(7).PaddingBottom(10).AlignCenter().Text($"{T("SessionNumber",ar)}: {d.Session.SessionNumber}").FontSize(10).FontColor("#" + Blue);
                });
                page.Content().Column(c =>
                {
                    PdfSection(c,T("SessionInfo",ar),ar);
                    PdfPairs(c,ar,
                        (T("SessionNumber",ar),d.Session.SessionNumber),(T("Status",ar),Label(d.Session.Status,ar)),
                        (T("InventoryType",ar),Label(d.Session.InventoryType,ar)),(T("InventoryDate",ar),Date(d.Session.InventoryDate)),
                        (T("Branch",ar),d.Session.BranchName),(T("Store",ar),d.Session.StoreName),
                        (T("BeforeFile",ar),d.Session.BeforeFileName ?? "—"),(T("AfterFile",ar),d.Session.AfterFileName ?? "—"));
                    PdfSection(c,T("Summary",ar),ar);
                    PdfPairs(c,ar,
                        (T("TotalItems",ar),N(d.Summary.TotalItems)),(T("Match",ar),N(d.Summary.Match)),
                        (T("Increase",ar),N(d.Summary.Increase)),(T("Decrease",ar),N(d.Summary.Decrease)),
                        (T("NewlyCounted",ar),N(d.Summary.NewlyCounted)),(T("FullyDepleted",ar),N(d.Summary.FullyDepleted)),
                        (T("PriceChanged",ar),N(d.Summary.PriceChanged)),(T("UnitNotDefined",ar),N(d.Summary.UnitNotDefined)));
                    PdfSection(c,T("Quantity",ar),ar);
                    PdfPairs(c,ar,
                        (T("QuantityBefore",ar),N(d.InventoryQuantity.TotalQuantityBefore)),
                        (T("QuantityAfter",ar),N(d.InventoryQuantity.TotalQuantityAfter)),
                        (T("QuantityIncrease",ar),N(d.InventoryQuantity.QuantityIncrease)),
                        (T("QuantityDecrease",ar),N(d.InventoryQuantity.QuantityDecrease)),
                        (T("QuantityDifference",ar),N(d.InventoryQuantity.QuantityDifference)));
                    PdfSection(c,T("Financial",ar),ar);
                    PdfPairs(c,ar,
                        (T("ValueBefore",ar),N(d.Financial.TotalValueBefore)),
                        (T("ValueAfter",ar),N(d.Financial.TotalValueAfter)),
                        (T("Difference",ar),N(d.Financial.TotalDifference)),
                        (T("DifferencePercent",ar),P(d.Financial.DifferencePercentage)));
                    c.Item().PageBreak();
                    PdfSection(c,T("Statuses",ar),ar);
                    PdfThreeColumn(c,ar,T("Status",ar),T("Count",ar),T("Percentage",ar),
                        d.Statuses.Select(x => (StatusLabel(x.Status,ar),N(x.Count),P(x.Percentage))).ToList());
                    PdfSection(c,T("Attention",ar),ar);
                    PdfPairs(c,ar,
                        (T("Total",ar),N(d.Attention.Total)),(T("NewlyCounted",ar),N(d.Attention.NewlyCounted)),
                        (T("FullyDepleted",ar),N(d.Attention.FullyDepleted)),(T("PriceChanged",ar),N(d.Attention.PriceChanged)),
                        (T("UnitNotDefined",ar),N(d.Attention.UnitNotDefined)));
                    c.Item().PaddingTop(4).Text(T("AttentionNote",ar)).Italic().FontSize(8).FontColor(Colors.Grey.Darken2);
                    PdfSection(c,T("TopDifferences",ar),ar);
                    PdfThreeColumn(c,ar,T("ItemCode",ar),T("ItemName",ar),T("ValueDifference",ar),
                        d.TopValueDifferences.Select(x => (x.ItemCode,x.ItemName,N(x.ValueDifference))).ToList());
                });
                page.Footer().BorderTop(1).BorderColor("#D9E2EC").PaddingTop(6).Row(r =>
                {
                    r.RelativeItem().Text($"{T("Generated",ar)}: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8);
                    r.AutoItem().Text(t =>
                    {
                        t.Span(T("Page",ar) + " ");
                        t.CurrentPageNumber();
                        t.Span(" " + T("Of",ar) + " ");
                        t.TotalPages();
                    });
                });
            });
        });
        return document.GeneratePdf();
    }
    private static void PdfSection(ColumnDescriptor c,string title,bool ar)
    {
        c.Item().PaddingTop(12).PaddingBottom(5).Background("#" + Pale).Padding(8)
            .AlignLeft().Text(title).Bold().FontSize(11).FontColor("#" + Navy);
    }
    private static void PdfPairs(ColumnDescriptor c,bool ar,params (string Label,string Value)[] pairs)
    {
        c.Item().Table(t =>
        {
            t.ColumnsDefinition(cols => { cols.RelativeColumn(2); cols.RelativeColumn(3); cols.RelativeColumn(2); cols.RelativeColumn(3); });
            for(var i = 0;i < pairs.Length;i += 2)
            {
                var first = pairs[i];
                (string Label,string Value) second = i + 1 < pairs.Length ? pairs[i + 1] : (Label:"",Value:"");
                if(ar)
                {
                    PdfCell(t,second.Value,false); PdfCell(t,second.Label,true);
                    PdfCell(t,first.Value,false); PdfCell(t,first.Label,true);
                }
                else
                {
                    PdfCell(t,first.Label,true); PdfCell(t,first.Value,false);
                    PdfCell(t,second.Label,true); PdfCell(t,second.Value,false);
                }
            }
        });
    }
    private static void PdfThreeColumn(ColumnDescriptor c,bool ar,string a,string b,string d,List<(string,string,string)> rows)
    {
        c.Item().Table(t =>
        {
            t.ColumnsDefinition(cols => { cols.RelativeColumn(2); cols.RelativeColumn(4); cols.RelativeColumn(2); });
            if(ar) { PdfHeader(t,d); PdfHeader(t,b); PdfHeader(t,a); }
            else { PdfHeader(t,a); PdfHeader(t,b); PdfHeader(t,d); }
            foreach(var item in rows)
            {
                if(ar) { PdfCell(t,item.Item3,false); PdfCell(t,item.Item2,false); PdfCell(t,item.Item1,false); }
                else { PdfCell(t,item.Item1,false); PdfCell(t,item.Item2,false); PdfCell(t,item.Item3,false); }
            }
            if(rows.Count == 0)
            {
                PdfCell(t,"—",false); PdfCell(t,"—",false); PdfCell(t,"—",false);
            }
        });
    }
    private static void PdfHeader(TableDescriptor table,string text) => table.Cell().Background("#" + Blue)
        .Padding(6).AlignCenter().Text(text).Bold().FontColor(Colors.White).FontSize(9);
    private static void PdfCell(TableDescriptor t,string text,bool label)
    {
        var cell = t.Cell().BorderBottom(0.5f).BorderColor("#D9E2EC").Padding(6);
        if(label) cell.Background("#" + Light).Text(text).Bold().FontSize(8);
        else cell.Text(text).FontSize(8);
    }
}
