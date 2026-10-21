using System.Globalization;
using System.Text.Json;
using ClosedXML.Excel;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Common.Localization;
using InventorySystem.Application.DTOs.FinancialAnalysis.DashboardAggregate;
using InventorySystem.Application.Features.FinancialAnalysis.Common.Localization;
using InventorySystem.Application.Features.FinancialAnalysis.Dashboard.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Reports.FinancialAnalysis;

public sealed class FinancialDashboardExcelExportService
    : IFinancialDashboardExcelExportService
{
    private readonly IApplicationDbContext _context;

    public FinancialDashboardExcelExportService(
        IApplicationDbContext context)
    {
        _context = context;
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

    private static readonly Dictionary<string,string> FieldKeys =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["branchCode"] = LocalizationKeys.FinancialAnalysis.BranchCode,
            ["branchName"] = LocalizationKeys.FinancialAnalysis.BranchName,
            ["resultType"] = LocalizationKeys.FinancialAnalysis.ResultType,
            ["resultValue"] = LocalizationKeys.FinancialAnalysis.ResultValue,
            ["signedValue"] = LocalizationKeys.FinancialAnalysis.SignedValue,
            ["assets"] = LocalizationKeys.FinancialAnalysis.Assets,
            ["liabilities"] = LocalizationKeys.FinancialAnalysis.Liabilities,
            ["accountCode"] = LocalizationKeys.FinancialAnalysis.AccountCode,
            ["accountName"] = LocalizationKeys.FinancialAnalysis.AccountName,
            ["hasFile"] = LocalizationKeys.FinancialAnalysis.FileAvailable,
            ["status"] = LocalizationKeys.FinancialAnalysis.Status,

            ["sales"] = LocalizationKeys.FinancialAnalysis.Sales,
            ["salesReturns"] = LocalizationKeys.FinancialAnalysis.SalesReturns,
            ["discountAllowed"] = LocalizationKeys.FinancialAnalysis.DiscountAllowed,
            ["branchDiscountAllowed"] = LocalizationKeys.FinancialAnalysis.BranchDiscountAllowed,
            ["netSales"] = LocalizationKeys.FinancialAnalysis.NetSales,
            ["allBranchesSales"] = LocalizationKeys.FinancialAnalysis.Sales,
            ["allBranchesSalesReturns"] = LocalizationKeys.FinancialAnalysis.SalesReturns,
            ["allBranchesDiscountAllowed"] = LocalizationKeys.FinancialAnalysis.DiscountAllowed,
            ["allBranchesBranchDiscountAllowed"] = LocalizationKeys.FinancialAnalysis.BranchDiscountAllowed,
            ["allBranchesTotal"] = LocalizationKeys.FinancialAnalysis.Total,
            ["averageNetSales"] = LocalizationKeys.FinancialAnalysis.Average,

            ["openingInventory"] = LocalizationKeys.FinancialAnalysis.OpeningInventory,
            ["closingInventory"] = LocalizationKeys.FinancialAnalysis.ClosingInventory,
            ["inventoryChange"] = LocalizationKeys.FinancialAnalysis.InventoryChange,
            ["allBranchesOpeningTotal"] = LocalizationKeys.FinancialAnalysis.OpeningInventory,
            ["allBranchesClosingTotal"] = LocalizationKeys.FinancialAnalysis.ClosingInventory,
            ["averageOpeningInventory"] = LocalizationKeys.FinancialAnalysis.Average,
            ["averageClosingInventory"] = LocalizationKeys.FinancialAnalysis.Average,

            ["expensesTotal"] = LocalizationKeys.FinancialAnalysis.TotalExpenses,
            ["allBranchesExpenses"] = LocalizationKeys.FinancialAnalysis.TotalExpenses,

            ["purchases"] = LocalizationKeys.FinancialAnalysis.Purchases,
            ["purchasesTotal"] = LocalizationKeys.FinancialAnalysis.TotalPurchases,
            ["allBranchesPurchases"] = LocalizationKeys.FinancialAnalysis.TotalPurchases,

            ["profit"] = LocalizationKeys.FinancialAnalysis.Profit,
            ["loss"] = LocalizationKeys.FinancialAnalysis.Loss,
            ["totalProfit"] = LocalizationKeys.FinancialAnalysis.TotalProfit,
            ["totalLoss"] = LocalizationKeys.FinancialAnalysis.TotalLoss,
            ["overallResult"] = LocalizationKeys.FinancialAnalysis.OverallResult,
            ["profitableBranches"] = LocalizationKeys.FinancialAnalysis.ProfitableBranches,
            ["lossBranches"] = LocalizationKeys.FinancialAnalysis.LossBranches,
            ["breakEvenBranches"] = LocalizationKeys.FinancialAnalysis.BreakEvenBranches,

            ["customersTotal"] = LocalizationKeys.FinancialAnalysis.CustomerBalance,
            ["employeesTotal"] = LocalizationKeys.FinancialAnalysis.EmployeeBalance,
            ["allBranchesCustomersTotal"] = LocalizationKeys.FinancialAnalysis.CustomerBalance,
            ["allBranchesEmployeesTotal"] = LocalizationKeys.FinancialAnalysis.EmployeeBalance,

            ["maleDebt"] = LocalizationKeys.FinancialAnalysis.MaleDebt,
            ["femaleDebt"] = LocalizationKeys.FinancialAnalysis.FemaleDebt,
            ["totalDebt"] = LocalizationKeys.FinancialAnalysis.TotalDebt,
            ["allBranchesMaleDebt"] = LocalizationKeys.FinancialAnalysis.MaleDebt,
            ["allBranchesFemaleDebt"] = LocalizationKeys.FinancialAnalysis.FemaleDebt,
            ["allBranchesTotalDebt"] = LocalizationKeys.FinancialAnalysis.TotalDebt,
            ["averageDebt"] = LocalizationKeys.FinancialAnalysis.Average,

            ["totalCash"] = LocalizationKeys.FinancialAnalysis.TotalCash,
            ["allBranchesCash"] = LocalizationKeys.FinancialAnalysis.TotalCash,
            ["averageCash"] = LocalizationKeys.FinancialAnalysis.Average,
            ["cashboxesCount"] = LocalizationKeys.FinancialAnalysis.CashboxesCount,

            ["suppliersTotal"] = LocalizationKeys.FinancialAnalysis.SuppliersTotal,
            ["allBranchesSuppliers"] = LocalizationKeys.FinancialAnalysis.SuppliersTotal,
            ["averageSuppliers"] = LocalizationKeys.FinancialAnalysis.Average,

            ["capital"] = LocalizationKeys.FinancialAnalysis.CapitalValue,
            ["allBranchesCapital"] = LocalizationKeys.FinancialAnalysis.CapitalValue,
            ["averageCapital"] = LocalizationKeys.FinancialAnalysis.Average,

            ["maleTransfer"] = LocalizationKeys.FinancialAnalysis.MaleTransfer,
            ["femaleTransfer"] = LocalizationKeys.FinancialAnalysis.FemaleTransfer,
            ["totalTransfer"] = LocalizationKeys.FinancialAnalysis.TotalTransfer,
            ["allBranchesMaleTransfers"] = LocalizationKeys.FinancialAnalysis.MaleTransfer,
            ["allBranchesFemaleTransfers"] = LocalizationKeys.FinancialAnalysis.FemaleTransfer,
            ["allBranchesTotalTransfers"] = LocalizationKeys.FinancialAnalysis.TotalTransfer,
            ["averageCompleteBranchTransfers"] = LocalizationKeys.FinancialAnalysis.Average,

            ["debit"] = LocalizationKeys.FinancialAnalysis.Debit,
            ["credit"] = LocalizationKeys.FinancialAnalysis.Credit,
            ["balance"] = LocalizationKeys.FinancialAnalysis.Balance,
            ["periodDebit"] = LocalizationKeys.FinancialAnalysis.PeriodDebit,
            ["periodCredit"] = LocalizationKeys.FinancialAnalysis.PeriodCredit,
            ["periodBalance"] = LocalizationKeys.FinancialAnalysis.PeriodBalance,
            ["allBranchesDebit"] = LocalizationKeys.FinancialAnalysis.Debit,
            ["allBranchesCredit"] = LocalizationKeys.FinancialAnalysis.Credit,
            ["allBranchesBalance"] = LocalizationKeys.FinancialAnalysis.Balance,
            ["allBranchesMaleDebit"] = LocalizationKeys.FinancialAnalysis.MaleDebt,
            ["allBranchesMaleCredit"] = LocalizationKeys.FinancialAnalysis.MaleTransfer,
            ["allBranchesMaleBalance"] = LocalizationKeys.FinancialAnalysis.MaleDebt,
            ["allBranchesFemaleDebit"] = LocalizationKeys.FinancialAnalysis.FemaleDebt,
            ["allBranchesFemaleCredit"] = LocalizationKeys.FinancialAnalysis.FemaleTransfer,
            ["allBranchesFemaleBalance"] = LocalizationKeys.FinancialAnalysis.FemaleDebt,
            ["averageCompleteBranchBalance"] = LocalizationKeys.FinancialAnalysis.Average,

            ["branchesWithData"] = LocalizationKeys.FinancialAnalysis.BranchesWithData,
            ["branchesWithoutData"] = LocalizationKeys.FinancialAnalysis.BranchesWithoutData,
            ["branchesWithAnyData"] = LocalizationKeys.FinancialAnalysis.BranchesWithData,
            ["branchesWithCompleteData"] = LocalizationKeys.FinancialAnalysis.CompleteBranches,
            ["branchesWithoutCompleteData"] = LocalizationKeys.FinancialAnalysis.IncompleteBranches,
            ["completeBranches"] = LocalizationKeys.FinancialAnalysis.CompleteBranches,
            ["incompleteBranches"] = LocalizationKeys.FinancialAnalysis.IncompleteBranches,
            ["maleFilesCount"] = LocalizationKeys.FinancialAnalysis.MaleFiles,
            ["femaleFilesCount"] = LocalizationKeys.FinancialAnalysis.FemaleFiles,
            ["maleRecordsCount"] = LocalizationKeys.FinancialAnalysis.Records,
            ["femaleRecordsCount"] = LocalizationKeys.FinancialAnalysis.Records,
            ["transactionsCount"] = LocalizationKeys.FinancialAnalysis.Transactions
        };

    private static readonly HashSet<string> IgnoredFields =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "financialPeriodId",
            "startDate",
            "endDate",
            "status",
            "activeBranches",
            "branchId",
            "hasFile",
            "uploadId",
            "gender",
            "branches"
        };

    public async Task<byte[]> ExportAsync(
        FinancialAnalysisDashboardDto dashboard,
        string language,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        language =
            FinancialAnalysisLocalization.NormalizeLanguage(
                language);

        var branchNames =
            await _context.Branches
                .AsNoTracking()
                .Where(
                    x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.BranchCode != "0" &&
                        x.BranchCode != "001" &&
                        x.BranchCode != "002")
                .OrderBy(
                    x =>
                        x.BranchCode)
                .Select(
                    x =>
                        new
                        {
                            x.Id,
                            x.BranchCode,
                            ArabicName =
                                x.BranchNameArabic,
                            EnglishName =
                                x.BranchNameEnglish
                        })
                .ToDictionaryAsync(
                    x =>
                        x.Id,
                    x =>
                        language == "en"
                            ? (
                                string.IsNullOrWhiteSpace(
                                    x.EnglishName)
                                    ? x.BranchCode
                                    : x.EnglishName
                              )
                            : (
                                string.IsNullOrWhiteSpace(
                                    x.ArabicName)
                                    ? x.BranchCode
                                    : x.ArabicName
                              ),
                    cancellationToken);

        if(
            dashboard.SelectedBranch is not null &&
            branchNames.TryGetValue(
                dashboard.SelectedBranch.BranchId,
                out var selectedBranchName)
        )
        {
            dashboard.SelectedBranch.BranchName =
                selectedBranchName;
        }

        var localizedSections =
            new Dictionary<string,JsonElement?>(
                StringComparer.OrdinalIgnoreCase);

        foreach(var pair in dashboard.Sections)
        {
            localizedSections[pair.Key] =
                pair.Value.HasValue
                    ? LocalizeBranchNames(
                        pair.Value.Value,
                        branchNames)
                    : null;
        }

        dashboard.Sections =
            localizedSections;

        using var workbook =
            new XLWorkbook();

        CreateOverviewSheet(
            workbook,
            dashboard,
            language);

        foreach(
            var sectionName
            in dashboard.SelectedSections)
        {
            if(
                !dashboard.Sections.TryGetValue(
                    sectionName,
                    out var section) ||
                section is null
            )
            {
                continue;
            }

            CreateSectionSheet(
                workbook,
                dashboard,
                sectionName,
                section.Value,
                language);
        }

        using var stream =
            new MemoryStream();



        FinalizeWorkbookPresentation(


            workbook,


            language);



        workbook.SaveAs(


            stream);

        return stream.ToArray();
    }

    private static JsonElement LocalizeBranchNames(
        JsonElement element,
        IReadOnlyDictionary<int,string> branchNames)
    {
        using var stream =
            new MemoryStream();

        using(
            var writer =
                new Utf8JsonWriter(
                    stream)
        )
        {
            WriteLocalizedJson(
                writer,
                element,
                branchNames);
        }

        using var document =
            JsonDocument.Parse(
                stream.ToArray());

        return document.RootElement.Clone();
    }

    private static void WriteLocalizedJson(
        Utf8JsonWriter writer,
        JsonElement element,
        IReadOnlyDictionary<int,string> branchNames)
    {
        switch(element.ValueKind)
        {
            case JsonValueKind.Object:
            {
                int? branchId =
                    null;

                if(
                    element.TryGetProperty(
                        "branchId",
                        out var branchIdElement) &&
                    branchIdElement.TryGetInt32(
                        out var parsedBranchId)
                )
                {
                    branchId =
                        parsedBranchId;
                }

                writer.WriteStartObject();

                foreach(
                    var property
                    in element.EnumerateObject())
                {
                    writer.WritePropertyName(
                        property.Name);

                    if(
                        branchId.HasValue &&
                        property.Name.Equals(
                            "branchName",
                            StringComparison.OrdinalIgnoreCase) &&
                        branchNames.TryGetValue(
                            branchId.Value,
                            out var localizedBranchName)
                    )
                    {
                        writer.WriteStringValue(
                            localizedBranchName);

                        continue;
                    }

                    WriteLocalizedJson(
                        writer,
                        property.Value,
                        branchNames);
                }

                writer.WriteEndObject();

                break;
            }

            case JsonValueKind.Array:
            {
                writer.WriteStartArray();

                foreach(
                    var item
                    in element.EnumerateArray())
                {
                    WriteLocalizedJson(
                        writer,
                        item,
                        branchNames);
                }

                writer.WriteEndArray();

                break;
            }

            default:
                element.WriteTo(
                    writer);
                break;
        }
    }
    private static void CreateOverviewSheet(
        XLWorkbook workbook,
        FinancialAnalysisDashboardDto dashboard,
        string language)
    {
        var ws =
            workbook.Worksheets.Add(
                language == "ar"
                    ? "الملخص التنفيذي"
                    : "Executive Summary");

        PrepareSheet(
            ws,
            language);

        ws.Range("A1:H2")
            .Merge();

        ws.Cell("A1").Value =
            T(
                LocalizationKeys.FinancialAnalysis.Dashboard,
                language);

        StyleMainTitle(
            ws.Range("A1:H2"));

        // ----------------------------------------------------
        // PERIOD / STATUS / SCOPE
        // ----------------------------------------------------

        ws.Range("A4:B4").Merge();
        ws.Range("C4:D4").Merge();
        ws.Range("E4:F4").Merge();
        ws.Range("G4:H4").Merge();

        ws.Cell("A4").Value =
            T(
                LocalizationKeys.FinancialAnalysis.PeriodFrom,
                language);

        ws.Cell("C4").Value =
            T(
                LocalizationKeys.FinancialAnalysis.PeriodTo,
                language);

        ws.Cell("E4").Value =
            T(
                LocalizationKeys.FinancialAnalysis.Status,
                language);

        ws.Cell("G4").Value =
            T(
                LocalizationKeys.FinancialAnalysis.Scope,
                language);

        StyleMetaHeader(
            ws.Range("A4:H4"));

        ws.Range("A5:B5").Merge();
        ws.Range("C5:D5").Merge();
        ws.Range("E5:F5").Merge();
        ws.Range("G5:H5").Merge();

        ws.Cell("A5").Value =
            dashboard.StartDate
                .ToString("dd/MM/yyyy");

        ws.Cell("C5").Value =
            dashboard.EndDate
                .ToString("dd/MM/yyyy");

        ws.Cell("E5").Value =
            FinancialAnalysisLocalization.PeriodStatus(
                dashboard.Status,
                language);

        ws.Cell("G5").Value =
            dashboard.IsAllBranches
                ? T(
                    LocalizationKeys.FinancialAnalysis.AllBranches,
                    language)
                : $"{dashboard.SelectedBranch?.BranchCode} - {dashboard.SelectedBranch?.BranchName}";

        StyleMetaValue(
            ws.Range("A5:H5"));

        // ----------------------------------------------------
        // IMPORTANT SUMMARY ONLY
        // ----------------------------------------------------

        WriteKpiCard(
            ws,
            7,
            2,
            T(
                LocalizationKeys.FinancialAnalysis.ActiveBranches,
                language),
            dashboard.ActiveBranches
                .ToString("N0"));

        WriteKpiCard(
            ws,
            7,
            5,
            T(
                LocalizationKeys.FinancialAnalysis.FinancialPeriod,
                language),
            dashboard.FinancialPeriodId
                .ToString());

        ws.Row(7).Height =
            28;

        ws.Row(8).Height =
            34;

        ws.Columns("A:H").Width =
            18;

        ws.Column("A").Width =
            22;

        ws.Column("C").Width =
            22;

        ws.Column("E").Width =
            22;

        ws.Column("G").Width =
            22;
    }

    private static void CreateSectionSheet(
        XLWorkbook workbook,
        FinancialAnalysisDashboardDto dashboard,
        string sectionName,
        JsonElement section,
        string language)
    {
        var ws =
            workbook.Worksheets.Add(
                SafeSheetName(
                    SectionTitle(
                        sectionName,
                        language)));

        PrepareSheet(
            ws,
            language);

        ws.Range("A1:H2")
            .Merge();

        ws.Cell("A1").Value =
            SectionTitle(
                sectionName,
                language);

        StyleMainTitle(
            ws.Range("A1:H2"));

        // ----------------------------------------------------
        // PERIOD / SCOPE
        // ----------------------------------------------------

        ws.Range("A4:D4")
            .Merge();

        ws.Range("E4:H4")
            .Merge();

        ws.Cell("A4").Value =
            T(
                LocalizationKeys.FinancialAnalysis.FinancialPeriod,
                language);

        ws.Cell("E4").Value =
            T(
                LocalizationKeys.FinancialAnalysis.Scope,
                language);

        StyleMetaHeader(
            ws.Range("A4:H4"));

        ws.Range("A5:D5")
            .Merge();

        ws.Range("E5:H5")
            .Merge();

        ws.Cell("A5").Value =
            $"{dashboard.StartDate:dd/MM/yyyy} - {dashboard.EndDate:dd/MM/yyyy}";

        ws.Cell("E5").Value =
            dashboard.IsAllBranches
                ? T(
                    LocalizationKeys.FinancialAnalysis.AllBranches,
                    language)
                : $"{dashboard.SelectedBranch?.BranchCode} - {dashboard.SelectedBranch?.BranchName}";

        StyleMetaValue(
            ws.Range("A5:H5"));

        // ----------------------------------------------------
        // FINANCIAL SUMMARY ONLY
        // ----------------------------------------------------

        var metrics =
            ExtractSummaryMetrics(
                    section,
                    sectionName)
                .Where(
                    x =>
                        !IsTechnicalExcelMetric(
                            x.Name))
                .Take(12)
                .ToList();

        var nextRow =
            7;

        if(metrics.Count > 0)
        {
            ws.Range(
                    nextRow,
                    1,
                    nextRow,
                    8)
                .Merge();

            ws.Cell(
                    nextRow,
                    1)
                .Value =
                T(
                    LocalizationKeys.FinancialAnalysis.SectionSummary,
                    language);

            StyleSectionHeader(
                ws.Range(
                    nextRow,
                    1,
                    nextRow,
                    8));

            nextRow++;

            var metricRow =
                nextRow;

            var metricColumn =
                1;

            foreach(
                var metric
                in metrics)
            {
                WriteKpiCard(
                    ws,
                    metricRow,
                    metricColumn,
                    Label(
                        metric.Name,
                        language),
                    FormatElement(
                        metric.Value,
                        language));

                metricColumn +=
                    2;

                if(metricColumn > 7)
                {
                    metricColumn =
                        1;

                    metricRow +=
                        3;
                }
            }

            nextRow =
                metricColumn == 1
                    ? metricRow
                    : metricRow + 3;
        }

        // ----------------------------------------------------
        // IMPORTANT OBJECT SUMMARIES
        // ----------------------------------------------------

        var objectMetrics =
            section.ValueKind ==
                JsonValueKind.Object
                ? section
                    .EnumerateObject()
                    .Where(
                        x =>
                            x.Value.ValueKind ==
                                JsonValueKind.Object &&
                            !IgnoredFields.Contains(
                                x.Name) &&
                            !x.Name.Equals(
                                "details",
                                StringComparison.OrdinalIgnoreCase) &&
                            !IsTechnicalExcelMetric(
                                x.Name))
                    .ToList()
                : new List<JsonProperty>();

        foreach(
            var metric
            in objectMetrics)
        {
            nextRow =
                WriteObjectSummary(
                    ws,
                    nextRow,
                    sectionName,
                    metric.Name,
                    metric.Value,
                    language);
        }

        // ----------------------------------------------------
        // BRANCH DETAILS
        // Same detailed financial information as PDF appendix.
        // ----------------------------------------------------

        if(
            section.ValueKind ==
                JsonValueKind.Object &&
            section.TryGetProperty(
                "branches",
                out var branches) &&
            branches.ValueKind ==
                JsonValueKind.Array)
        {
            nextRow +=
                1;

            ws.Range(
                    nextRow,
                    1,
                    nextRow,
                    8)
                .Merge();

            ws.Cell(
                    nextRow,
                    1)
                .Value =
                T(
                    LocalizationKeys.FinancialAnalysis.BranchDetails,
                    language);

            StyleSectionHeader(
                ws.Range(
                    nextRow,
                    1,
                    nextRow,
                    8));

            nextRow++;

            WriteBranchesTable(
                ws,
                ref nextRow,
                branches,
                language);
        }
        else if(
            !dashboard.IsAllBranches)
        {
            nextRow +=
                1;

            ws.Range(
                    nextRow,
                    1,
                    nextRow,
                    8)
                .Merge();

            ws.Cell(
                    nextRow,
                    1)
                .Value =
                T(
                    LocalizationKeys.FinancialAnalysis.Details,
                    language);

            StyleSectionHeader(
                ws.Range(
                    nextRow,
                    1,
                    nextRow,
                    8));

            nextRow++;

            WriteSingleBranchDetails(
                ws,
                ref nextRow,
                section,
                language);
        }

        // ----------------------------------------------------
        // REAL ACCOUNT / TRANSACTION DETAILS
        // ----------------------------------------------------

        WriteDeepSectionDetails(
            ws,
            ref nextRow,
            sectionName,
            section,
            language);

        foreach(
            var column
            in ws.ColumnsUsed())
        {
            if(column.Width < 14)
                column.Width = 14;

            if(column.Width > 30)
                column.Width = 30;
        }
    }

    private static List<(string Name,JsonElement Value)>
        ExtractSummaryMetrics(
            JsonElement section,
            string sectionName)
    {
        var result =
            new List<(string Name,JsonElement Value)>();

        if(section.ValueKind != JsonValueKind.Object)
            return result;

        foreach(var p in section.EnumerateObject())
        {
            if(IgnoredFields.Contains(p.Name))
                continue;

            if(p.Value.ValueKind == JsonValueKind.Object ||
               p.Value.ValueKind == JsonValueKind.Array ||
               p.Value.ValueKind == JsonValueKind.Null)
            {
                continue;
            }

            if(!FieldKeys.ContainsKey(p.Name) &&
               !IsSpecialPresentationField(p.Name))
            {
                continue;
            }

            result.Add((p.Name,p.Value));
        }

        return result
            .OrderBy(x => MetricOrder(sectionName,x.Name))
            .ThenBy(x => x.Name)
            .ToList();
    }

    private static int WriteObjectSummary(
        IXLWorksheet ws,
        int row,
        string sectionName,
        string name,
        JsonElement obj,
        string language)
    {
        if(obj.ValueKind != JsonValueKind.Object)
            return row;

        ws.Range(row,1,row,8).Merge();

        ws.Cell(row,1).Value =
            ObjectMetricTitle(
                sectionName,
                name,
                language);

        StyleSubHeader(ws.Range(row,1,row,8));

        ws.Row(row).Height = 26;

        row++;

        var values =
            obj.EnumerateObject()
                .Where(x =>
                    x.Value.ValueKind != JsonValueKind.Array &&
                    x.Value.ValueKind != JsonValueKind.Object &&
                    !x.Name.Equals(
                        "branchId",
                        StringComparison.OrdinalIgnoreCase) &&
                    !x.Name.Equals(
                        "financialPeriodId",
                        StringComparison.OrdinalIgnoreCase) &&
                    !x.Name.Equals(
                        "financialUploadId",
                        StringComparison.OrdinalIgnoreCase) &&
                    !x.Name.Equals(
                        "id",
                        StringComparison.OrdinalIgnoreCase) &&
                    !x.Name.Equals(
                        "rowOrder",
                        StringComparison.OrdinalIgnoreCase) &&
                    !x.Name.Equals(
                        "isMarked",
                        StringComparison.OrdinalIgnoreCase) &&
                    !x.Name.Equals(
                        "isMatched",
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

        foreach(var item in values)
        {
            ws.Range(row,1,row,4).Merge();
            ws.Range(row,5,row,8).Merge();

            ws.Cell(row,1).Value =
                ContextualChildLabel(
                    sectionName,
                    name,
                    item.Name,
                    language);

            ws.Cell(row,1).Style.Font.Bold = true;
            ws.Cell(row,1).Style.Font.FontSize = 11;

            ws.Range(row,1,row,8)
                .Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

            ws.Range(row,1,row,8)
                .Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

            SetValue(
                ws.Cell(row,5),
                item.Value,
                language);

            ws.Cell(row,5).Style.Font.FontSize = 12;

            ws.Range(row,1,row,8)
                .Style.Border.BottomBorder =
                    XLBorderStyleValues.Hair;

            ws.Row(row).Height = 23;

            row++;
        }

        return row + 1;
    }

    private static void WriteBranchesTable(
        IXLWorksheet ws,
        ref int row,
        JsonElement branches,
        string language)
    {
        var items =
            branches.EnumerateArray()
                .Where(x => x.ValueKind == JsonValueKind.Object)
                .ToList();

        if(items.Count == 0)
        {
            ws.Cell(row,1).Value =
                T(LocalizationKeys.FinancialAnalysis.NoData,language);

            row++;
            return;
        }

        var columns =
            items
                .SelectMany(x => x.EnumerateObject())
                .Where(x =>
                    x.Value.ValueKind != JsonValueKind.Array &&
                    x.Value.ValueKind != JsonValueKind.Object &&
                    (
                        x.Name.Equals("branchCode",StringComparison.OrdinalIgnoreCase) ||
                        x.Name.Equals("branchName",StringComparison.OrdinalIgnoreCase) ||
                        FieldKeys.ContainsKey(x.Name)
                    ))
                .Select(x => x.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(x =>
                    !x.Equals("branchId",StringComparison.OrdinalIgnoreCase) &&
                    !x.Equals("status",StringComparison.OrdinalIgnoreCase))
                .OrderBy(x =>
                    x.Equals("branchCode",StringComparison.OrdinalIgnoreCase) ? 0 :
                    x.Equals("branchName",StringComparison.OrdinalIgnoreCase) ? 1 : 2)
                .ToList();

        if(columns.Count == 0)
        {
            ws.Cell(row,1).Value =
                T(LocalizationKeys.FinancialAnalysis.NoData,language);

            row++;
            return;
        }

        var headerRow = row;

        for(var c = 0;c < columns.Count;c++)
        {
            ws.Cell(row,c + 1).Value =
                Label(columns[c],language);
        }

        StyleTableHeader(
            ws.Range(
                row,
                1,
                row,
                columns.Count));

        row++;

        foreach(var item in items)
        {
            for(var c = 0;c < columns.Count;c++)
            {
                if(item.TryGetProperty(columns[c],out var value))
                {
                    SetValue(
                        ws.Cell(row,c + 1),
                        value,
                        language);
                }
            }

            if(row % 2 == 0)
            {
                ws.Range(row,1,row,columns.Count)
                    .Style.Fill.BackgroundColor =
                        XLColor.AliceBlue;
            }

            row++;
        }

        var tableRange =
            ws.Range(
                headerRow,
                1,
                row - 1,
                columns.Count);

        tableRange.Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        tableRange.Style.Border.InsideBorder =
            XLBorderStyleValues.Hair;
    }

    private static void WriteSingleBranchDetails(
        IXLWorksheet ws,
        ref int row,
        JsonElement section,
        string language)
    {
        if(section.ValueKind != JsonValueKind.Object)
            return;

        foreach(var p in section.EnumerateObject())
        {
            if(
                p.Value.ValueKind == JsonValueKind.Object ||
                p.Value.ValueKind == JsonValueKind.Array ||
                p.Value.ValueKind == JsonValueKind.Null ||
                IgnoredFields.Contains(p.Name) ||
                IsFinalHiddenReportField(p.Name)
            )
            {
                continue;
            }

            var label =
                FinalDetailLabel(
                    p.Name,
                    language);

            if(string.IsNullOrWhiteSpace(label))
                continue;

            ws.Range(row,1,row,4).Merge();
            ws.Range(row,5,row,8).Merge();

            ws.Cell(row,1).Value =
                label;

            ws.Cell(row,1).Style.Font.Bold =
                true;

            ws.Cell(row,1).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            ws.Cell(row,1).Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            ws.Cell(row,1).Style.Alignment.WrapText =
                true;

            SetFinalDetailValue(
                ws.Cell(row,5),
                p.Name,
                p.Value,
                language);

            ws.Cell(row,5).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            ws.Cell(row,5).Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            ws.Cell(row,5).Style.Alignment.WrapText =
                true;

            ws.Range(row,1,row,8)
                .Style.Border.BottomBorder =
                    XLBorderStyleValues.Hair;

            row++;
        }
    }

    private sealed class FinancialDeepDetailRow
    {
        public string Group { get; init; } =
            string.Empty;

        public string BranchCode { get; init; } =
            string.Empty;

        public string BranchName { get; init; } =
            string.Empty;

        public Dictionary<string,JsonElement> Values { get; init; } =
            new(StringComparer.OrdinalIgnoreCase);
    }

    private static void WriteDeepSectionDetails(
        IXLWorksheet ws,
        ref int row,
        string sectionName,
        JsonElement section,
        string language)
    {
        if(section.ValueKind != JsonValueKind.Object)
            return;

        var hasAnyDetails =
            false;

        if(
            section.TryGetProperty(
                "branches",
                out var branches) &&
            branches.ValueKind ==
                JsonValueKind.Array
        )
        {
            foreach(
                var branch
                in branches.EnumerateArray())
            {
                if(
                    branch.ValueKind !=
                        JsonValueKind.Object ||
                    !branch.TryGetProperty(
                        "details",
                        out var details) ||
                    details.ValueKind !=
                        JsonValueKind.Object
                )
                {
                    continue;
                }

                if(!hasAnyDetails)
                {
                    WriteFinalDetailsMainHeader(
                        ws,
                        ref row,
                        language);

                    hasAnyDetails =
                        true;
                }

                var branchCode =
                    branch.TryGetProperty(
                        "branchCode",
                        out var codeElement)
                        ? codeElement.GetString() ??
                            string.Empty
                        : string.Empty;

                var branchName =
                    branch.TryGetProperty(
                        "branchName",
                        out var nameElement)
                        ? nameElement.GetString() ??
                            string.Empty
                        : string.Empty;

                WriteFinalBranchHeader(
                    ws,
                    ref row,
                    branchCode,
                    branchName);

                WriteFinalDetailObject(
                    ws,
                    ref row,
                    details,
                    language,
                    null);
            }

            return;
        }

        if(
            !section.TryGetProperty(
                "details",
                out var singleDetails) ||
            singleDetails.ValueKind !=
                JsonValueKind.Object
        )
        {
            return;
        }

        WriteFinalDetailsMainHeader(
            ws,
            ref row,
            language);

        WriteFinalDetailObject(
            ws,
            ref row,
            singleDetails,
            language,
            null);
    }

    private static void WriteFinalDetailsMainHeader(
        IXLWorksheet ws,
        ref int row,
        string language)
    {
        row += 2;

        ws.Range(
            row,
            1,
            row,
            8)
            .Merge();

        ws.Cell(
            row,
            1)
            .Value =
                T(
                    LocalizationKeys.FinancialAnalysis.Details,
                    language);

        StyleSectionHeader(
            ws.Range(
                row,
                1,
                row,
                8));

        row += 2;
    }

    private static void WriteFinalBranchHeader(
        IXLWorksheet ws,
        ref int row,
        string branchCode,
        string branchName)
    {
        ws.Range(
            row,
            1,
            row,
            8)
            .Merge();

        ws.Cell(
            row,
            1)
            .Value =
                string.IsNullOrWhiteSpace(branchCode)
                    ? branchName
                    : $"{branchCode} - {branchName}";

        StyleSubHeader(
            ws.Range(
                row,
                1,
                row,
                8));

        row += 2;
    }

    private static void WriteFinalDetailObject(
        IXLWorksheet ws,
        ref int row,
        JsonElement node,
        string language,
        string? groupName)
    {
        if(node.ValueKind != JsonValueKind.Object)
            return;

        var properties =
            node.EnumerateObject()
                .ToList();

        var nested =
            properties
                .Where(
                    x =>
                        x.Value.ValueKind ==
                            JsonValueKind.Object ||
                        x.Value.ValueKind ==
                            JsonValueKind.Array)
                .ToList();

        var isGenderGroup =
            groupName is not null &&
            (
                groupName.Equals(
                    "male",
                    StringComparison.OrdinalIgnoreCase) ||
                groupName.Equals(
                    "female",
                    StringComparison.OrdinalIgnoreCase)
            );

        if(!string.IsNullOrWhiteSpace(groupName))
        {
            ws.Range(
                row,
                1,
                row,
                8)
                .Merge();

            ws.Cell(
                row,
                1)
                .Value =
                    FinalDetailGroupTitle(
                        groupName,
                        language);

            StyleSubHeader(
                ws.Range(
                    row,
                    1,
                    row,
                    8));

            row++;
        }

        // Wrapper DTOs such as expenses/cash/suppliers/customers
        // already have their summary above.
        // Only gender objects keep useful scalar summaries.
        var renderScalars =
            nested.Count == 0 ||
            isGenderGroup;

        if(renderScalars)
        {
            foreach(
                var property
                in properties)
            {
                if(
                    property.Value.ValueKind ==
                        JsonValueKind.Object ||
                    property.Value.ValueKind ==
                        JsonValueKind.Array ||
                    property.Value.ValueKind ==
                        JsonValueKind.Null ||
                    IsFinalHiddenReportField(
                        property.Name)
                )
                {
                    continue;
                }

                var label =
                    FinalDetailLabel(
                        property.Name,
                        language);

                if(string.IsNullOrWhiteSpace(label))
                    continue;

                ws.Range(
                    row,
                    1,
                    row,
                    4)
                    .Merge();

                ws.Range(
                    row,
                    5,
                    row,
                    8)
                    .Merge();

                ws.Cell(
                    row,
                    1)
                    .Value =
                        label;

                ws.Cell(
                    row,
                    1)
                    .Style.Font.Bold =
                        true;

                ws.Cell(
                    row,
                    1)
                    .Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Center;

                ws.Cell(
                    row,
                    1)
                    .Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                SetFinalDetailValue(
                    ws.Cell(
                        row,
                        5),
                    property.Name,
                    property.Value,
                    language);

                ws.Cell(
                    row,
                    5)
                    .Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Center;

                ws.Cell(
                    row,
                    5)
                    .Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                ws.Range(
                    row,
                    1,
                    row,
                    8)
                    .Style.Border.BottomBorder =
                        XLBorderStyleValues.Hair;

                row++;
            }

            if(renderScalars)
                row++;
        }

        foreach(
            var property
            in nested)
        {
            if(
                property.Value.ValueKind ==
                    JsonValueKind.Object)
            {
                WriteFinalDetailObject(
                    ws,
                    ref row,
                    property.Value,
                    language,
                    property.Name);

                continue;
            }

            if(
                property.Value.ValueKind ==
                    JsonValueKind.Array)
            {
                WriteFinalDetailArray(
                    ws,
                    ref row,
                    property.Name,
                    property.Value,
                    language);
            }
        }
    }

    private static void WriteFinalDetailArray(
        IXLWorksheet ws,
        ref int row,
        string arrayName,
        JsonElement array,
        string language)
    {
        var items =
            array.EnumerateArray()
                .Where(
                    x =>
                        x.ValueKind ==
                            JsonValueKind.Object)
                .ToList();

        if(items.Count == 0)
            return;

        var columns =
            items
                .SelectMany(
                    x =>
                        x.EnumerateObject())
                .Where(
                    x =>
                        x.Value.ValueKind !=
                            JsonValueKind.Object &&
                        x.Value.ValueKind !=
                            JsonValueKind.Array &&
                        !IsFinalHiddenReportField(
                            x.Name) &&
                        !string.IsNullOrWhiteSpace(
                            FinalDetailLabel(
                                x.Name,
                                language)))
                .Select(
                    x =>
                        x.Name)
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .OrderBy(
                    FinalDetailColumnOrder)
                .ThenBy(
                    x =>
                        x,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        if(columns.Count == 0)
            return;

        ws.Range(
            row,
            1,
            row,
            Math.Max(
                8,
                columns.Count))
            .Merge();

        ws.Cell(
            row,
            1)
            .Value =
                FinalDetailGroupTitle(
                    arrayName,
                    language);

        StyleSubHeader(
            ws.Range(
                row,
                1,
                row,
                Math.Max(
                    8,
                    columns.Count)));

        row++;

        for(
            var index = 0;
            index < columns.Count;
            index++)
        {
            ws.Cell(
                row,
                index + 1)
                .Value =
                    FinalDetailLabel(
                        columns[index],
                        language);

            ws.Cell(
                row,
                index + 1)
                .Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

            ws.Cell(
                row,
                index + 1)
                .Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

            ws.Cell(
                row,
                index + 1)
                .Style.Alignment.WrapText =
                    true;
        }

        StyleTableHeader(
            ws.Range(
                row,
                1,
                row,
                columns.Count));

        row++;

        foreach(
            var item
            in items)
        {
            for(
                var index = 0;
                index < columns.Count;
                index++)
            {
                if(
                    item.TryGetProperty(
                        columns[index],
                        out var value))
                {
                    SetFinalDetailValue(
                        ws.Cell(
                            row,
                            index + 1),
                        columns[index],
                        value,
                        language);
                }

                ws.Cell(
                    row,
                    index + 1)
                    .Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Center;

                ws.Cell(
                    row,
                    index + 1)
                    .Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                ws.Cell(
                    row,
                    index + 1)
                    .Style.Alignment.WrapText =
                        true;
            }

            row++;
        }

        var tableRange =
            ws.Range(
                row - items.Count - 1,
                1,
                row - 1,
                columns.Count);

        tableRange.Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        tableRange.Style.Border.InsideBorder =
            XLBorderStyleValues.Hair;

        row += 2;
    }

    private static bool IsFinalHiddenReportField(
        string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            return true;

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
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "gender",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "hasCompleteData",
                StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith(
                "hasIncomeStatement",
                StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith(
                "hasFinancialPosition",
                StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith(
                "DetailsCount",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "detailsCount",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "suppliersCount",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "transactionsCount",
                StringComparison.OrdinalIgnoreCase);
    }

    private static int FinalDetailColumnOrder(
        string name)
    {
        if(
            name.Equals(
                "branchCode",
                StringComparison.OrdinalIgnoreCase))
            return 1;

        if(
            name.Equals(
                "branchName",
                StringComparison.OrdinalIgnoreCase))
            return 2;

        if(
            name.Equals(
                "accountCode",
                StringComparison.OrdinalIgnoreCase))
            return 3;

        if(
            name.Equals(
                "accountName",
                StringComparison.OrdinalIgnoreCase))
            return 4;

        if(
            name.Equals(
                "transactionDate",
                StringComparison.OrdinalIgnoreCase))
            return 5;

        if(
            name.Equals(
                "entryNumber",
                StringComparison.OrdinalIgnoreCase))
            return 6;

        if(
            name.Equals(
                "description",
                StringComparison.OrdinalIgnoreCase))
            return 7;

        if(
            name.Equals(
                "debit",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "assets",
                StringComparison.OrdinalIgnoreCase))
            return 8;

        if(
            name.Equals(
                "credit",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "liabilities",
                StringComparison.OrdinalIgnoreCase))
            return 9;

        if(
            name.Equals(
                "balance",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "value",
                StringComparison.OrdinalIgnoreCase))
            return 10;

        return 100;
    }

    private static string? FinalDetailLabel(
        string name,
        string language)
    {
        if(
            name.Equals(
                "branchCode",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.BranchCode,
                language);
        }

        if(
            name.Equals(
                "branchName",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.BranchName,
                language);
        }

        if(
            name.Equals(
                "accountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "customerAccountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "employeeAccountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "supplierAccountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "expenseAccountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "cashGroupAccountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "capitalAccountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "parentAccountCode",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.AccountCode,
                language);
        }

        if(
            name.Equals(
                "accountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "customerAccountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "employeeAccountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "supplierAccountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "expenseAccountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "cashGroupAccountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "capitalAccountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "parentAccountName",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.AccountName,
                language);
        }

        if(
            name.Equals(
                "debit",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Debit,
                language);
        }

        if(
            name.Equals(
                "credit",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Credit,
                language);
        }

        if(
            name.Equals(
                "balance",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Balance,
                language);
        }

        if(
            name.Equals(
                "assets",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Assets,
                language);
        }

        if(
            name.Equals(
                "liabilities",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Liabilities,
                language);
        }

        if(
            name.Equals(
                "value",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Value,
                language);
        }

        if(
            name.Equals(
                "periodDebit",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.PeriodDebit,
                language);
        }

        if(
            name.Equals(
                "periodCredit",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.PeriodCredit,
                language);
        }

        if(
            name.Equals(
                "periodBalance",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.PeriodBalance,
                language);
        }

        if(
            name.Equals(
                "hasFile",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "fileAvailable",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.FileAvailable,
                language);
        }

        if(
            name.Equals(
                "transactionDate",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialExport.TransactionDate,
                language);
        }

        if(
            name.Equals(
                "description",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialExport.Description,
                language);
        }

        if(
            name.Equals(
                "entryNumber",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialExport.EntryNumber,
                language);
        }

        if(
            name.Equals(
                "purchases",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "totalPurchases",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.TotalPurchases,
                language);
        }
        var regular =
            Label(
                name,
                language);

        if(
            string.IsNullOrWhiteSpace(regular) ||
            regular.Equals(
                name,
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return regular;
    }

    private static string FinalDetailGroupTitle(
        string name,
        string language)
    {
        if(
            name.Equals(
                "male",
                StringComparison.OrdinalIgnoreCase))
        {
            return language.Equals(
                "ar",
                StringComparison.OrdinalIgnoreCase)
                ? "رجالي"
                : "Male";
        }

        if(
            name.Equals(
                "female",
                StringComparison.OrdinalIgnoreCase))
        {
            return language.Equals(
                "ar",
                StringComparison.OrdinalIgnoreCase)
                ? "نسائي"
                : "Female";
        }

        if(
            name.Equals(
                "transactions",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Transactions,
                language);
        }

        if(
            name.Equals(
                "customerDetails",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Customers,
                language);
        }

        if(
            name.Equals(
                "employeeDetails",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Employees,
                language);
        }

        if(
            name.Equals(
                "suppliers",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Suppliers,
                language);
        }

        if(
            name.Equals(
                "cashboxes",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Cash,
                language);
        }

        return T(
            LocalizationKeys.FinancialAnalysis.Details,
            language);
    }

    private static void SetFinalDetailValue(
        IXLCell cell,
        string fieldName,
        JsonElement value,
        string language)
    {
        if(
            fieldName.Equals(
                "transactionDate",
                StringComparison.OrdinalIgnoreCase) &&
            value.ValueKind ==
                JsonValueKind.String)
        {
            var raw =
                value.GetString();

            if(
                !string.IsNullOrWhiteSpace(raw) &&
                DateTime.TryParse(
                    raw,
                    out var parsed) &&
                parsed.Year <= 1)
            {
                cell.Value =
                    string.Empty;

                return;
            }
        }

        SetValue(
            cell,
            value,
            language);
    }

    private static void CollectDeepSectionDetails(
        JsonElement section,
        List<FinancialDeepDetailRow> rows)
    {
        if(section.ValueKind !=
            JsonValueKind.Object)
        {
            return;
        }

        if(
            section.TryGetProperty(
                "branches",
                out var branches) &&
            branches.ValueKind ==
                JsonValueKind.Array
        )
        {
            foreach(
                var branch
                in branches.EnumerateArray())
            {
                CollectDeepBranchDetails(
                    branch,
                    rows);
            }

            return;
        }

        CollectDeepBranchDetails(
            section,
            rows);
    }

    private static void CollectDeepBranchDetails(
        JsonElement branch,
        List<FinancialDeepDetailRow> rows)
    {
        if(branch.ValueKind !=
            JsonValueKind.Object)
        {
            return;
        }

        var branchCode =
            GetDeepString(
                branch,
                "branchCode");

        var branchName =
            GetDeepString(
                branch,
                "branchName");

        foreach(
            var property
            in branch.EnumerateObject())
        {
            if(
                property.Name.Equals(
                    "branchId",
                    StringComparison.OrdinalIgnoreCase) ||
                property.Name.Equals(
                    "branchCode",
                    StringComparison.OrdinalIgnoreCase) ||
                property.Name.Equals(
                    "branchName",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                continue;
            }

            if(
                property.Value.ValueKind !=
                    JsonValueKind.Object &&
                property.Value.ValueKind !=
                    JsonValueKind.Array
            )
            {
                continue;
            }

            CollectNestedDetails(
                property.Name,
                property.Value,
                branchCode,
                branchName,
                rows);
        }
    }

    private static void CollectNestedDetails(
        string group,
        JsonElement value,
        string branchCode,
        string branchName,
        List<FinancialDeepDetailRow> rows)
    {
        if(value.ValueKind ==
            JsonValueKind.Null)
        {
            return;
        }

        if(value.ValueKind ==
            JsonValueKind.Array)
        {
            foreach(
                var item
                in value.EnumerateArray())
            {
                if(item.ValueKind ==
                    JsonValueKind.Object)
                {
                    CollectNestedDetails(
                        group,
                        item,
                        branchCode,
                        branchName,
                        rows);
                }
            }

            return;
        }

        if(value.ValueKind !=
            JsonValueKind.Object)
        {
            return;
        }

        var values =
            new Dictionary<string,JsonElement>(
                StringComparer.OrdinalIgnoreCase);

        var nested =
            new List<JsonProperty>();

        foreach(
            var property
            in value.EnumerateObject())
        {
            if(
                property.Value.ValueKind ==
                    JsonValueKind.Array ||
                property.Value.ValueKind ==
                    JsonValueKind.Object
            )
            {
                nested.Add(
                    property);

                continue;
            }

            if(
                property.Value.ValueKind ==
                    JsonValueKind.Null ||
                property.Name.Equals(
                    "financialPeriodId",
                    StringComparison.OrdinalIgnoreCase) ||
                property.Name.Equals(
                    "branchId",
                    StringComparison.OrdinalIgnoreCase) ||
                property.Name.Equals(
                    "uploadId",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                continue;
            }

            values[property.Name] =
                property.Value.Clone();
        }

        // If this object contains nested detail collections,
        // its scalar fields are summary/metadata already shown
        // elsewhere in the report. Do not print them again.
        if(
            values.Count > 0 &&
            nested.Count == 0
        )
        {
            rows.Add(
                new FinancialDeepDetailRow
                {
                    Group =
                        group,

                    BranchCode =
                        branchCode,

                    BranchName =
                        branchName,

                    Values =
                        values
                });
        }

        foreach(
            var child
            in nested)
        {
            var childGroup =
                string.IsNullOrWhiteSpace(group)
                    ? child.Name
                    : $"{group}.{child.Name}";

            CollectNestedDetails(
                childGroup,
                child.Value,
                branchCode,
                branchName,
                rows);
        }
    }

    private static void WriteDeepDetailGroup(
        IXLWorksheet ws,
        ref int row,
        string sectionName,
        string groupName,
        IReadOnlyCollection<FinancialDeepDetailRow> rows,
        string language)
    {
        if(rows.Count == 0)
            return;

        ws.Range(
                row,
                1,
                row,
                8)
            .Merge();

        ws.Cell(
                row,
                1)
            .Value =
                $"{SectionTitle(sectionName,language)} - {DeepGroupTitle(groupName,language)}";

        StyleSubHeader(
            ws.Range(
                row,
                1,
                row,
                8));

        row++;

        var columns =
            rows
                .SelectMany(
                    x =>
                        x.Values.Keys)
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .OrderBy(
                    DeepColumnOrder)
                .ThenBy(
                    x =>
                        x,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        if(columns.Count == 0)
            return;

        var column =
            1;

        ws.Cell(
                row,
                column++)
            .Value =
                T(
                    LocalizationKeys.FinancialAnalysis.BranchCode,
                    language);

        ws.Cell(
                row,
                column++)
            .Value =
                T(
                    LocalizationKeys.FinancialAnalysis.BranchName,
                    language);

        foreach(
            var name
            in columns)
        {
            ws.Cell(
                    row,
                    column++)
                .Value =
                    DeepDetailLabel(
                        name,
                        language);
        }

        var totalColumns =
            column - 1;

        StyleTableHeader(
            ws.Range(
                row,
                1,
                row,
                totalColumns));

        row++;

        foreach(
            var detail
            in rows
                .OrderBy(
                    x =>
                        x.BranchCode,
                    StringComparer.OrdinalIgnoreCase))
        {
            column =
                1;

            ws.Cell(
                    row,
                    column++)
                .Value =
                    detail.BranchCode;

            ws.Cell(
                    row,
                    column++)
                .Value =
                    detail.BranchName;

            foreach(
                var name
                in columns)
            {
                if(
                    detail.Values.TryGetValue(
                        name,
                        out var value)
                )
                {
                    SetValue(
                        ws.Cell(
                            row,
                            column),
                        value,
                        language);
                }

                column++;
            }

            if(row % 2 == 0)
            {
                ws.Range(
                        row,
                        1,
                        row,
                        totalColumns)
                    .Style.Fill.BackgroundColor =
                        XLColor.AliceBlue;
            }

            row++;
        }

        ws.Range(
                row - rows.Count - 1,
                1,
                row - 1,
                totalColumns)
            .Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

        ws.Range(
                row - rows.Count - 1,
                1,
                row - 1,
                totalColumns)
            .Style.Border.InsideBorder =
                XLBorderStyleValues.Hair;
    }

    private static string GetDeepString(
        JsonElement element,
        string name)
    {
        if(
            element.TryGetProperty(
                name,
                out var value)
        )
        {
            return value.ValueKind ==
                JsonValueKind.String
                    ? value.GetString() ??
                        string.Empty
                    : value.ToString();
        }

        return string.Empty;
    }

    private static int DeepColumnOrder(
        string name)
    {
        return name.ToLowerInvariant() switch
        {
            "accountcode" => 10,
            "accountname" => 20,
            "gender" => 30,
            "debit" => 40,
            "credit" => 50,
            "balance" => 60,
            "value" => 70,
            "resulttype" => 80,
            "resultvalue" => 90,
            "signedvalue" => 100,
            "roworder" => 900,
            _ => 500
        };
    }

    private static string DeepDetailLabel(
        string name,
        string language)
    {
        if(name.Equals(
            "value",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Value,
                language);
        }

        if(name.Equals(
            "gender",
            StringComparison.OrdinalIgnoreCase))
        {
            return language == "ar"
                ? "الجنس"
                : "Gender";
        }

        var label =
            Label(
                name,
                language);

        if(!string.Equals(
            label,
            name,
            StringComparison.Ordinal))
        {
            return label;
        }

        return SplitDeepCamelCase(
            name);
    }

    private static string DeepGroupTitle(
        string path,
        string language)
    {
        var parts =
            path.Split(
                '.',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);

        var leaf =
            parts.LastOrDefault() ??
            path;

        string TranslatePart(
            string value)
        {
            if(value.Equals(
                "details",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Details,
                    language);
            }

            if(value.Equals(
                "records",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Records,
                    language);
            }

            if(value.Equals(
                "transactions",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Transactions,
                    language);
            }

            if(value.Equals(
                "male",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Male,
                    language);
            }

            if(value.Equals(
                "female",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Female,
                    language);
            }

            if(value.Equals(
                "cashboxes",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Cash,
                    language);
            }

            if(value.Equals(
                "suppliers",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Suppliers,
                    language);
            }

            if(value.Equals(
                "customerDetails",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Customers,
                    language);
            }

            if(value.Equals(
                "employeeDetails",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Employees,
                    language);
            }

            var label =
                Label(
                    value,
                    language);

            return !string.Equals(
                label,
                value,
                StringComparison.Ordinal)
                    ? label
                    : SplitDeepCamelCase(
                        value);
        }

        if(
            parts.Any(
                x =>
                    x.Equals(
                        "male",
                        StringComparison.OrdinalIgnoreCase))
        )
        {
            return
                $"{T(LocalizationKeys.FinancialAnalysis.Male,language)} - {TranslatePart(leaf)}";
        }

        if(
            parts.Any(
                x =>
                    x.Equals(
                        "female",
                        StringComparison.OrdinalIgnoreCase))
        )
        {
            return
                $"{T(LocalizationKeys.FinancialAnalysis.Female,language)} - {TranslatePart(leaf)}";
        }

        return TranslatePart(
            leaf);
    }

    private static string SplitDeepCamelCase(
        string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return value;

        return System.Text.RegularExpressions.Regex.Replace(
            value,
            "([a-z0-9])([A-Z])",
            "$1 $2");
    }
    private static void WriteKpiCard(
        IXLWorksheet ws,
        int row,
        int column,
        string title,
        string value)
    {
        var titleRange =
            ws.Range(
                row,
                column,
                row,
                column + 1);

        var valueRange =
            ws.Range(
                row + 1,
                column,
                row + 1,
                column + 1);

        titleRange.Merge();
        valueRange.Merge();

        titleRange.Value = title;
        valueRange.Value = value;

        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 12;
        titleRange.Style.Font.FontColor = XLColor.DarkBlue;
        titleRange.Style.Fill.BackgroundColor = XLColor.AliceBlue;
        titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        valueRange.Style.Font.Bold = true;
        valueRange.Style.Font.FontSize = 15;
        valueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        valueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        var card =
            ws.Range(
                row,
                column,
                row + 1,
                column + 1);

        card.Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        card.Style.Border.OutsideBorderColor =
            XLColor.LightGray;

        ws.Row(row).Height = 28;
        ws.Row(row + 1).Height = 34;
    }

    private static void CleanWorkbookLikePdf(
        XLWorkbook workbook,
        string language)
    {
        var removableLabels =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                T(
                    LocalizationKeys.FinancialAnalysis.ExpectedFiles,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.ValidFiles,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.MissingFiles,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.CompletionPercentage,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.BranchesWithData,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.BranchesWithoutData,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.CompleteBranches,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.IncompleteBranches,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.MaleFiles,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.FemaleFiles,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.Records,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.Transactions,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.GeneratedAt,
                    language),

                T(
                    LocalizationKeys.FinancialAnalysis.IncludedSections,
                    language)
            };

        foreach(
            var ws
            in workbook.Worksheets)
        {
            var used =
                ws.RangeUsed();

            if(used is null)
                continue;

            var lastRow =
                used.LastRow()
                    .RowNumber();

            var lastColumn =
                used.LastColumn()
                    .ColumnNumber();

            var cellsToClear =
                new List<(int Row,int Column)>();

            for(
                var row = 1;
                row <= lastRow;
                row++)
            {
                for(
                    var column = 1;
                    column <= lastColumn;
                    column++)
                {
                    var text =
                        ws.Cell(
                                row,
                                column)
                            .GetFormattedString()
                            .Trim();

                    if(
                        !string.IsNullOrWhiteSpace(text) &&
                        removableLabels.Contains(text))
                    {
                        cellsToClear.Add(
                            (row,column));
                    }
                }
            }

            foreach(
                var item
                in cellsToClear)
            {
                var cell =
                    ws.Cell(
                        item.Row,
                        item.Column);

                var merged =
                    ws.MergedRanges
                        .FirstOrDefault(
                            x =>
                                x.Contains(cell));

                var firstColumn =
                    item.Column;

                var lastMergedColumn =
                    item.Column;

                if(merged is not null)
                {
                    firstColumn =
                        merged.RangeAddress
                            .FirstAddress
                            .ColumnNumber;

                    lastMergedColumn =
                        merged.RangeAddress
                            .LastAddress
                            .ColumnNumber;

                    merged.Unmerge();
                }

                ws.Range(
                        item.Row,
                        firstColumn,
                        item.Row,
                        lastMergedColumn)
                    .Clear(
                        XLClearOptions.Contents);

                if(
                    item.Row + 1 <=
                    (ws.LastRowUsed()?.RowNumber() ??
                     item.Row))
                {
                    var valueCell =
                        ws.Cell(
                            item.Row + 1,
                            firstColumn);

                    var valueMerged =
                        ws.MergedRanges
                            .FirstOrDefault(
                                x =>
                                    x.Contains(
                                        valueCell));

                    if(valueMerged is not null)
                    {
                        var valueFirst =
                            valueMerged.RangeAddress
                                .FirstAddress
                                .ColumnNumber;

                        var valueLast =
                            valueMerged.RangeAddress
                                .LastAddress
                                .ColumnNumber;

                        valueMerged.Unmerge();

                        ws.Range(
                                item.Row + 1,
                                valueFirst,
                                item.Row + 1,
                                valueLast)
                            .Clear(
                                XLClearOptions.Contents);
                    }
                    else
                    {
                        ws.Range(
                                item.Row + 1,
                                firstColumn,
                                item.Row + 1,
                                lastMergedColumn)
                            .Clear(
                                XLClearOptions.Contents);
                    }
                }
            }

            // Executive summary: remove included-section list.
            if(ws.Position == 1)
            {
                var endRow =
                    ws.LastRowUsed()?
                        .RowNumber() ?? 0;

                if(endRow >= 14)
                {
                    ws.Rows(
                            14,
                            endRow)
                        .Delete();
                }
            }
        }
    }

    private static void ClearInformationCard(
        IXLWorksheet ws,
        int row,
        int column)
    {
        var merged =
            ws.MergedRanges
                .FirstOrDefault(
                    x =>
                        x.Contains(
                            ws.Cell(
                                row,
                                column)));

        if(merged is not null)
        {
            var firstRow =
                merged.RangeAddress
                    .FirstAddress
                    .RowNumber;

            var firstColumn =
                merged.RangeAddress
                    .FirstAddress
                    .ColumnNumber;

            var lastColumn =
                merged.RangeAddress
                    .LastAddress
                    .ColumnNumber;

            merged.Unmerge();

            ws.Range(
                    firstRow,
                    firstColumn,
                    Math.Min(
                        firstRow + 1,
                        ws.LastRowUsed()?.RowNumber() ??
                        firstRow + 1),
                    lastColumn)
                .Clear(
                    XLClearOptions.Contents);

            return;
        }

        ws.Cell(
                row,
                column)
            .Clear(
                XLClearOptions.Contents);

        if(
            row <
            (ws.LastRowUsed()?.RowNumber() ?? row))
        {
            ws.Cell(
                    row + 1,
                    column)
                .Clear(
                    XLClearOptions.Contents);
        }
    }

    private static void FinalizeWorkbookPresentation(
        XLWorkbook workbook,
        string language)
    {
        var isArabic =
            language == "ar";

        foreach(
            var ws
            in workbook.Worksheets)
        {
            ws.RightToLeft =
                isArabic;

            ws.Style.Font.FontName =
                "Arial";

            ws.Style.Font.FontSize =
                10;

            ws.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            ws.Style.Alignment.WrapText =
                true;

            var used =
                ws.RangeUsed();

            if(used is null)
                continue;

            var lastRow =
                used.LastRow()
                    .RowNumber();

            var lastColumn =
                Math.Max(
                    8,
                    used.LastColumn()
                        .ColumnNumber());

            // Main identification columns
            ws.Column(1).Width =
                25;

            ws.Column(2).Width =
                36;

            for(
                var column = 3;
                column <= lastColumn;
                column++)
            {
                ws.Column(column).Width =
                    22;
            }

            for(
                var row = 1;
                row <= lastRow;
                row++)
            {
                if(
                    ws.Row(row).Height <
                    22)
                {
                    ws.Row(row).Height =
                        22;
                }

                for(
                    var column = 1;
                    column <= lastColumn;
                    column++)
                {
                    var cell =
                        ws.Cell(
                            row,
                            column);

                    if(
                        cell.DataType ==
                        XLDataType.Number)
                    {
                        cell.Style
                            .NumberFormat
                            .Format =
                            "#,##0.00;[Red]-#,##0.00;0.00";
                    }
                }
            }

            ws.Row(1).Height =
                34;

            ws.Row(2).Height =
                34;

            used.Style.Alignment.WrapText =
                true;

            used.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            if(ws.Position == 1)
            {
                ws.SheetView
                    .FreezeRows(5);

                for(
                    var column = 1;
                    column <= 8;
                    column++)
                {
                    ws.Column(column).Width =
                        27;
                }
            }
            else
            {
                ws.SheetView
                    .FreezeRows(3);
            }
        }
    }
    private static bool IsTechnicalExcelMetric(
        string name)
    {
        return
            name.Equals(
                "status",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "activeBranches",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "expectedFiles",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "validFiles",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "missingFiles",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "completionPercentage",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "branchesWithData",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "branchesWithoutData",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "branchesWithAnyData",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "branchesWithCompleteData",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "branchesWithoutCompleteData",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "completeBranches",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "incompleteBranches",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "maleFilesCount",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "femaleFilesCount",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "maleRecordsCount",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "femaleRecordsCount",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "transactionsCount",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "financialPeriodId",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "startDate",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "endDate",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "branchId",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "uploadId",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "financialUploadId",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "rowOrder",
                StringComparison.OrdinalIgnoreCase) ||

            name.Equals(
                "gender",
                StringComparison.OrdinalIgnoreCase) ||

            name.StartsWith(
                "has",
                StringComparison.OrdinalIgnoreCase);
    }
    private static void PrepareSheet(
        IXLWorksheet ws,
        string language)
    {
        ws.RightToLeft = language == "ar";

        ws.SheetView.FreezeRows(2);

        ws.Style.Font.FontName = "Arial";
        ws.Style.Font.FontSize = 12;

        ws.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        ws.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        ws.Style.Alignment.WrapText = true;

        ws.Row(1).Height = 32;
        ws.Row(2).Height = 32;

        for(var i = 1;i <= 8;i++)
        {
            ws.Column(i).Width = 19;
        }
    }

    private static void StyleMainTitle(
        IXLRange range)
    {
        range.Style.Fill.BackgroundColor =
            XLColor.DarkBlue;

        range.Style.Font.FontColor =
            XLColor.White;

        range.Style.Font.Bold = true;
        range.Style.Font.FontSize = 20;

        range.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        range.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;
    }

    private static void StyleMetaHeader(
        IXLRange range)
    {
        range.Style.Fill.BackgroundColor =
            XLColor.LightBlue;

        range.Style.Font.Bold = true;

        range.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        range.Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        range.Style.Border.InsideBorder =
            XLBorderStyleValues.Thin;
    }

    private static void StyleMetaValue(
        IXLRange range)
    {
        range.Style.Font.Bold = true;

        range.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        range.Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        range.Style.Border.InsideBorder =
            XLBorderStyleValues.Thin;
    }

    private static void StyleSectionHeader(
        IXLRange range)
    {
        range.Style.Fill.BackgroundColor =
            XLColor.DarkBlue;

        range.Style.Font.FontColor =
            XLColor.White;

        range.Style.Font.Bold = true;
        range.Style.Font.FontSize = 14;

        range.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
    }

    private static void StyleSubHeader(
        IXLRange range)
    {
        range.Style.Fill.BackgroundColor =
            XLColor.LightBlue;

        range.Style.Font.Bold = true;

        range.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
    }

    private static void StyleTableHeader(
        IXLRange range)
    {
        range.Style.Fill.BackgroundColor =
            XLColor.DarkBlue;

        range.Style.Font.FontColor =
            XLColor.White;

        range.Style.Font.Bold = true;

        range.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        range.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        range.Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        range.Style.Border.InsideBorder =
            XLBorderStyleValues.Thin;
    }

    private static void SetValue(
        IXLCell cell,
        JsonElement value,
        string language)
    {
        if(value.ValueKind == JsonValueKind.String)
        {
            var raw =
                value.GetString();

            if(!string.IsNullOrWhiteSpace(raw))
            {
                if(
                    raw.Equals(
                        "Profit",
                        StringComparison.OrdinalIgnoreCase))
                {
                    cell.Value =
                        language.Equals(
                            "ar",
                            StringComparison.OrdinalIgnoreCase)
                            ? "ربح"
                            : "Profit";

                    return;
                }

                if(
                    raw.Equals(
                        "Loss",
                        StringComparison.OrdinalIgnoreCase))
                {
                    cell.Value =
                        language.Equals(
                            "ar",
                            StringComparison.OrdinalIgnoreCase)
                            ? "خسارة"
                            : "Loss";

                    return;
                }

                if(
                    raw.Equals(
                        "Male",
                        StringComparison.OrdinalIgnoreCase))
                {
                    cell.Value =
                        language.Equals(
                            "ar",
                            StringComparison.OrdinalIgnoreCase)
                            ? "رجالي"
                            : "Male";

                    return;
                }

                if(
                    raw.Equals(
                        "Female",
                        StringComparison.OrdinalIgnoreCase))
                {
                    cell.Value =
                        language.Equals(
                            "ar",
                            StringComparison.OrdinalIgnoreCase)
                            ? "نسائي"
                            : "Female";

                    return;
                }
            }
        }

        switch(value.ValueKind)
        {
            case JsonValueKind.Number:
                if(value.TryGetDecimal(out var d))
                {
                    cell.Value = d;
                    cell.Style.NumberFormat.Format =
                        "#,##0.00;[Red]-#,##0.00";
                }
                break;

            case JsonValueKind.True:
                cell.Value =
                    T(LocalizationKeys.FinancialAnalysis.Yes,language);
                break;

            case JsonValueKind.False:
                cell.Value =
                    T(LocalizationKeys.FinancialAnalysis.No,language);
                break;

            case JsonValueKind.String:
                var text = value.GetString() ?? string.Empty;

                if(text.Equals(
                    "Profit",
                    StringComparison.OrdinalIgnoreCase))
                {
                    cell.Value =
                        T(
                            LocalizationKeys.FinancialAnalysis.Profit,
                            language);

                    break;
                }

                if(text.Equals(
                    "Loss",
                    StringComparison.OrdinalIgnoreCase))
                {
                    cell.Value =
                        T(
                            LocalizationKeys.FinancialAnalysis.Loss,
                            language);

                    break;
                }

                if(text.Equals(
                    "BreakEven",
                    StringComparison.OrdinalIgnoreCase) ||
                   text.Equals(
                    "Break-even",
                    StringComparison.OrdinalIgnoreCase))
                {
                    cell.Value =
                        T(
                            LocalizationKeys.FinancialAnalysis.BreakEven,
                            language);

                    break;
                }

                if(text.Equals(
                    "Male",
                    StringComparison.OrdinalIgnoreCase))
                {
                    cell.Value =
                        T(
                            LocalizationKeys.FinancialAnalysis.Male,
                            language);

                    break;
                }

                if(text.Equals(
                    "Female",
                    StringComparison.OrdinalIgnoreCase))
                {
                    cell.Value =
                        T(
                            LocalizationKeys.FinancialAnalysis.Female,
                            language);

                    break;
                }
                if(DateTime.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
                {
                    cell.Value =
                        date.ToString("dd/MM/yyyy");
                }
                else
                {
                    cell.Value = text;
                }
                break;

            default:
                cell.Value = string.Empty;
                break;
        }
    }

    private static string FormatElement(
        JsonElement value,
        string language)
    {
        // FORMAT ELEMENT LOCALIZATION FIX
        if(value.ValueKind == JsonValueKind.String)
        {
            var text =
                value.GetString() ??
                string.Empty;

            if(
                text.Equals(
                    "Profit",
                    StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Profit,
                    language);
            }

            if(
                text.Equals(
                    "Loss",
                    StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Loss,
                    language);
            }

            if(
                text.Equals(
                    "BreakEven",
                    StringComparison.OrdinalIgnoreCase) ||
                text.Equals(
                    "Break-even",
                    StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.BreakEven,
                    language);
            }

            if(
                text.Equals(
                    "Male",
                    StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Male,
                    language);
            }

            if(
                text.Equals(
                    "Female",
                    StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.Female,
                    language);
            }
        }

        return value.ValueKind switch
        {
            JsonValueKind.Number =>
                value.TryGetDecimal(out var d)
                    ? d.ToString("#,##0.00")
                    : value.ToString(),

            JsonValueKind.True =>
                T(LocalizationKeys.FinancialAnalysis.Yes,language),

            JsonValueKind.False =>
                T(LocalizationKeys.FinancialAnalysis.No,language),

            JsonValueKind.String =>
                value.GetString() ?? string.Empty,

            _ =>
                value.ToString()
        };
    }

    private static string Label(
        string name,
        string language)
    {
        // ----------------------------------------------------
        // Inventory
        // ----------------------------------------------------

        if(name.Equals(
            "averageOpeningInventory",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Average,language)} - {T(LocalizationKeys.FinancialAnalysis.OpeningInventory,language)}";
        }

        if(name.Equals(
            "averageClosingInventory",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Average,language)} - {T(LocalizationKeys.FinancialAnalysis.ClosingInventory,language)}";
        }

        // ----------------------------------------------------
        // Settlements
        // ----------------------------------------------------

        if(name.Equals(
            "allBranchesMaleDebit",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Debit,language)} - {T(LocalizationKeys.FinancialAnalysis.Male,language)}";
        }

        if(name.Equals(
            "allBranchesMaleCredit",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Credit,language)} - {T(LocalizationKeys.FinancialAnalysis.Male,language)}";
        }

        if(name.Equals(
            "allBranchesMaleBalance",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Balance,language)} - {T(LocalizationKeys.FinancialAnalysis.Male,language)}";
        }

        if(name.Equals(
            "allBranchesFemaleDebit",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Debit,language)} - {T(LocalizationKeys.FinancialAnalysis.Female,language)}";
        }

        if(name.Equals(
            "allBranchesFemaleCredit",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Credit,language)} - {T(LocalizationKeys.FinancialAnalysis.Female,language)}";
        }

        if(name.Equals(
            "allBranchesFemaleBalance",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Balance,language)} - {T(LocalizationKeys.FinancialAnalysis.Female,language)}";
        }

        if(name.Equals(
            "allBranchesDebit",
            StringComparison.OrdinalIgnoreCase) ||
           name.Equals(
            "totalDebit",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Total,language)} {T(LocalizationKeys.FinancialAnalysis.Debit,language)}";
        }

        if(name.Equals(
            "allBranchesCredit",
            StringComparison.OrdinalIgnoreCase) ||
           name.Equals(
            "totalCredit",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Total,language)} {T(LocalizationKeys.FinancialAnalysis.Credit,language)}";
        }

        if(name.Equals(
            "allBranchesBalance",
            StringComparison.OrdinalIgnoreCase) ||
           name.Equals(
            "totalBalance",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Total,language)} {T(LocalizationKeys.FinancialAnalysis.Balance,language)}";
        }

        if(name.Equals(
            "averageCompleteBranchBalance",
            StringComparison.OrdinalIgnoreCase))
        {
            return $"{T(LocalizationKeys.FinancialAnalysis.Average,language)} - {T(LocalizationKeys.FinancialAnalysis.Balance,language)}";
        }

        // ----------------------------------------------------
        // General known fields
        // ----------------------------------------------------

        // ----------------------------------------------------
        // Single-branch / deep-detail aliases
        // ----------------------------------------------------

        if(
            name.Equals(
                "expenseAccountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "cashGroupAccountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "supplierAccountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "capitalAccountCode",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "parentAccountCode",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.AccountCode,
                language);
        }

        if(
            name.Equals(
                "expenseAccountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "cashGroupAccountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "supplierAccountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "capitalAccountName",
                StringComparison.OrdinalIgnoreCase) ||
            name.Equals(
                "parentAccountName",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.AccountName,
                language);
        }

        if(
            name.Equals(
                "customerBalance",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.CustomerBalance,
                language);
        }

        if(
            name.Equals(
                "employeeBalance",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.EmployeeBalance,
                language);
        }

        if(
            name.Equals(
                "totalExpenses",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.TotalExpenses,
                language);
        }

        if(
            name.Equals(
                "value",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Value,
                language);
        }

        if(
            name.Equals(
                "details",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Details,
                language);
        }

        if(
            name.Equals(
                "records",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Records,
                language);
        }

        if(
            name.Equals(
                "transactions",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Transactions,
                language);
        }
        if(FieldKeys.TryGetValue(name,out var key))
        {
            return T(key,language);
        }

        if(name.Contains(
            "highest",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Highest,
                language);
        }

        if(name.Contains(
            "lowest",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Lowest,
                language);
        }

        if(name.Contains(
            "average",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Average,
                language);
        }

        if(name.Contains(
            "total",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Total,
                language);
        }

        return name;
    }

    private static bool IsSpecialPresentationField(
        string name)
    {
        return
            name.Equals("averageOpeningInventory",StringComparison.OrdinalIgnoreCase) ||
            name.Equals("averageClosingInventory",StringComparison.OrdinalIgnoreCase) ||
            name.Equals("allBranchesMaleDebit",StringComparison.OrdinalIgnoreCase) ||
            name.Equals("allBranchesMaleCredit",StringComparison.OrdinalIgnoreCase) ||
            name.Equals("allBranchesMaleBalance",StringComparison.OrdinalIgnoreCase) ||
            name.Equals("allBranchesFemaleDebit",StringComparison.OrdinalIgnoreCase) ||
            name.Equals("allBranchesFemaleCredit",StringComparison.OrdinalIgnoreCase) ||
            name.Equals("allBranchesFemaleBalance",StringComparison.OrdinalIgnoreCase) ||
            name.Equals("allBranchesDebit",StringComparison.OrdinalIgnoreCase) ||
            name.Equals("allBranchesCredit",StringComparison.OrdinalIgnoreCase) ||
            name.Equals("allBranchesBalance",StringComparison.OrdinalIgnoreCase);
    }

    private static int MetricOrder(
        string section,
        string name)
    {
        var s = section.ToLowerInvariant();

        if(s == "inventory")
        {
            return name.ToLowerInvariant() switch
            {
                "brancheswithdata" => 10,
                "brancheswithoutdata" => 20,
                "allbranchesopeningtotal" => 30,
                "allbranchesclosingtotal" => 40,
                "inventorychange" => 50,
                "averageopeninginventory" => 60,
                "averageclosinginventory" => 70,
                _ => 500
            };
        }

        if(s == "settlements")
        {
            return name.ToLowerInvariant() switch
            {
                "malefilescount" => 10,
                "femalefilescount" => 20,
                "brancheswithanydata" => 30,
                "brancheswithcompletedata" => 40,
                "brancheswithoutcompletedata" => 50,

                "allbranchesmaledebit" => 100,
                "allbranchesmalecredit" => 110,
                "allbranchesmalebalance" => 120,

                "allbranchesfemaledebit" => 200,
                "allbranchesfemalecredit" => 210,
                "allbranchesfemalebalance" => 220,

                "allbranchesdebit" => 300,
                "allbranchescredit" => 310,
                "allbranchesbalance" => 320,

                "averagecompletebranchbalance" => 330,

                _ => 500
            };
        }

        return 500;
    }

    private static string ObjectMetricTitle(
        string section,
        string name,
        string language)
    {
        // OBJECT METRIC LOCALIZATION FIX
        if(
            name.Equals(
                "male",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Male,
                language);
        }

        if(
            name.Equals(
                "female",
                StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Female,
                language);
        }

        var isHighest =
            name.Contains(
                "highest",
                StringComparison.OrdinalIgnoreCase);

        var isLowest =
            name.Contains(
                "lowest",
                StringComparison.OrdinalIgnoreCase);

        var direction =
            isHighest
                ? T(LocalizationKeys.FinancialAnalysis.Highest,language)
                : isLowest
                    ? T(LocalizationKeys.FinancialAnalysis.Lowest,language)
                    : Label(name,language);

        if(section.Equals(
            "inventory",
            StringComparison.OrdinalIgnoreCase))
        {
            if(name.Contains(
                "opening",
                StringComparison.OrdinalIgnoreCase))
            {
                return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.OpeningInventory,language)}";
            }

            if(name.Contains(
                "closing",
                StringComparison.OrdinalIgnoreCase))
            {
                return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.ClosingInventory,language)}";
            }
        }

        if(section.Equals(
            "customers",
            StringComparison.OrdinalIgnoreCase))
        {
            if(name.Contains(
                "customer",
                StringComparison.OrdinalIgnoreCase))
            {
                return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.CustomerBalance,language)}";
            }

            if(name.Contains(
                "employee",
                StringComparison.OrdinalIgnoreCase))
            {
                return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.EmployeeBalance,language)}";
            }
        }

        if(section.Equals(
            "settlements",
            StringComparison.OrdinalIgnoreCase) &&
           (isHighest || isLowest))
        {
            return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.Balance,language)}";
        }

        if(section.Equals(
            "transfers",
            StringComparison.OrdinalIgnoreCase) &&
           (isHighest || isLowest))
        {
            return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.TotalTransfer,language)}";
        }

        if(section.Equals(
            "debt",
            StringComparison.OrdinalIgnoreCase) &&
           (isHighest || isLowest))
        {
            return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.TotalDebt,language)}";
        }

        if(section.Equals(
            "cash",
            StringComparison.OrdinalIgnoreCase) &&
           (isHighest || isLowest))
        {
            return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.TotalCash,language)}";
        }

        if(section.Equals(
            "suppliers",
            StringComparison.OrdinalIgnoreCase) &&
           (isHighest || isLowest))
        {
            return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.SuppliersTotal,language)}";
        }

        if(section.Equals(
            "capital",
            StringComparison.OrdinalIgnoreCase) &&
           (isHighest || isLowest))
        {
            return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.CapitalValue,language)}";
        }

        if(section.Equals(
            "sales",
            StringComparison.OrdinalIgnoreCase) &&
           (isHighest || isLowest))
        {
            return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.NetSales,language)}";
        }

        if(section.Equals(
            "expenses",
            StringComparison.OrdinalIgnoreCase) &&
           (isHighest || isLowest))
        {
            return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.TotalExpenses,language)}";
        }

        if(section.Equals(
            "purchases",
            StringComparison.OrdinalIgnoreCase) &&
           (isHighest || isLowest))
        {
            return $"{direction} - {T(LocalizationKeys.FinancialAnalysis.TotalPurchases,language)}";
        }

        return direction;
    }

    private static string ContextualChildLabel(
        string section,
        string metricName,
        string childName,
        string language)
    {
        if(!childName.Equals(
            "value",
            StringComparison.OrdinalIgnoreCase))
        {
            return Label(
                childName,
                language);
        }

        if(section.Equals(
            "inventory",
            StringComparison.OrdinalIgnoreCase))
        {
            if(metricName.Contains(
                "opening",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.OpeningInventory,
                    language);
            }

            if(metricName.Contains(
                "closing",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.ClosingInventory,
                    language);
            }
        }

        if(section.Equals(
            "customers",
            StringComparison.OrdinalIgnoreCase))
        {
            if(metricName.Contains(
                "employee",
                StringComparison.OrdinalIgnoreCase))
            {
                return T(
                    LocalizationKeys.FinancialAnalysis.EmployeeBalance,
                    language);
            }

            return T(
                LocalizationKeys.FinancialAnalysis.CustomerBalance,
                language);
        }

        if(section.Equals(
            "sales",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.NetSales,
                language);
        }

        if(section.Equals(
            "expenses",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.TotalExpenses,
                language);
        }

        if(section.Equals(
            "purchases",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.TotalPurchases,
                language);
        }

        if(section.Equals(
            "debt",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.TotalDebt,
                language);
        }

        if(section.Equals(
            "cash",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.TotalCash,
                language);
        }

        if(section.Equals(
            "suppliers",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.SuppliersTotal,
                language);
        }

        if(section.Equals(
            "capital",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.CapitalValue,
                language);
        }

        if(section.Equals(
            "transfers",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.TotalTransfer,
                language);
        }

        if(section.Equals(
            "settlements",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.Balance,
                language);
        }

        if(section.Equals(
            "profit",
            StringComparison.OrdinalIgnoreCase))
        {
            return T(
                LocalizationKeys.FinancialAnalysis.ResultValue,
                language);
        }

        return T(
            LocalizationKeys.FinancialAnalysis.Value,
            language);
    }
    private static string SectionTitle(
        string section,
        string language)
    {
        if(SectionKeys.TryGetValue(section,out var key))
        {
            return T(key,language);
        }

        return section;
    }

    private static string T(
        string key,
        string language)
    {
        return FinancialAnalysisLocalization.T(
            key,
            language);
    }

    private static string SafeSheetName(string value)
    {
        foreach(var c in new[]{':','\\','/','?','*','[',']'})
        {
            value = value.Replace(c,'-');
        }

        return value.Length > 31
            ? value[..31]
            : value;
    }

    private static DateTime BaghdadNow()
    {
        try
        {
            var zone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    "Asia/Baghdad");

            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                zone);
        }
        catch
        {
            try
            {
                var zone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "Arabic Standard Time");

                return TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    zone);
            }
            catch
            {
                return DateTime.UtcNow.AddHours(3);
            }
        }
    }
}
