using ClosedXML.Excel;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryDashboard;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventorySystem.Infrastructure.Reports;

public sealed class DashboardExportService : IDashboardExportService {
    private readonly ISender _sender;

    public DashboardExportService(ISender sender)
    {
        _sender = sender;
    }

    private static bool Ar(string language) =>
        language.Equals("ar",StringComparison.OrdinalIgnoreCase);

    private static string T(string key,string language)
    {
        var ar = new Dictionary<string,string>
        {
            ["Title"]="تقرير لوحة معلومات الجرد",
            ["SessionInfo"]="معلومات الجلسة",
            ["SessionNumber"]="رقم الجلسة",
            ["Status"]="الحالة",
            ["InventoryType"]="نوع الجرد",
            ["InventoryDate"]="تاريخ الجرد",
            ["Branch"]="الفرع",
            ["Store"]="المخزن",
            ["BeforeFile"]="ملف الجرد السابق",
            ["AfterFile"]="ملف الجرد الحالي",
            ["Summary"]="ملخص الجرد",
            ["TotalItems"]="إجمالي المواد",
            ["Increase"]="زيادة",
            ["Decrease"]="نقص",
            ["Match"]="متطابق",
            ["NewlyCounted"]="مواد جديدة",
            ["FullyDepleted"]="مستنفدة بالكامل",
            ["PriceChanged"]="تغير السعر",
            ["UnitNotDefined"]="الوحدة غير معرفة",
            ["Financial"]="الملخص المالي",
            ["ValueBefore"]="إجمالي القيمة قبل",
            ["ValueAfter"]="إجمالي القيمة بعد",
            ["Difference"]="إجمالي الفرق",
            ["DifferencePercent"]="نسبة الفرق",
            ["Statuses"]="توزيع الحالات",
            ["Count"]="العدد",
            ["Percentage"]="النسبة",
            ["Attention"]="ملخص التنبيهات",
            ["Total"]="الإجمالي",
            ["TopDifferences"]="أعلى الفروقات في القيمة",
            ["ItemCode"]="رمز المادة",
            ["ItemName"]="اسم المادة",
            ["ValueDifference"]="فرق القيمة",
            ["Generated"]="تاريخ الإنشاء"
        };

        var en = new Dictionary<string,string>
        {
            ["Title"]="Inventory Dashboard Report",
            ["SessionInfo"]="Session Information",
            ["SessionNumber"]="Session Number",
            ["Status"]="Status",
            ["InventoryType"]="Inventory Type",
            ["InventoryDate"]="Inventory Date",
            ["Branch"]="Branch",
            ["Store"]="Store",
            ["BeforeFile"]="Before Inventory File",
            ["AfterFile"]="After Inventory File",
            ["Summary"]="Inventory Summary",
            ["TotalItems"]="Total Items",
            ["Increase"]="Increase",
            ["Decrease"]="Decrease",
            ["Match"]="Match",
            ["NewlyCounted"]="Newly Counted",
            ["FullyDepleted"]="Fully Depleted",
            ["PriceChanged"]="Price Changed",
            ["UnitNotDefined"]="Unit Not Defined",
            ["Financial"]="Financial Summary",
            ["ValueBefore"]="Total Value Before",
            ["ValueAfter"]="Total Value After",
            ["Difference"]="Total Difference",
            ["DifferencePercent"]="Difference %",
            ["Statuses"]="Status Distribution",
            ["Count"]="Count",
            ["Percentage"]="Percentage",
            ["Attention"]="Attention Summary",
            ["Total"]="Total",
            ["TopDifferences"]="Top Value Differences",
            ["ItemCode"]="Item Code",
            ["ItemName"]="Item Name",
            ["ValueDifference"]="Value Difference",
            ["Generated"]="Generated"
        };

        return (Ar(language) ? ar : en).GetValueOrDefault(key,key);
    }

    public async Task<byte[]> ExportExcelAsync(
        int sessionId,
        string language,
        CancellationToken cancellationToken)
    {
        var d = await _sender.Send(
            new GetInventoryDashboardQuery(sessionId),
            cancellationToken);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(
            Ar(language) ? "لوحة المعلومات" : "Dashboard");

        ws.RightToLeft = Ar(language);

        ws.Cell("A1").Value = T("Title",language);
        ws.Range("A1:F1").Merge();
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 18;
        ws.Cell("A1").Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        ws.Cell("A3").Value = T("SessionInfo",language);
        ws.Cell("A3").Style.Font.Bold = true;

        Row(ws,4,T("SessionNumber",language),d.Session.SessionNumber);
        Row(ws,5,T("Status",language),d.Session.Status);
        Row(ws,6,T("InventoryType",language),d.Session.InventoryType);
        Row(ws,7,T("InventoryDate",language),d.Session.InventoryDate);
        Row(ws,8,T("Branch",language),d.Session.BranchName);
        Row(ws,9,T("Store",language),d.Session.StoreName);
        Row(ws,10,T("BeforeFile",language),d.Session.BeforeFileName ?? "");
        Row(ws,11,T("AfterFile",language),d.Session.AfterFileName ?? "");

        ws.Cell("A13").Value = T("Summary",language);
        ws.Cell("A13").Style.Font.Bold = true;

        Row(ws,14,T("TotalItems",language),d.Summary.TotalItems);
        Row(ws,15,T("Increase",language),d.Summary.Increase);
        Row(ws,16,T("Decrease",language),d.Summary.Decrease);
        Row(ws,17,T("Match",language),d.Summary.Match);
        Row(ws,18,T("NewlyCounted",language),d.Summary.NewlyCounted);
        Row(ws,19,T("FullyDepleted",language),d.Summary.FullyDepleted);
        Row(ws,20,T("PriceChanged",language),d.Summary.PriceChanged);
        Row(ws,21,T("UnitNotDefined",language),d.Summary.UnitNotDefined);

        ws.Cell("D3").Value = T("Financial",language);
        ws.Cell("D3").Style.Font.Bold = true;

        Row(ws,4,T("ValueBefore",language),d.Financial.TotalValueBefore,"D","E");
        Row(ws,5,T("ValueAfter",language),d.Financial.TotalValueAfter,"D","E");
        Row(ws,6,T("Difference",language),d.Financial.TotalDifference,"D","E");
        Row(ws,7,T("DifferencePercent",language),d.Financial.DifferencePercentage,"D","E");

        ws.Cell("D9").Value = T("Statuses",language);
        ws.Cell("D9").Style.Font.Bold = true;

        Header(ws,10,"D",T("Status",language));
        Header(ws,10,"E",T("Count",language));
        Header(ws,10,"F",T("Percentage",language));

        var r = 11;
        foreach(var item in d.Statuses)
        {
            ws.Cell(r,4).Value = item.Status;
            ws.Cell(r,5).Value = item.Count;
            ws.Cell(r,6).Value = item.Percentage;
            r++;
        }

        ws.Cell("A24").Value = T("Attention",language);
        ws.Cell("A24").Style.Font.Bold = true;

        Row(ws,25,T("Total",language),d.Attention.Total);
        Row(ws,26,T("NewlyCounted",language),d.Attention.NewlyCounted);
        Row(ws,27,T("FullyDepleted",language),d.Attention.FullyDepleted);
        Row(ws,28,T("UnitNotDefined",language),d.Attention.UnitNotDefined);
        Row(ws,29,T("PriceChanged",language),d.Attention.PriceChanged);

        ws.Cell("D24").Value = T("TopDifferences",language);
        ws.Cell("D24").Style.Font.Bold = true;

        Header(ws,25,"D",T("ItemCode",language));
        Header(ws,25,"E",T("ItemName",language));
        Header(ws,25,"F",T("ValueDifference",language));

        r = 26;
        foreach(var item in d.TopValueDifferences)
        {
            ws.Cell(r,4).Value = item.ItemCode;
            ws.Cell(r,5).Value = item.ItemName;
            ws.Cell(r,6).Value = item.ValueDifference;
            r++;
        }

        ws.Columns().AdjustToContents();
        ws.Column("A").Width = 28;
        ws.Column("B").Width = 25;
        ws.Column("D").Width = 28;
        ws.Column("E").Width = 30;
        ws.Column("F").Width = 20;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportPdfAsync(
        int sessionId,
        string language,
        CancellationToken cancellationToken)
    {
        var d = await _sender.Send(
            new GetInventoryDashboardQuery(sessionId),
            cancellationToken);

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x =>
                    x.FontFamily("Arial").FontSize(10));

                page.Header().Column(c =>
                {
                    c.Item().AlignCenter()
                        .Text(T("Title",language))
                        .Bold().FontSize(20);

                    c.Item().AlignCenter()
                        .Text($"{T("SessionNumber",language)}: {d.Session.SessionNumber}")
                        .FontSize(12);
                });

                page.Content().Column(c =>
                {
                    Section(c,T("SessionInfo",language));

                    Table(c,
                        (T("SessionNumber",language),d.Session.SessionNumber),
                        (T("Status",language),d.Session.Status),
                        (T("InventoryType",language),d.Session.InventoryType),
                        (T("InventoryDate",language),d.Session.InventoryDate.ToString("yyyy-MM-dd")),
                        (T("Branch",language),d.Session.BranchName),
                        (T("Store",language),d.Session.StoreName));

                    Section(c,T("Summary",language));

                    Table(c,
                        (T("TotalItems",language),d.Summary.TotalItems.ToString()),
                        (T("Increase",language),d.Summary.Increase.ToString()),
                        (T("Decrease",language),d.Summary.Decrease.ToString()),
                        (T("Match",language),d.Summary.Match.ToString()),
                        (T("NewlyCounted",language),d.Summary.NewlyCounted.ToString()),
                        (T("FullyDepleted",language),d.Summary.FullyDepleted.ToString()),
                        (T("PriceChanged",language),d.Summary.PriceChanged.ToString()),
                        (T("UnitNotDefined",language),d.Summary.UnitNotDefined.ToString()));

                    Section(c,T("Financial",language));

                    Table(c,
                        (T("ValueBefore",language),d.Financial.TotalValueBefore.ToString("N2")),
                        (T("ValueAfter",language),d.Financial.TotalValueAfter.ToString("N2")),
                        (T("Difference",language),d.Financial.TotalDifference.ToString("N2")),
                        (T("DifferencePercent",language),d.Financial.DifferencePercentage.ToString("N2")));

                    Section(c,T("Statuses",language));

                    c.Item().Table(t =>
                    {
                        t.ColumnsDefinition(x =>
                        {
                            x.RelativeColumn(2);
                            x.RelativeColumn();
                            x.RelativeColumn();
                        });

                        Cell(t,T("Status",language),true);
                        Cell(t,T("Count",language),true);
                        Cell(t,T("Percentage",language),true);

                        foreach(var s in d.Statuses)
                        {
                            Cell(t,s.Status);
                            Cell(t,s.Count.ToString());
                            Cell(t,s.Percentage.ToString("N2"));
                        }
                    });

                    Section(c,T("Attention",language));

                    Table(c,
                        (T("Total",language),d.Attention.Total.ToString()),
                        (T("NewlyCounted",language),d.Attention.NewlyCounted.ToString()),
                        (T("FullyDepleted",language),d.Attention.FullyDepleted.ToString()),
                        (T("UnitNotDefined",language),d.Attention.UnitNotDefined.ToString()),
                        (T("PriceChanged",language),d.Attention.PriceChanged.ToString()));

                    Section(c,T("TopDifferences",language));

                    c.Item().Table(t =>
                    {
                        t.ColumnsDefinition(x =>
                        {
                            x.RelativeColumn();
                            x.RelativeColumn(2);
                            x.RelativeColumn();
                        });

                        Cell(t,T("ItemCode",language),true);
                        Cell(t,T("ItemName",language),true);
                        Cell(t,T("ValueDifference",language),true);

                        foreach(var item in d.TopValueDifferences)
                        {
                            Cell(t,item.ItemCode);
                            Cell(t,item.ItemName);
                            Cell(t,item.ValueDifference.ToString("N2"));
                        }
                    });
                });

                page.Footer()
                    .AlignCenter()
                    .Text($"{T("Generated",language)}: {DateTime.Now:yyyy-MM-dd HH:mm}");
            });
        });

        return document.GeneratePdf();
    }

    private static void Row(
        IXLWorksheet ws,
        int row,
        string label,
        object? value,
        string lc="A",
        string vc="B")
    {
        ws.Cell($"{lc}{row}").Value = label;
        ws.Cell($"{vc}{row}").Value = XLCellValue.FromObject(value);
    }

    private static void Header(
        IXLWorksheet ws,
        int row,
        string column,
        string value)
    {
        ws.Cell($"{column}{row}").Value = value;
        ws.Cell($"{column}{row}").Style.Font.Bold = true;
    }

    private static void Section(
        ColumnDescriptor c,
        string text)
    {
        c.Item().PaddingTop(10).PaddingBottom(5)
            .Text(text).Bold().FontSize(14);
    }

    private static void Table(
        ColumnDescriptor c,
        params (string Label,string Value)[] rows)
    {
        c.Item().Table(t =>
        {
            t.ColumnsDefinition(x =>
            {
                x.RelativeColumn();
                x.RelativeColumn();
            });

            foreach(var row in rows)
            {
                Cell(t,row.Label,true);
                Cell(t,row.Value);
            }
        });
    }

    private static void Cell(
        TableDescriptor table,
        string text,
        bool bold=false)
    {
        var cell = table.Cell()
            .Border(1)
            .Padding(5);

        if(bold)
            cell.Text(text).Bold();
        else
            cell.Text(text);
    }
}