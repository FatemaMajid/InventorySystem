using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using InventorySystem.Application.Common.Localization;
using InventorySystem.Application.DTOs.FinancialAnalysis.DashboardAggregate;
using InventorySystem.Application.Features.FinancialAnalysis.Dashboard.Interfaces;
using InventorySystem.Application.Features.FinancialAnalysis.Dashboard.Queries.GetFinancialAnalysisDashboard;
using MediatR;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventorySystem.Infrastructure.Reports.FinancialAnalysis;

public sealed class FinancialDashboardPdfExportService
    : IFinancialDashboardPdfExportService
{
    private static readonly object FontLock = new();
    private static bool _fontRegistered;

    private readonly IMediator _mediator;

    public FinancialDashboardPdfExportService(
        IMediator mediator)
    {
        _mediator =
            mediator;
    }

    private static readonly Dictionary<string,string> SectionKeys =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["sales"] = LocalizationKeys.FinancialAnalysis.Sales,
            ["inventory"] = LocalizationKeys.FinancialAnalysis.Inventory,
            ["expenses"] = LocalizationKeys.FinancialAnalysis.Expenses,
            ["purchases"] = LocalizationKeys.FinancialAnalysis.Purchases,
            ["profit"] = LocalizationKeys.FinancialAnalysis.ProfitLoss,
            ["customers"] = LocalizationKeys.FinancialAnalysis.CustomersEmployees,
            ["debt"] = LocalizationKeys.FinancialAnalysis.BranchDebt,
            ["cash"] = LocalizationKeys.FinancialAnalysis.Cash,
            ["suppliers"] = LocalizationKeys.FinancialAnalysis.Suppliers,
            ["capital"] = LocalizationKeys.FinancialAnalysis.Capital,
            ["transfers"] = LocalizationKeys.FinancialAnalysis.Transfers,
            ["settlements"] = LocalizationKeys.FinancialAnalysis.Settlements
        };

    private static readonly Dictionary<string,string> AliasKeys =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["branchCode"] = LocalizationKeys.FinancialAnalysis.BranchCode,
            ["branchName"] = LocalizationKeys.FinancialAnalysis.BranchName,

            ["accountCode"] = LocalizationKeys.FinancialAnalysis.AccountCode,
            ["accountName"] = LocalizationKeys.FinancialAnalysis.AccountName,

            ["parentAccountCode"] = LocalizationKeys.FinancialAnalysis.AccountCode,
            ["parentAccountName"] = LocalizationKeys.FinancialAnalysis.AccountName,

            ["customerAccountCode"] = LocalizationKeys.FinancialAnalysis.AccountCode,
            ["customerAccountName"] = LocalizationKeys.FinancialAnalysis.AccountName,

            ["employeeAccountCode"] = LocalizationKeys.FinancialAnalysis.AccountCode,
            ["employeeAccountName"] = LocalizationKeys.FinancialAnalysis.AccountName,

            ["supplierAccountCode"] = LocalizationKeys.FinancialAnalysis.AccountCode,
            ["supplierAccountName"] = LocalizationKeys.FinancialAnalysis.AccountName,

            ["cashGroupAccountCode"] = LocalizationKeys.FinancialAnalysis.AccountCode,
            ["cashGroupAccountName"] = LocalizationKeys.FinancialAnalysis.AccountName,

            ["male"] = LocalizationKeys.FinancialAnalysis.Male,
            ["female"] = LocalizationKeys.FinancialAnalysis.Female,

            ["details"] = LocalizationKeys.FinancialAnalysis.Details,
            ["transactions"] = LocalizationKeys.FinancialAnalysis.Transactions,
            ["customerDetails"] = LocalizationKeys.FinancialAnalysis.Customers,
            ["employeeDetails"] = LocalizationKeys.FinancialAnalysis.Employees,
            ["cashboxes"] = LocalizationKeys.FinancialAnalysis.Cash,
            ["suppliers"] = LocalizationKeys.FinancialAnalysis.Suppliers,

            ["hasFile"] = LocalizationKeys.FinancialAnalysis.FileAvailable,
            ["totalPurchases"] = LocalizationKeys.FinancialAnalysis.TotalPurchases
        };

    private static readonly Dictionary<string,string> AutomaticKeys =
        typeof(LocalizationKeys.FinancialAnalysis)
            .GetFields(
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy)
            .Where(
                x =>
                    x.IsLiteral &&
                    !x.IsInitOnly &&
                    x.FieldType == typeof(string))
            .Select(
                x =>
                    new
                    {
                        Name = x.Name,
                        Key = x.GetRawConstantValue()?.ToString()
                              ?? string.Empty
                    })
            .Where(
                x =>
                    !string.IsNullOrWhiteSpace(
                        x.Key))
            .SelectMany(
                x =>
                    new[]
                    {
                        new
                        {
                            x.Name,
                            x.Key
                        },
                        new
                        {
                            Name =
                                char.ToLowerInvariant(
                                    x.Name[0]) +
                                x.Name[1..],
                            x.Key
                        }
                    })
            .GroupBy(
                x => x.Name,
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                x => x.Key,
                x => x.First().Key,
                StringComparer.OrdinalIgnoreCase);

    public async Task<byte[]> ExportAsync(
        FinancialAnalysisDashboardDto dashboard,
        string language,
        CancellationToken cancellationToken)
    {
        language =
            string.Equals(
                language,
                "en",
                StringComparison.OrdinalIgnoreCase)
                ? "en"
                : "ar";

        QuestPDF.Settings.License =
            LicenseType.Evaluation;

        RegisterTimesNewRoman();

        var isArabic =
            language == "ar";


var comparisonDashboard =
                dashboard.IsAllBranches
                    ? dashboard
                    : await _mediator.Send(
                        new GetFinancialAnalysisDashboardQuery(
                            dashboard.FinancialPeriodId,
                            null,
                            dashboard.SelectedSections,
                            false),
                        cancellationToken)
                      ?? dashboard;

        var document =
            Document.Create(
                document =>
                {
                    document.Page(
                        page =>
                        {
                            page.Size(
                                PageSizes.A4.Portrait());

                            page.Margin(22);

                            page.PageColor(
                                Colors.White);

                            if(isArabic)
                                page.ContentFromRightToLeft();
                            else
                                page.ContentFromLeftToRight();

                            page.DefaultTextStyle(
                                style =>
                                    style
                                        .FontFamily("Times New Roman")
                                        .FontSize(12)
                                        .FontColor(
                                            Colors.Black)
                                            .LineHeight(1.2f)
                                            .SemiBold());


                            page.Header()
                                .Element(
                                    x =>
                                        ComposeHeader(
                                            x,
                                            dashboard,
                                            language));

                            page.Content()
                                .PaddingTop(12)
                                .Column(
                                    column =>
                                    {
                                        column.Spacing(10);

                                        ComposeSummary(
                                            column,
                                            dashboard,
                                            language);

                        // Executive PDF:
                        // raw recursive section rendering is intentionally
                        // omitted from the main report.
                        // The comparison analysis below is the primary
                        // management-facing financial report.


column.Item()
                                            .PageBreak();


ComposeVisualAnalysis(
                                            column,
                                            comparisonDashboard,
                                            dashboard,
                                            language);
                        // ====================================================
                        // FULL FINANCIAL DETAILS APPENDIX
                        // Preserve every original section and its deep details
                        // after the executive comparison analysis.
                        // ====================================================

                        column.Item()
                            .PageBreak();

                        column.Item()
                            .Element(
                                x =>
                                    SectionHeader(
                                        x,
                                        T(
                                            LocalizationKeys.FinancialAnalysis.Details,
                                            language)));

                        var appendixFirstSection =
                            true;

                        foreach(
                            var sectionName
                            in dashboard.SelectedSections)
                        {
                            if(
                                !dashboard.Sections.TryGetValue(
                                    sectionName,
                                    out var section) ||
                                !section.HasValue)
                            {
                                continue;
                            }

                            if(!appendixFirstSection)
                            {
                                column.Item()
                                    .PageBreak();
                            }

                            appendixFirstSection =
                                false;

                            column.Item()
                                .PaddingTop(8)
                                .Element(
                                    x =>
                                        SectionHeader(
                                            x,
                                            SectionTitle(
                                                sectionName,
                                                language)));

                            RenderElement(
                                column,
                                sectionName,
                                section.Value,
                                language,
                                true);
                        }


                                    });

                            page.Footer()
                                .AlignCenter()
                                .Text(
                                    text =>
                                    {
                                        text.CurrentPageNumber();
                                        text.Span(" / ");
                                        text.TotalPages();
                                    });
                        });
                });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(
        IContainer container,
        FinancialAnalysisDashboardDto dashboard,
        string language)
    {
        container
            .BorderBottom(1)
            .BorderColor(
                Colors.Blue.Darken2)
            .PaddingBottom(8)
            .Column(
                column =>
                {
                    column.Item()
                        .AlignCenter()
                        .Text(
                            T(
                                LocalizationKeys.FinancialAnalysis.Dashboard,
                                language))
                        .Bold()
                        .FontSize(18)
                        .FontColor(
                            Colors.Blue.Darken3);

                    column.Item()
                        .PaddingTop(4)
                        .AlignCenter()
                        .Text(
                            $"{dashboard.StartDate:dd/MM/yyyy} - {dashboard.EndDate:dd/MM/yyyy}");

                    column.Item()
                        .PaddingTop(3)
                        .AlignCenter()
                        .Text(
                            dashboard.IsAllBranches
                                ? T(
                                    LocalizationKeys.FinancialAnalysis.AllBranches,
                                    language)
                                : $"{dashboard.SelectedBranch?.BranchCode} - {dashboard.SelectedBranch?.BranchName}")
                        .Bold();
                });
    }

    private static void ComposeSummary(
        ColumnDescriptor column,
        FinancialAnalysisDashboardDto dashboard,
        string language)
    {
        column.Item()
            .Element(
                x =>
                    SectionHeader(
                        x,
                        T(
                            LocalizationKeys.FinancialAnalysis.Dashboard,
                            language)));

        AddKeyValue(
            column,
            T(
                LocalizationKeys.FinancialAnalysis.FinancialPeriod,
                language),
            dashboard.FinancialPeriodId.ToString(
                CultureInfo.InvariantCulture));

        AddKeyValue(
            column,
            T(
                LocalizationKeys.FinancialAnalysis.Status,
                language),
            LocalizeNamedValue(
                dashboard.Status.ToString(),
                language));

        AddKeyValue(
            column,
            T(
                LocalizationKeys.FinancialAnalysis.ActiveBranches,
                language),
            dashboard.ActiveBranches.ToString(
                "N0",
                CultureInfo.InvariantCulture));





    }



    private static void AddChartLegend(
        ColumnDescriptor column,
        string label,
        int count,
        decimal percentage)
    {
        column.Item()
            .BorderBottom(0.5f)
            .BorderColor(
                Colors.Grey.Lighten2)
            .PaddingVertical(7)
            .Column(
                item =>
                {
                    item.Item()
                        .Text(
                            label)
                        .Bold()
                        .FontSize(11);

                    item.Item()
                        .PaddingTop(2)
                        .Text(
                            $"{count:N0} - {percentage:N2}%")
                        .FontSize(11);
                });
    }

    private static void AddBarMetric(
        ColumnDescriptor column,
        ChartMetric metric,
        decimal max)
    {
        var absolute =
            Math.Abs(
                metric.Value);

        var width =
            max <= 0
                ? 0f
                : (float)(
                    absolute /
                    max *
                    300m);

        width =
            Math.Clamp(
                width,
                2f,
                300f);

        column.Item()
            .PaddingTop(10)
            .Column(
                metricColumn =>
                {
                    metricColumn.Item()
                        .Row(
                            row =>
                            {
                                row.RelativeItem()
                                    .Text(
                                        metric.Label)
                                    .Bold()
                                    .FontSize(10);

                                row.ConstantItem(150)
                                    .AlignRight()
                                    .Text(
                                        metric.Value.ToString(
                                            "#,##0.00",
                                            CultureInfo.InvariantCulture))
                                    .Bold()
                                    .FontSize(10);
                            });

                    metricColumn.Item()
                        .PaddingTop(4)
                        .Height(16)
                        .Row(
                            bar =>
                            {
                                bar.ConstantItem(
                                        width)
                                    .Background(
                                        metric.Value < 0
                                            ? Colors.Red.Darken1
                                            : Colors.Blue.Darken2);

                                bar.RelativeItem()
                                    .Background(
                                        Colors.Grey.Lighten3);
                            });
                });
    }

    private static List<ChartMetric> BuildFinancialMetrics(
        FinancialAnalysisDashboardDto dashboard,
        string language)
    {
        var result =
            new List<ChartMetric>();

        AddMetric(
            result,
            dashboard,
            "sales",
            language,
            new[]
            {
                "netSales",
                "allBranchesNetSales",
                "totalSales"
            });

        AddMetric(
            result,
            dashboard,
            "inventory",
            language,
            new[]
            {
                "closingInventory",
                "allBranchesClosingInventory",
                "inventoryValue"
            });

        AddMetric(
            result,
            dashboard,
            "expenses",
            language,
            new[]
            {
                "totalExpenses",
                "allBranchesExpenses"
            });

        AddMetric(
            result,
            dashboard,
            "purchases",
            language,
            new[]
            {
                "totalPurchases",
                "allBranchesPurchases"
            });

        AddMetric(
            result,
            dashboard,
            "profit",
            language,
            new[]
            {
                "signedValue",
                "resultValue",
                "overallResult",
                "profitLoss"
            });

        AddMetric(
            result,
            dashboard,
            "debt",
            language,
            new[]
            {
                "totalDebt",
                "allBranchesTotalDebt"
            });

        AddMetric(
            result,
            dashboard,
            "cash",
            language,
            new[]
            {
                "totalCash",
                "allBranchesCash"
            });

        AddMetric(
            result,
            dashboard,
            "suppliers",
            language,
            new[]
            {
                "suppliersTotal",
                "totalLiabilities"
            });

        AddMetric(
            result,
            dashboard,
            "capital",
            language,
            new[]
            {
                "capitalValue",
                "totalCapital"
            });

        AddMetric(
            result,
            dashboard,
            "transfers",
            language,
            new[]
            {
                "totalTransfer",
                "totalTransfers"
            });

        return result
            .Where(
                x =>
                    x.Value != 0)
            .ToList();
    }

    private static void AddMetric(
        List<ChartMetric> result,
        FinancialAnalysisDashboardDto dashboard,
        string sectionName,
        string language,
        IReadOnlyCollection<string> propertyNames)
    {
        if(
            !dashboard.Sections.TryGetValue(
                sectionName,
                out var section) ||
            !section.HasValue)
        {
            return;
        }

        foreach(var propertyName in propertyNames)
        {
            if(
                TryFindNumber(
                    section.Value,
                    propertyName,
                    out var value))
            {
                result.Add(
                    new ChartMetric(
                        SectionTitle(
                            sectionName,
                            language),
                        value));

                return;
            }
        }
    }

    private static bool TryFindNumber(
        JsonElement element,
        string propertyName,
        out decimal value)
    {
        value = 0;

        if(
            element.ValueKind ==
                JsonValueKind.Object)
        {
            foreach(
                var property
                in element.EnumerateObject())
            {
                if(
                    property.Name.Equals(
                        propertyName,
                        StringComparison.OrdinalIgnoreCase) &&
                    property.Value.ValueKind ==
                        JsonValueKind.Number &&
                    property.Value.TryGetDecimal(
                        out value))
                {
                    return true;
                }
            }

            foreach(
                var property
                in element.EnumerateObject())
            {
                if(
                    property.Value.ValueKind ==
                        JsonValueKind.Object &&
                    TryFindNumber(
                        property.Value,
                        propertyName,
                        out value))
                {
                    return true;
                }
            }
        }

        return false;
    }



    private sealed record ChartMetric(
        string Label,
        decimal Value);
        private static void ComposeVisualAnalysis(
        ColumnDescriptor column,
        FinancialAnalysisDashboardDto comparisonDashboard,
        FinancialAnalysisDashboardDto requestedDashboard,
        string language)
    {
        var profiles =
            BuildComparisonProfiles(
                comparisonDashboard,
                language);

        if(profiles.Count == 0)
            return;

        ComposeExecutiveOverview(
            column,
            profiles,
            comparisonDashboard,
            language);

        for(
            var profileIndex = 0;
            profileIndex < profiles.Count;
            profileIndex++)
        {
            column.Item()
                .PageBreak();

            ComposeComparisonProfile(
                column,
                profiles[profileIndex],
                requestedDashboard,
                language);
        }
    }

    private static void ComposeExecutiveOverview(
        ColumnDescriptor column,
        IReadOnlyList<ComparisonProfile> profiles,
        FinancialAnalysisDashboardDto dashboard,
        string language)
    {
        column.Item()
            .Element(
                x =>
                    SectionHeader(
                        x,
                        T(
                            LocalizationKeys.FinancialAnalysis.Dashboard,
                            language)));

        column.Item()
            .PaddingTop(5)
            .AlignCenter()
            .Text(
                $"{dashboard.StartDate:dd/MM/yyyy} - {dashboard.EndDate:dd/MM/yyyy}")
            .FontSize(10)
            .Bold();

        foreach(
            var profile
            in profiles)
        {
            var ordered =
                profile.Descending
                    ? profile.Branches
                        .OrderByDescending(
                            x => x.Value)
                        .ThenBy(
                            x => x.BranchCode)
                        .ToList()
                    : profile.Branches
                        .OrderBy(
                            x => x.Value)
                        .ThenBy(
                            x => x.BranchCode)
                        .ToList();

            if(ordered.Count == 0)
                continue;

            var highest =
                ordered.First();

            var lowest =
                ordered.Last();

            var average =
                ordered.Average(
                    x => x.Value);

            var difference =
                Math.Abs(
                    highest.Value -
                    lowest.Value);

            column.Item()
                .PaddingTop(9)
                .Border(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .Padding(7)
                .Column(
                    metric =>
                    {
                        metric.Item()
                            .Background(
                                Colors.Blue.Lighten5)
                            .Padding(5)
                            .Text(
                                profile.Title)
                            .Bold()
                            .FontSize(10)
                            .FontColor(
                                Colors.Blue.Darken3);

                        metric.Item()
                            .PaddingTop(5)
                            .Row(
                                row =>
                                {
                                    ExecutiveCard(
                                        row,
                                        T(
                                            LocalizationKeys.FinancialAnalysis.Highest,
                                            language),
                                        BranchDisplay(
                                            highest,
                                            language),
                                        highest.Value);

                                    ExecutiveCard(
                                        row,
                                        T(
                                            LocalizationKeys.FinancialAnalysis.Lowest,
                                            language),
                                        BranchDisplay(
                                            lowest,
                                            language),
                                        lowest.Value);

                                    ExecutiveCard(
                                        row,
                                        T(
                                            LocalizationKeys.FinancialAnalysis.Average,
                                            language),
                                        string.Empty,
                                        average);

                                    ExecutiveCard(
                                        row,
                                        T(
                                            LocalizationKeys.FinancialAnalysis.Change,
                                            language),
                                        string.Empty,
                                        difference);
                                });
                    });
        }
    }

    private static void ExecutiveCard(
        RowDescriptor row,
        string title,
        string branch,
        decimal value)
    {
        row.RelativeItem()
            .PaddingHorizontal(2)
            .Border(0.7f)
            .BorderColor(
                Colors.Grey.Lighten2)
            .Padding(5)
            .Column(
                card =>
                {
                    card.Item()
                        .AlignCenter()
                        .Text(
                            title)
                        .Bold()
                        .FontSize(8)
                        .FontColor(
                            Colors.Blue.Darken3);

                    if(
                        !string.IsNullOrWhiteSpace(
                            branch))
                    {
                        card.Item()
                            .PaddingTop(2)
                            .AlignCenter()
                            .Text(
                                branch)
                            .FontSize(7.5f)
                            .Bold();
                    }

                    card.Item()
                        .PaddingTop(2)
                        .AlignCenter()
                        .Text(
                            value.ToString(
                                "#,##0.00",
                                CultureInfo.InvariantCulture))
                        .FontSize(8.5f)
                        .Bold();
                });
    }


    private static void ComposeComparisonProfile(
        ColumnDescriptor column,
        ComparisonProfile profile,
        FinancialAnalysisDashboardDto requestedDashboard,
        string language)
    {
        column.Item()
            .Element(
                x =>
                    SubHeader(
                        x,
                        profile.Title));

        var ordered =
            profile.Descending
                ? profile.Branches
                    .OrderByDescending(
                        x => x.Value)
                    .ThenBy(
                        x => x.BranchCode)
                    .ToList()
                : profile.Branches
                    .OrderBy(
                        x => x.Value)
                    .ThenBy(
                        x => x.BranchCode)
                    .ToList();

        if(ordered.Count == 0)
            return;

        var highest =
            ordered.First();

        var lowest =
            ordered.Last();

        var average =
            ordered.Average(
                x => x.Value);

        var difference =
            Math.Abs(
                highest.Value -
                lowest.Value);

        var percentageDifference =
            lowest.Value == 0
                ? (decimal?)null
                : Math.Abs(
                    difference /
                    lowest.Value *
                    100m);

        column.Item()
            .PaddingTop(10)
            .Row(
                row =>
                {
                    SummaryCard(
                        row,
                        T(
                            LocalizationKeys.FinancialAnalysis.Highest,
                            language),
                        BranchDisplay(
                            highest,
                            language),
                        highest.Value);

                    SummaryCard(
                        row,
                        T(
                            LocalizationKeys.FinancialAnalysis.Lowest,
                            language),
                        BranchDisplay(
                            lowest,
                            language),
                        lowest.Value);

                    SummaryCard(
                        row,
                        T(
                            LocalizationKeys.FinancialAnalysis.Average,
                            language),
                        string.Empty,
                        average);
                });

        column.Item()
            .PaddingTop(8)
            .Border(1)
            .BorderColor(
                Colors.Blue.Lighten3)
            .Background(
                Colors.Blue.Lighten5)
            .Padding(8)
            .Row(
                row =>
                {
                    row.RelativeItem()
                        .Text(
                            T(
                                LocalizationKeys.FinancialAnalysis.Change,
                                language))
                        .Bold()
                        .FontSize(10);

                    row.RelativeItem()
                        .AlignCenter()
                        .Text(
                            difference.ToString(
                                "#,##0.00",
                                CultureInfo.InvariantCulture))
                        .Bold()
                        .FontSize(10);

                    row.RelativeItem()
                        .AlignRight()
                        .Text(
                            percentageDifference.HasValue
                                ? $"{percentageDifference.Value:N2}%"
                                : "-")
                        .Bold()
                        .FontSize(10);
                });

        column.Item()
            .PaddingTop(12)
            .Text(
                T(
                    LocalizationKeys.FinancialAnalysis.BranchDetails,
                    language))
            .Bold()
            .FontSize(11)
            .FontColor(
                Colors.Blue.Darken3);

        var maxAbsolute =
            ordered
                .Select(
                    x =>
                        Math.Abs(
                            x.Value))
                .DefaultIfEmpty(1m)
                .Max();

        if(maxAbsolute <= 0)
            maxAbsolute = 1;

        foreach(
            var item
            in ordered)
        {
            var isSelected =
                !requestedDashboard.IsAllBranches &&
                requestedDashboard.SelectedBranch is not null &&
                item.BranchCode.Equals(
                    requestedDashboard.SelectedBranch.BranchCode,
                    StringComparison.OrdinalIgnoreCase);

            var ratio =
                Math.Abs(
                    item.Value) /
                maxAbsolute;

            var barWidth =
                Math.Clamp(
                    (float)(
                        ratio *
                        250m),
                    2f,
                    250f);

            column.Item()
                .PaddingTop(5)
                .Border(
                    isSelected
                        ? 1.4f
                        : 0.5f)
                .BorderColor(
                    isSelected
                        ? Colors.Blue.Darken2
                        : Colors.Grey.Lighten2)
                .Padding(5)
                .Row(
                    row =>
                    {
                        row.ConstantItem(105)
                            .AlignMiddle()
                            .Text(
                                BranchDisplay(
                                    item,
                                    language))
                            .Bold()
                            .FontSize(9);

                        row.RelativeItem()
                            .AlignMiddle()
                            .Height(13)
                            .Row(
                                bar =>
                                {
                                    bar.ConstantItem(
                                            barWidth)
                                        .Background(
                                            item.Value < 0
                                                ? Colors.Red.Darken1
                                                : isSelected
                                                    ? Colors.Blue.Darken3
                                                    : Colors.Blue.Darken1);

                                    bar.RelativeItem()
                                        .Background(
                                            Colors.Grey.Lighten3);
                                });

                        row.ConstantItem(120)
                            .AlignMiddle()
                            .AlignRight()
                            .Text(
                                item.Value.ToString(
                                    "#,##0.00",
                                    CultureInfo.InvariantCulture))
                            .FontSize(9)
                            .Bold();
                    });
        }
    }

    private static void SummaryCard(
        RowDescriptor row,
        string title,
        string branch,
        decimal value)
    {
        row.RelativeItem()
            .PaddingHorizontal(3)
            .Border(1)
            .BorderColor(
                Colors.Grey.Lighten2)
            .Padding(7)
            .Column(
                card =>
                {
                    card.Item()
                        .AlignCenter()
                        .Text(
                            title)
                        .Bold()
                        .FontSize(9)
                        .FontColor(
                            Colors.Blue.Darken3);

                    if(
                        !string.IsNullOrWhiteSpace(
                            branch))
                    {
                        card.Item()
                            .PaddingTop(3)
                            .AlignCenter()
                            .Text(
                                branch)
                            .Bold()
                            .FontSize(9);
                    }

                    card.Item()
                        .PaddingTop(3)
                        .AlignCenter()
                        .Text(
                            value.ToString(
                                "#,##0.00",
                                CultureInfo.InvariantCulture))
                        .FontSize(10)
                        .Bold();
                });
    }

    private static List<ComparisonProfile>
        BuildComparisonProfiles(
            FinancialAnalysisDashboardDto dashboard,
            string language)
    {
        var result =
            new List<ComparisonProfile>();

        AddSimpleProfile(
            result,
            dashboard,
            "sales",
            "netSales",
            "hasIncomeStatement",
            SectionTitle(
                "sales",
                language),
            true);

        AddSimpleProfile(
            result,
            dashboard,
            "inventory",
            "openingInventory",
            "hasInventoryAccount",
            $"{SectionTitle("inventory",language)} - {T(LocalizationKeys.FinancialAnalysis.OpeningInventory,language)}",
            true);

        AddSimpleProfile(
            result,
            dashboard,
            "inventory",
            "closingInventory",
            "hasInventoryAccount",
            $"{SectionTitle("inventory",language)} - {T(LocalizationKeys.FinancialAnalysis.ClosingInventory,language)}",
            true);

        AddSimpleProfile(
            result,
            dashboard,
            "expenses",
            "totalExpenses",
            "hasExpenseAccount",
            SectionTitle(
                "expenses",
                language),
            true);

        AddSimpleProfile(
            result,
            dashboard,
            "purchases",
            "purchases",
            "hasPurchasesAccount",
            SectionTitle(
                "purchases",
                language),
            true);

        AddSimpleProfile(
            result,
            dashboard,
            "profit",
            "signedValue",
            "hasProfitLossResult",
            SectionTitle(
                "profit",
                language),
            true);

        AddSimpleProfile(
            result,
            dashboard,
            "customers",
            "customersTotal",
            "hasCustomerAccount",
            $"{SectionTitle("customers",language)} - {T(LocalizationKeys.FinancialAnalysis.Customers,language)}",
            true);

        AddSimpleProfile(
            result,
            dashboard,
            "customers",
            "employeesTotal",
            "hasEmployeeAccount",
            $"{SectionTitle("customers",language)} - {T(LocalizationKeys.FinancialAnalysis.Employees,language)}",
            true);

        AddDebtProfile(
            result,
            dashboard,
            language);

        AddSimpleProfile(
            result,
            dashboard,
            "cash",
            "totalCash",
            "hasCashGroup",
            SectionTitle(
                "cash",
                language),
            true);

        AddSimpleProfile(
            result,
            dashboard,
            "suppliers",
            "suppliersTotal",
            "hasSupplierAccount",
            SectionTitle(
                "suppliers",
                language),
            true);

        AddSimpleProfile(
            result,
            dashboard,
            "capital",
            "capital",
            "hasCapitalAccount",
            SectionTitle(
                "capital",
                language),
            true);

        AddTransferProfile(
            result,
            dashboard,
            language);

        AddSettlementProfile(
            result,
            dashboard,
            language);

        return result;
    }

    private static void AddSimpleProfile(
        List<ComparisonProfile> result,
        FinancialAnalysisDashboardDto dashboard,
        string sectionName,
        string valueProperty,
        string requiredFlag,
        string title,
        bool descending)
    {
        if(
            !dashboard.Sections.TryGetValue(
                sectionName,
                out var section) ||
            !section.HasValue ||
            section.Value.ValueKind !=
                JsonValueKind.Object ||
            !section.Value.TryGetProperty(
                "branches",
                out var branches) ||
            branches.ValueKind !=
                JsonValueKind.Array)
        {
            return;
        }

        var items =
            new List<ComparisonBranch>();

        foreach(
            var branch
            in branches.EnumerateArray())
        {
            if(
                branch.ValueKind !=
                    JsonValueKind.Object ||
                !IsFlagEnabled(
                    branch,
                    requiredFlag) ||
                !TryDecimal(
                    branch,
                    valueProperty,
                    out var value))
            {
                continue;
            }

            items.Add(
                ReadBranch(
                    branch,
                    value));
        }

        if(items.Count == 0)
            return;

        result.Add(
            new ComparisonProfile(
                title,
                descending,
                items));
    }

    private static void AddDebtProfile(
        List<ComparisonProfile> result,
        FinancialAnalysisDashboardDto dashboard,
        string language)
    {
        if(
            !TryGetBranches(
                dashboard,
                "debt",
                out var branches))
        {
            return;
        }

        var items =
            new List<ComparisonBranch>();

        foreach(
            var branch
            in branches.EnumerateArray())
        {
            if(
                !IsFlagEnabled(
                    branch,
                    "hasMaleDebtAccount") ||
                !IsFlagEnabled(
                    branch,
                    "hasFemaleDebtAccount") ||
                !TryDecimal(
                    branch,
                    "totalDebt",
                    out var value))
            {
                continue;
            }

            items.Add(
                ReadBranch(
                    branch,
                    value));
        }

        if(items.Count > 0)
        {
            result.Add(
                new ComparisonProfile(
                    SectionTitle(
                        "debt",
                        language),
                    false,
                    items));
        }
    }

    private static void AddTransferProfile(
        List<ComparisonProfile> result,
        FinancialAnalysisDashboardDto dashboard,
        string language)
    {
        if(
            !TryGetBranches(
                dashboard,
                "transfers",
                out var branches))
        {
            return;
        }

        var items =
            new List<ComparisonBranch>();

        foreach(
            var branch
            in branches.EnumerateArray())
        {
            if(
                !IsFlagEnabled(
                    branch,
                    "hasMaleTransfer") ||
                !IsFlagEnabled(
                    branch,
                    "hasFemaleTransfer") ||
                !TryDecimal(
                    branch,
                    "totalTransfer",
                    out var value))
            {
                continue;
            }

            items.Add(
                ReadBranch(
                    branch,
                    value));
        }

        if(items.Count > 0)
        {
            result.Add(
                new ComparisonProfile(
                    SectionTitle(
                        "transfers",
                        language),
                    true,
                    items));
        }
    }

    private static void AddSettlementProfile(
        List<ComparisonProfile> result,
        FinancialAnalysisDashboardDto dashboard,
        string language)
    {
        if(
            !TryGetBranches(
                dashboard,
                "settlements",
                out var branches))
        {
            return;
        }

        var items =
            new List<ComparisonBranch>();

        foreach(
            var branch
            in branches.EnumerateArray())
        {
            if(
                branch.ValueKind !=
                    JsonValueKind.Object ||
                !branch.TryGetProperty(
                    "male",
                    out var male) ||
                !branch.TryGetProperty(
                    "female",
                    out var female) ||
                !IsFlagEnabled(
                    male,
                    "hasFile") ||
                !IsFlagEnabled(
                    female,
                    "hasFile") ||
                !TryDecimal(
                    male,
                    "balance",
                    out var maleBalance) ||
                !TryDecimal(
                    female,
                    "balance",
                    out var femaleBalance))
            {
                continue;
            }

            items.Add(
                ReadBranch(
                    branch,
                    maleBalance +
                    femaleBalance));
        }

        if(items.Count > 0)
        {
            result.Add(
                new ComparisonProfile(
                    $"{SectionTitle("settlements",language)} - {T(LocalizationKeys.FinancialAnalysis.Balance,language)}",
                    true,
                    items));
        }
    }

    private static bool TryGetBranches(
        FinancialAnalysisDashboardDto dashboard,
        string sectionName,
        out JsonElement branches)
    {
        branches =
            default;

        return
            dashboard.Sections.TryGetValue(
                sectionName,
                out var section) &&
            section.HasValue &&
            section.Value.ValueKind ==
                JsonValueKind.Object &&
            section.Value.TryGetProperty(
                "branches",
                out branches) &&
            branches.ValueKind ==
                JsonValueKind.Array;
    }

    private static ComparisonBranch ReadBranch(
        JsonElement branch,
        decimal value)
    {
        return new ComparisonBranch(
            GetString(
                branch,
                "branchCode"),
            GetString(
                branch,
                "branchName"),
            value);
    }

    private static bool IsFlagEnabled(
        JsonElement element,
        string propertyName)
    {
        if(
            element.ValueKind !=
                JsonValueKind.Object ||
            !element.TryGetProperty(
                propertyName,
                out var property))
        {
            return false;
        }

        return
            property.ValueKind ==
                JsonValueKind.True ||
            (
                property.ValueKind ==
                    JsonValueKind.String &&
                bool.TryParse(
                    property.GetString(),
                    out var value) &&
                value
            );
    }

    private static bool TryDecimal(
        JsonElement element,
        string propertyName,
        out decimal value)
    {
        value = 0;

        if(
            element.ValueKind !=
                JsonValueKind.Object ||
            !element.TryGetProperty(
                propertyName,
                out var property))
        {
            return false;
        }

        if(
            property.ValueKind ==
                JsonValueKind.Number &&
            property.TryGetDecimal(
                out value))
        {
            return true;
        }

        return
            property.ValueKind ==
                JsonValueKind.String &&
            decimal.TryParse(
                property.GetString(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out value);
    }

    private static string BranchDisplay(
        ComparisonBranch branch,
        string language)
    {
        if(
            language == "en" ||
            string.IsNullOrWhiteSpace(
                branch.BranchName))
        {
            return branch.BranchCode;
        }

        return
            $"{branch.BranchCode} - {branch.BranchName}";
    }

    private sealed record ComparisonBranch(
        string BranchCode,
        string BranchName,
        decimal Value);

    private sealed record ComparisonProfile(
        string Title,
        bool Descending,
        List<ComparisonBranch> Branches);
    private static void RenderElement(
        ColumnDescriptor column,
        string name,
        JsonElement element,
        string language,
        bool isSectionRoot = false)
    {
        switch(element.ValueKind)
        {
            case JsonValueKind.Object:
                RenderObject(
                    column,
                    name,
                    element,
                    language,
                    isSectionRoot);
                break;

            case JsonValueKind.Array:
                RenderArray(
                    column,
                    name,
                    element,
                    language);
                break;

            default:
            {
                var label =
                    Label(
                        name,
                        language);

                if(
                    !string.IsNullOrWhiteSpace(
                        label))
                {
                    AddKeyValue(
                        column,
                        label,
                        FormatValue(
                            element,
                            language));
                }

                break;
            }
        }
    }

    private static void RenderObject(
        ColumnDescriptor column,
        string name,
        JsonElement element,
        string language,
        bool isSectionRoot)
    {
        if(
            !isSectionRoot &&
            !name.Equals(
                "details",
                StringComparison.OrdinalIgnoreCase))
        {
            var objectTitle =
                Label(
                    name,
                    language);

            if(
                !string.IsNullOrWhiteSpace(
                    objectTitle))
            {
                column.Item()
                    .PaddingTop(5)
                    .Element(
                        x =>
                            SubHeader(
                                x,
                                objectTitle));
            }
        }

        foreach(
            var property
            in element.EnumerateObject())
        {
            if(
                IsHiddenField(
                    property.Name))
            {
                continue;
            }

            if(
                property.Value.ValueKind ==
                    JsonValueKind.Object ||
                property.Value.ValueKind ==
                    JsonValueKind.Array)
            {
                RenderElement(
                    column,
                    property.Name,
                    property.Value,
                    language);

                continue;
            }

            var label =
                Label(
                    property.Name,
                    language);

            if(
                string.IsNullOrWhiteSpace(
                    label))
            {
                continue;
            }

            AddKeyValue(
                column,
                label,
                FormatValue(
                    property.Value,
                    language));
        }
    }

    private static void RenderArray(
        ColumnDescriptor column,
        string name,
        JsonElement array,
        string language)
    {
        var title =
            Label(
                name,
                language);

        if(
            !string.IsNullOrWhiteSpace(
                title))
        {
            column.Item()
                .PaddingTop(6)
                .Element(
                    x =>
                        SubHeader(
                            x,
                            title));
        }

        foreach(
            var item
            in array.EnumerateArray())
        {
            if(
                item.ValueKind ==
                    JsonValueKind.Object)
            {
                var branchCode =
                    GetString(
                        item,
                        "branchCode");

                var branchName =
                    GetString(
                        item,
                        "branchName");

                column.Item()
                    .PaddingTop(4)
                    .Border(1)
                    .BorderColor(
                        Colors.Grey.Lighten2)
                    .Padding(6)
                    .Column(
                        card =>
                        {
                            card.Spacing(3);

                            if(
                                !string.IsNullOrWhiteSpace(
                                    branchCode) ||
                                !string.IsNullOrWhiteSpace(
                                    branchName))
                            {
                                card.Item()
                                    .Background(
                                        Colors.Grey.Lighten3)
                                    .Padding(5)
                                    .Text(
                                        $"{branchCode} - {branchName}")
                                    .Bold()
                                    .FontColor(
                                        Colors.Blue.Darken2);
                            }

                            RenderObject(
                                card,
                                name,
                                item,
                                language,
                                true);
                        });
            }
            else
            {
                column.Item()
                    .PaddingVertical(2)
                    .Text(
                        FormatValue(
                            item,
                            language));
            }
        }
    }

    private static void AddKeyValue(
        ColumnDescriptor column,
        string label,
        string value)
    {
        column.Item()
            .BorderBottom(0.5f)
            .BorderColor(
                Colors.Grey.Lighten2)
            .Row(
                row =>
                {
                    row.RelativeItem()
                        .Background(
                            Colors.Grey.Lighten3)
                        .Padding(5)
                        .Text(
                            label)
                        .Bold();

                    row.RelativeItem()
                        .Padding(5)
                        .Text(
                            value);
                });
    }

    private static void SectionHeader(
        IContainer container,
        string title)
    {
        container
            .Background(
                Colors.Blue.Darken3)
            .Padding(8)
            .AlignCenter()
            .Text(
                title)
            .Bold()
            .FontSize(14)
            .FontColor(
                Colors.White);
    }

    private static void SubHeader(
        IContainer container,
        string title)
    {
        container
            .Background(
                Colors.LightBlue.Lighten3)
            .Padding(6)
            .AlignCenter()
            .Text(
                title)
            .Bold()
            .FontSize(10);
    }

    private static string FormatValue(
        JsonElement value,
        string language)
    {
        switch(value.ValueKind)
        {
            case JsonValueKind.Number:
            {
                if(
                    value.TryGetDecimal(
                        out var number))
                {
                    return number.ToString(
                        "#,##0.00",
                        CultureInfo.InvariantCulture);
                }

                return value.ToString();
            }

            case JsonValueKind.True:
                return T(
                    LocalizationKeys.FinancialAnalysis.Yes,
                    language);

            case JsonValueKind.False:
                return T(
                    LocalizationKeys.FinancialAnalysis.No,
                    language);

            case JsonValueKind.String:
            {
                var text =
                    value.GetString()
                    ?? string.Empty;

                var localized =
                    LocalizeNamedValue(
                        text,
                        language);

                if(
                    !string.Equals(
                        localized,
                        text,
                        StringComparison.Ordinal))
                {
                    return localized;
                }

                if(
                    DateTime.TryParse(
                        text,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var date))
                {
                    if(date.Year <= 1)
                        return string.Empty;

                    return date.ToString(
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture);
                }

                return text;
            }

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return string.Empty;

            default:
                return value.ToString();
        }
    }

    private static string SectionTitle(
        string section,
        string language)
    {
        return
            SectionKeys.TryGetValue(
                section,
                out var key)
                ? T(
                    key,
                    language)
                : section;
    }

    private static string? Label(
        string name,
        string language)
    {
        if(
            AliasKeys.TryGetValue(
                name,
                out var aliasKey))
        {
            return T(
                aliasKey,
                language);
        }

        if(
            AutomaticKeys.TryGetValue(
                name,
                out var key))
        {
            return T(
                key,
                language);
        }

        var prefixes =
            new[]
            {
                (
                    Prefix: "allBranches",
                    Key: LocalizationKeys.FinancialAnalysis.AllBranches
                ),
                (
                    Prefix: "average",
                    Key: LocalizationKeys.FinancialAnalysis.Average
                ),
                (
                    Prefix: "highest",
                    Key: LocalizationKeys.FinancialAnalysis.Highest
                ),
                (
                    Prefix: "lowest",
                    Key: LocalizationKeys.FinancialAnalysis.Lowest
                )
            };

        foreach(
            var item
            in prefixes)
        {
            if(
                !name.StartsWith(
                    item.Prefix,
                    StringComparison.OrdinalIgnoreCase) ||
                name.Length <=
                    item.Prefix.Length)
            {
                continue;
            }

            var childName =
                name[item.Prefix.Length..];

            childName =
                char.ToLowerInvariant(
                    childName[0]) +
                childName[1..];

            var child =
                Label(
                    childName,
                    language);

            if(
                !string.IsNullOrWhiteSpace(
                    child))
            {
                return
                    $"{T(item.Key,language)} - {child}";
            }
        }

        return language == "en"
            ? Humanize(
                name)
            : null;
    }

    private static string LocalizeNamedValue(
        string value,
        string language)
    {
        var normalized =
            value
                .Replace(
                    "-",
                    string.Empty)
                .Replace(
                    " ",
                    string.Empty);

        if(
            AutomaticKeys.TryGetValue(
                normalized,
                out var key))
        {
            return T(
                key,
                language);
        }

        return value;
    }

    private static bool IsHiddenField(
        string name)
    {
        return
            name.Equals(
                "financialPeriodId",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "financialUploadId",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "branchId",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "id",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "rowOrder",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "isMarked",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "isMatched",
                StringComparison.OrdinalIgnoreCase);
    }

    private static string GetString(
        JsonElement element,
        string name)
    {
        if(
            element.ValueKind ==
                JsonValueKind.Object &&
            element.TryGetProperty(
                name,
                out var property))
        {
            return property.ValueKind ==
                JsonValueKind.String
                ? property.GetString()
                    ?? string.Empty
                : property.ToString();
        }

        return string.Empty;
    }

    private static string Humanize(
        string value)
    {
        if(
            string.IsNullOrWhiteSpace(
                value))
        {
            return string.Empty;
        }

        var result =
            new List<char>();

        for(
            var i = 0;
            i < value.Length;
            i++)
        {
            if(
                i > 0 &&
                char.IsUpper(
                    value[i]))
            {
                result.Add(' ');
            }

            result.Add(
                value[i]);
        }

        return new string(
            result.ToArray());
    }

    private static string T(
        string key,
        string language)
    {
        var resources =
            language == "en"
                ? EnglishResources.Values
                : ArabicResources.Values;

        return
            resources.TryGetValue(
                key,
                out var value)
                ? value
                : key;
    }

    private static void RegisterTimesNewRoman()
    {
        if(_fontRegistered)
            return;

        lock(FontLock)
        {
            if(_fontRegistered)
                return;

            var path =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.Fonts),
                    "times.ttf");

            if(!File.Exists(path))
            {
                throw new FileNotFoundException(
                    "Times New Roman font was not found.",
                    path);
            }

            using var stream =
                File.OpenRead(
                    path);

            FontManager.RegisterFont(
                stream);

            _fontRegistered =
                true;
        }
    }
}
