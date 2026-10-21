using ClosedXML.Excel;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Common.Localization;
using InventorySystem.Application.Common.Time;
using InventorySystem.Application.Features.FinancialAnalysis.Interfaces;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Reports.FinancialAnalysis;

public sealed class FinancialPeriodExportService
    : IFinancialPeriodExportService
{
    private readonly IApplicationDbContext _context;

    private sealed class FinancialBranchInfo
    {
        public int Id { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string BranchNameArabic { get; set; } = string.Empty;
        public string? BranchNameEnglish { get; set; }
    }

    private sealed class BranchAnalysis
    {
        public int BranchId { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public decimal TransferMale { get; set; }
        public decimal TransferFemale { get; set; }
        public decimal TransferTotal => TransferMale + TransferFemale;
        public decimal SettlementDebit { get; set; }
        public decimal SettlementCredit { get; set; }
        public decimal SettlementBalance { get; set; }
        public int CompletedFiles { get; set; }
        public int MissingFiles => Math.Max(0,4 - CompletedFiles);
        public decimal CompletionPercentage =>
            Math.Round(CompletedFiles * 100m / 4m,2);
        public bool IsComplete => CompletedFiles == 4;
    }

    public FinancialPeriodExportService(
        IApplicationDbContext context)
    {
        _context = context;
    }

    private static string T(
        string key,
        string language) =>
        LocalizationService.Get(
            key,
            language);

    private static bool IsArabic(
        string language) =>
        language.Equals(
            "ar",
            StringComparison.OrdinalIgnoreCase);

    public async Task<byte[]> ExportExcelAsync(
        int financialPeriodId,
        string language,
        string mode,
        int? branchId,
        CancellationToken cancellationToken)
    {
        language =
            language.Equals(
                "en",
                StringComparison.OrdinalIgnoreCase)
                ? "en"
                : "ar";

        mode =
            string.IsNullOrWhiteSpace(mode)
                ? "all"
                : mode.Trim()
                    .ToLowerInvariant();

        if (mode != "all" &&
            mode != "branch")
        {
            throw new InvalidOperationException(
                LocalizationKeys
                    .FinancialExport
                    .InvalidMode);
        }

        if (mode == "branch" &&
            (!branchId.HasValue ||
             branchId.Value <= 0))
        {
            throw new InvalidOperationException(
                LocalizationKeys
                    .FinancialExport
                    .BranchRequired);
        }

        var period =
            await _context
                .FinancialPeriods
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id ==
                            financialPeriodId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (period is null)
        {
            throw new KeyNotFoundException(
                LocalizationKeys
                    .FinancialExport
                    .PeriodNotFound);
        }

        var branches =
            await _context
                .Branches
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
                        new FinancialBranchInfo
                        {
                            Id = x.Id,
                            BranchCode =
                                x.BranchCode,
                            BranchNameArabic =
                                x.BranchNameArabic,
                            BranchNameEnglish =
                                x.BranchNameEnglish
                        })
                .ToListAsync(
                    cancellationToken);

        FinancialBranchInfo? selectedBranch =
            null;

        if (mode == "branch")
        {
            selectedBranch =
                branches
                    .FirstOrDefault(
                        x =>
                            x.Id ==
                                branchId!.Value);

            if (selectedBranch is null)
            {
                throw new KeyNotFoundException(
                    LocalizationKeys
                        .FinancialExport
                        .BranchNotFound);
            }
        }

        var uploads =
            await _context
                .FinancialUploads
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialPeriodId ==
                            financialPeriodId &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid)
                .Include(x => x.Branch)
                .ToListAsync(
                    cancellationToken);

        var uploadIds =
            uploads
                .Select(x => x.Id)
                .ToList();

        var accountRows =
            await _context
                .FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId))
                .OrderBy(
                    x =>
                        x.FinancialUploadId)
                .ThenBy(
                    x =>
                        x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var transferRecords =
            await _context
                .TransferRecords
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId))
                .Include(x => x.Branch)
                .OrderBy(
                    x =>
                        x.Branch != null
                            ? x.Branch.BranchCode
                            : string.Empty)
                .ThenBy(x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var settlementRows =
            await _context
                .SettlementRows
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId))
                .OrderBy(
                    x =>
                        x.FinancialUploadId)
                .ThenBy(
                    x =>
                        x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var settlementSummaries =
            await _context
                .SettlementSummaries
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId))
                .ToListAsync(
                    cancellationToken);

        var analysis =
            BuildBranchAnalysis(
                branches,
                uploads,
                transferRecords,
                settlementSummaries,
                language);

        using var workbook =
            new XLWorkbook();

        if (mode == "all")
        {
            BuildAllReport(
                workbook,
                period,
                branches,
                uploads,
                accountRows,
                transferRecords,
                settlementRows,
                settlementSummaries,
                analysis,
                language);
        }
        else
        {
            BuildBranchReport(
                workbook,
                period,
                selectedBranch!,
                uploads,
                accountRows,
                transferRecords,
                settlementRows,
                settlementSummaries,
                analysis,
                language);
        }

        using var stream =
            new MemoryStream();

        workbook.SaveAs(stream);

        return stream.ToArray();
    }

    private static void BuildAllReport(
        XLWorkbook workbook,
        FinancialPeriod period,
        List<FinancialBranchInfo> branches,
        List<FinancialUpload> uploads,
        List<FinancialAccountRow> accountRows,
        List<TransferRecord> transfers,
        List<SettlementRow> settlementRows,
        List<SettlementSummary> settlementSummaries,
        List<BranchAnalysis> analysis,
        string language)
    {
        BuildAllSummary(
            workbook,
            period,
            branches,
            uploads,
            transfers,
            settlementSummaries,
            analysis,
            language);

        BuildBranchComparison(
            workbook,
            analysis,
            language);

        BuildAccountSheet(
            workbook,
            T(
                LocalizationKeys.FinancialExport.IncomeStatements,
                language),
            T(
                LocalizationKeys.FinancialExport.IncomeStatements,
                language),
            FinancialFileType.IncomeStatement,
            null,
            uploads,
            accountRows,
            language);

        BuildAccountSheet(
            workbook,
            T(
                LocalizationKeys.FinancialExport.FinancialPositions,
                language),
            T(
                LocalizationKeys.FinancialExport.FinancialPositions,
                language),
            FinancialFileType.FinancialPosition,
            null,
            uploads,
            accountRows,
            language);

        BuildTransferSheet(
            workbook,
            FinancialGender.Male,
            null,
            transfers,
            language);

        BuildTransferSheet(
            workbook,
            FinancialGender.Female,
            null,
            transfers,
            language);

        BuildSettlementSheet(
            workbook,
            FinancialGender.Male,
            null,
            uploads,
            settlementRows,
            settlementSummaries,
            language);

        BuildSettlementSheet(
            workbook,
            FinancialGender.Female,
            null,
            uploads,
            settlementRows,
            settlementSummaries,
            language);

        BuildBranchStatus(
            workbook,
            branches,
            uploads,
            language);

        BuildMissingFiles(
            workbook,
            branches,
            uploads,
            language);
    }

    private static void BuildBranchReport(
        XLWorkbook workbook,
        FinancialPeriod period,
        FinancialBranchInfo branch,
        List<FinancialUpload> uploads,
        List<FinancialAccountRow> accountRows,
        List<TransferRecord> transfers,
        List<SettlementRow> settlementRows,
        List<SettlementSummary> settlementSummaries,
        List<BranchAnalysis> analysis,
        string language)
    {
        var branchAnalysis =
            analysis.First(
                x =>
                    x.BranchId ==
                        branch.Id);

        BuildSingleBranchSummary(
            workbook,
            period,
            branch,
            branchAnalysis,
            transfers,
            language);

        BuildAccountSheet(
            workbook,
            T(
                LocalizationKeys.FinancialExport.IncomeStatement,
                language),
            T(
                LocalizationKeys.FinancialExport.IncomeStatement,
                language),
            FinancialFileType.IncomeStatement,
            branch.Id,
            uploads,
            accountRows,
            language);

        BuildAccountSheet(
            workbook,
            T(
                LocalizationKeys.FinancialExport.FinancialPosition,
                language),
            T(
                LocalizationKeys.FinancialExport.FinancialPosition,
                language),
            FinancialFileType.FinancialPosition,
            branch.Id,
            uploads,
            accountRows,
            language);

        BuildTransferSheet(
            workbook,
            FinancialGender.Male,
            branch.Id,
            transfers,
            language);

        BuildTransferSheet(
            workbook,
            FinancialGender.Female,
            branch.Id,
            transfers,
            language);

        BuildSettlementSheet(
            workbook,
            FinancialGender.Male,
            branch.Id,
            uploads,
            settlementRows,
            settlementSummaries,
            language);

        BuildSettlementSheet(
            workbook,
            FinancialGender.Female,
            branch.Id,
            uploads,
            settlementRows,
            settlementSummaries,
            language);
    }

    private static List<BranchAnalysis> BuildBranchAnalysis(
        List<FinancialBranchInfo> branches,
        List<FinancialUpload> uploads,
        List<TransferRecord> transfers,
        List<SettlementSummary> summaries,
        string language)
    {
        var result =
            new List<BranchAnalysis>();

        foreach (var branch in branches)
        {
            var branchUploads =
                uploads
                    .Where(
                        x =>
                            x.BranchId ==
                                branch.Id)
                    .ToList();

            var income =
                branchUploads.Any(
                    x =>
                        x.FileType ==
                            FinancialFileType.IncomeStatement);

            var position =
                branchUploads.Any(
                    x =>
                        x.FileType ==
                            FinancialFileType.FinancialPosition);

            var maleSettlement =
                branchUploads
                    .FirstOrDefault(
                        x =>
                            x.FileType ==
                                FinancialFileType.Settlement &&
                            x.Gender ==
                                FinancialGender.Male);

            var femaleSettlement =
                branchUploads
                    .FirstOrDefault(
                        x =>
                            x.FileType ==
                                FinancialFileType.Settlement &&
                            x.Gender ==
                                FinancialGender.Female);

            var completed =
                (income ? 1 : 0) +
                (position ? 1 : 0) +
                (maleSettlement != null ? 1 : 0) +
                (femaleSettlement != null ? 1 : 0);

            var settlementIds =
                new[]
                {
                    maleSettlement?.Id,
                    femaleSettlement?.Id
                }
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();

            var branchSummaries =
                summaries
                    .Where(
                        x =>
                            settlementIds.Contains(
                                x.FinancialUploadId))
                    .ToList();

            result.Add(
                new BranchAnalysis
                {
                    BranchId = branch.Id,
                    BranchCode = branch.BranchCode,
                    BranchName =
                        GetBranchName(
                            branch,
                            language),

                    TransferMale =
                        transfers
                            .Where(
                                x =>
                                    x.BranchId ==
                                        branch.Id &&
                                    x.Gender ==
                                        FinancialGender.Male)
                            .Sum(x => x.Value),

                    TransferFemale =
                        transfers
                            .Where(
                                x =>
                                    x.BranchId ==
                                        branch.Id &&
                                    x.Gender ==
                                        FinancialGender.Female)
                            .Sum(x => x.Value),

                    SettlementDebit =
                        branchSummaries.Sum(
                            x => x.PeriodDebit),

                    SettlementCredit =
                        branchSummaries.Sum(
                            x => x.PeriodCredit),

                    SettlementBalance =
                        branchSummaries.Sum(
                            x => x.PeriodBalance),

                    CompletedFiles =
                        completed
                });
        }

        return result
            .OrderBy(x => x.BranchCode)
            .ToList();
    }

    private static void BuildAllSummary(
        XLWorkbook workbook,
        FinancialPeriod period,
        List<FinancialBranchInfo> branches,
        List<FinancialUpload> uploads,
        List<TransferRecord> transfers,
        List<SettlementSummary> settlements,
        List<BranchAnalysis> analysis,
        string language)
    {
        var ws =
            workbook.Worksheets.Add(
                T(
                    LocalizationKeys.FinancialExport.Summary,
                    language));

        ws.RightToLeft =
            IsArabic(language);

        WriteTitle(
            ws,
            T(
                LocalizationKeys.FinancialExport.AllBranchesReport,
                language),
            4);

        var expectedFiles =
            2 + branches.Count * 4;

        var validFiles =
            uploads.Count;

        var missingFiles =
            Math.Max(
                0,
                expectedFiles - validFiles);

        var completion =
            expectedFiles == 0
                ? 0
                : Math.Round(
                    validFiles * 100m /
                    expectedFiles,
                    2);

        var completeBranches =
            analysis.Count(x => x.IsComplete);

        var maleTransfer =
            transfers
                .Where(
                    x =>
                        x.Gender ==
                            FinancialGender.Male)
                .Sum(x => x.Value);

        var femaleTransfer =
            transfers
                .Where(
                    x =>
                        x.Gender ==
                            FinancialGender.Female)
                .Sum(x => x.Value);

        var highestTransfer =
            analysis
                .OrderByDescending(
                    x => x.TransferTotal)
                .FirstOrDefault();

        var lowestTransfer =
            analysis
                .OrderBy(
                    x => x.TransferTotal)
                .FirstOrDefault();

        var highestBalance =
            analysis
                .OrderByDescending(
                    x => x.SettlementBalance)
                .FirstOrDefault();

        var lowestBalance =
            analysis
                .OrderBy(
                    x => x.SettlementBalance)
                .FirstOrDefault();

        var row = 3;

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.PeriodId,language),
            period.Id.ToString());

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.StartDate,language),
            period.StartDate.ToString("yyyy-MM-dd"));

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.EndDate,language),
            period.EndDate.ToString("yyyy-MM-dd"));

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.Status,language),
            StatusName(period.Status,language));

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.GeneratedAtBaghdad,language),
            BaghdadTime.Now.ToString("yyyy-MM-dd hh:mm tt"));

        row++;

        WriteSection(
            ws,row++,
            T(LocalizationKeys.FinancialExport.FilesAndBranchesStatus,language),
            4);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.ActiveBranches,language),
            branches.Count);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.CompleteBranches,language),
            completeBranches);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.IncompleteBranches,language),
            branches.Count - completeBranches);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.ExpectedFiles,language),
            expectedFiles);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.ValidFiles,language),
            validFiles);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.MissingFiles,language),
            missingFiles);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.Completion,language),
            completion,
            true);

        row++;

        WriteSection(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TransferAnalysis,language),
            4);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TransferMale,language),
            maleTransfer);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TransferFemale,language),
            femaleTransfer);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TotalTransfers,language),
            maleTransfer + femaleTransfer);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.AverageTransfersPerBranch,language),
            SafeAverage(
                analysis.Select(x => x.TransferTotal)));

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.HighestTransferBranch,language),
            highestTransfer == null
                ? "-"
                : $"{highestTransfer.BranchCode} - {highestTransfer.BranchName} ({highestTransfer.TransferTotal:N2})");

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.LowestTransferBranch,language),
            lowestTransfer == null
                ? "-"
                : $"{lowestTransfer.BranchCode} - {lowestTransfer.BranchName} ({lowestTransfer.TransferTotal:N2})");

        row++;

        WriteSection(
            ws,row++,
            T(LocalizationKeys.FinancialExport.SettlementAnalysis,language),
            4);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TotalDebit,language),
            settlements.Sum(x => x.PeriodDebit));

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TotalCredit,language),
            settlements.Sum(x => x.PeriodCredit));

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TotalBalance,language),
            settlements.Sum(x => x.PeriodBalance));

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.AverageBalancePerBranch,language),
            SafeAverage(
                analysis.Select(
                    x => x.SettlementBalance)));

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.HighestBranchBalance,language),
            highestBalance == null
                ? "-"
                : $"{highestBalance.BranchCode} - {highestBalance.BranchName} ({highestBalance.SettlementBalance:N2})");

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.LowestBranchBalance,language),
            lowestBalance == null
                ? "-"
                : $"{lowestBalance.BranchCode} - {lowestBalance.BranchName} ({lowestBalance.SettlementBalance:N2})");

        ws.Column(1).Width = 36;
        ws.Column(2).Width = 45;
    }

    private static void BuildSingleBranchSummary(
        XLWorkbook workbook,
        FinancialPeriod period,
        FinancialBranchInfo branch,
        BranchAnalysis analysis,
        List<TransferRecord> transfers,
        string language)
    {
        var ws =
            workbook.Worksheets.Add(
                T(
                    LocalizationKeys.FinancialExport.Summary,
                    language));

        ws.RightToLeft =
            IsArabic(language);

        WriteTitle(
            ws,
            $"{T(LocalizationKeys.FinancialExport.BranchReport,language)} - {GetBranchName(branch,language)}",
            4);

        var row = 3;

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.BranchCode,language),
            branch.BranchCode);

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.Branch,language),
            GetBranchName(branch,language));

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.StartDate,language),
            period.StartDate.ToString("yyyy-MM-dd"));

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.EndDate,language),
            period.EndDate.ToString("yyyy-MM-dd"));

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.Status,language),
            StatusName(period.Status,language));

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.GeneratedAtBaghdad,language),
            BaghdadTime.Now.ToString("yyyy-MM-dd hh:mm tt"));

        row++;

        WriteSection(
            ws,row++,
            T(LocalizationKeys.FinancialExport.FilesStatus,language),
            4);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.ExpectedFiles,language),
            4);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.CompletedFiles,language),
            analysis.CompletedFiles);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.MissingFiles,language),
            analysis.MissingFiles);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.Completion,language),
            analysis.CompletionPercentage,
            true);

        row++;

        WriteSection(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TransferAnalysis,language),
            4);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TransferMale,language),
            analysis.TransferMale);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TransferFemale,language),
            analysis.TransferFemale);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TotalTransfers,language),
            analysis.TransferTotal);

        var branchTransfers =
            transfers
                .Where(
                    x =>
                        x.BranchId ==
                            branch.Id)
                .ToList();

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.AverageTransferRecord,language),
            branchTransfers.Count == 0
                ? 0
                : branchTransfers.Average(x => x.Value));

        var high =
            branchTransfers
                .OrderByDescending(x => x.Value)
                .FirstOrDefault();

        var low =
            branchTransfers
                .OrderBy(x => x.Value)
                .FirstOrDefault();

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.HighestTransferRecord,language),
            high == null
                ? "-"
                : $"{high.AccountCode} - {high.AccountName} ({high.Value:N2})");

        WriteTextMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.LowestTransferRecord,language),
            low == null
                ? "-"
                : $"{low.AccountCode} - {low.AccountName} ({low.Value:N2})");

        row++;

        WriteSection(
            ws,row++,
            T(LocalizationKeys.FinancialExport.SettlementAnalysis,language),
            4);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TotalDebit,language),
            analysis.SettlementDebit);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.TotalCredit,language),
            analysis.SettlementCredit);

        WriteMetric(
            ws,row++,
            T(LocalizationKeys.FinancialExport.Balance,language),
            analysis.SettlementBalance);

        ws.Column(1).Width = 36;
        ws.Column(2).Width = 45;
    }

    private static void BuildBranchComparison(
        XLWorkbook workbook,
        List<BranchAnalysis> analysis,
        string language)
    {
        var ws =
            workbook.Worksheets.Add(
                T(
                    LocalizationKeys.FinancialExport.BranchComparison,
                    language));

        ws.RightToLeft =
            IsArabic(language);

        WriteTitle(
            ws,
            T(
                LocalizationKeys.FinancialExport.BranchAnalyticalComparison,
                language),
            12);

        var headers =
            new[]
            {
                T(LocalizationKeys.FinancialExport.BranchCode,language),
                T(LocalizationKeys.FinancialExport.Branch,language),
                T(LocalizationKeys.FinancialExport.TransferMale,language),
                T(LocalizationKeys.FinancialExport.TransferFemale,language),
                T(LocalizationKeys.FinancialExport.TotalTransfers,language),
                T(LocalizationKeys.FinancialExport.TotalDebit,language),
                T(LocalizationKeys.FinancialExport.TotalCredit,language),
                T(LocalizationKeys.FinancialExport.Balance,language),
                T(LocalizationKeys.FinancialExport.CompletedFiles,language),
                T(LocalizationKeys.FinancialExport.MissingFiles,language),
                T(LocalizationKeys.FinancialExport.Completion,language),
                T(LocalizationKeys.FinancialExport.Status,language)
            };

        const int headerRow = 3;

        WriteHeaders(ws,headerRow,headers);

        var row =
            headerRow + 1;

        foreach (var item in analysis)
        {
            ws.Cell(row,1).Value =
                item.BranchCode;

            ws.Cell(row,2).Value =
                item.BranchName;

            SetDecimal(ws.Cell(row,3),item.TransferMale);
            SetDecimal(ws.Cell(row,4),item.TransferFemale);
            SetDecimal(ws.Cell(row,5),item.TransferTotal);
            SetDecimal(ws.Cell(row,6),item.SettlementDebit);
            SetDecimal(ws.Cell(row,7),item.SettlementCredit);
            SetDecimal(ws.Cell(row,8),item.SettlementBalance);

            ws.Cell(row,9).Value =
                item.CompletedFiles;

            ws.Cell(row,10).Value =
                item.MissingFiles;

            ws.Cell(row,11).Value =
                item.CompletionPercentage;

            ws.Cell(row,11)
                .Style.NumberFormat.Format =
                "0.00\"%\"";

            ws.Cell(row,12).Value =
                item.IsComplete
                    ? T(LocalizationKeys.FinancialExport.Complete,language)
                    : T(LocalizationKeys.FinancialExport.Incomplete,language);

            row++;
        }

        var totalRow = row + 1;

        WriteStats(
            ws,totalRow,
            T(LocalizationKeys.FinancialExport.Total,language),
            analysis.Sum(x => x.TransferMale),
            analysis.Sum(x => x.TransferFemale),
            analysis.Sum(x => x.TransferTotal),
            analysis.Sum(x => x.SettlementDebit),
            analysis.Sum(x => x.SettlementCredit),
            analysis.Sum(x => x.SettlementBalance),
            null);

        WriteStats(
            ws,totalRow + 1,
            T(LocalizationKeys.FinancialExport.Highest,language),
            SafeMax(analysis.Select(x => x.TransferMale)),
            SafeMax(analysis.Select(x => x.TransferFemale)),
            SafeMax(analysis.Select(x => x.TransferTotal)),
            SafeMax(analysis.Select(x => x.SettlementDebit)),
            SafeMax(analysis.Select(x => x.SettlementCredit)),
            SafeMax(analysis.Select(x => x.SettlementBalance)),
            SafeMax(analysis.Select(x => x.CompletionPercentage)));

        WriteStats(
            ws,totalRow + 2,
            T(LocalizationKeys.FinancialExport.Lowest,language),
            SafeMin(analysis.Select(x => x.TransferMale)),
            SafeMin(analysis.Select(x => x.TransferFemale)),
            SafeMin(analysis.Select(x => x.TransferTotal)),
            SafeMin(analysis.Select(x => x.SettlementDebit)),
            SafeMin(analysis.Select(x => x.SettlementCredit)),
            SafeMin(analysis.Select(x => x.SettlementBalance)),
            SafeMin(analysis.Select(x => x.CompletionPercentage)));

        WriteStats(
            ws,totalRow + 3,
            T(LocalizationKeys.FinancialExport.Average,language),
            SafeAverage(analysis.Select(x => x.TransferMale)),
            SafeAverage(analysis.Select(x => x.TransferFemale)),
            SafeAverage(analysis.Select(x => x.TransferTotal)),
            SafeAverage(analysis.Select(x => x.SettlementDebit)),
            SafeAverage(analysis.Select(x => x.SettlementCredit)),
            SafeAverage(analysis.Select(x => x.SettlementBalance)),
            SafeAverage(analysis.Select(x => x.CompletionPercentage)));

        ApplyLayout(
            ws,
            headerRow,
            headers.Length,
            row - 1);

        ws.Column(1).Width = 14;
        ws.Column(2).Width = 30;
        ws.Columns(3,8).Width = 18;
        ws.Columns(9,12).Width = 16;
    }

    private static void BuildAccountSheet(
        XLWorkbook workbook,
        string sheetName,
        string title,
        FinancialFileType type,
        int? branchId,
        List<FinancialUpload> uploads,
        List<FinancialAccountRow> rows,
        string language)
    {
        var ws =
            workbook.Worksheets.Add(sheetName);

        ws.RightToLeft =
            IsArabic(language);

        WriteTitle(ws,title,10);

        ws.Cell(2,1).Value =
            T(
                LocalizationKeys
                    .FinancialExport
                    .NoteNoAccountGrandTotal,
                language);

        ws.Range(2,1,2,10).Merge();

        ws.Cell(2,1)
            .Style.Font.Italic =
            true;

        var headers =
            new[]
            {
                T(LocalizationKeys.FinancialExport.BranchCode,language),
                T(LocalizationKeys.FinancialExport.Branch,language),
                T(LocalizationKeys.FinancialExport.AccountCode,language),
                T(LocalizationKeys.FinancialExport.AccountName,language),
                T(LocalizationKeys.FinancialExport.Debit,language),
                T(LocalizationKeys.FinancialExport.Credit,language),
                T(LocalizationKeys.FinancialExport.Balance,language),
                T(LocalizationKeys.FinancialExport.Assets,language),
                T(LocalizationKeys.FinancialExport.Liabilities,language),
                T(LocalizationKeys.FinancialExport.RowOrder,language)
            };

        const int headerRow = 4;

        WriteHeaders(ws,headerRow,headers);

        var selectedUploads =
            uploads
                .Where(
                    x =>
                        x.FileType == type &&
                        (!branchId.HasValue ||
                         x.BranchId == branchId.Value))
                .OrderBy(
                    x =>
                        x.Branch != null
                            ? x.Branch.BranchCode
                            : string.Empty)
                .ToList();

        var row =
            headerRow + 1;

        foreach (var upload in selectedUploads)
        {
            foreach (
                var item in rows
                    .Where(
                        x =>
                            x.FinancialUploadId ==
                                upload.Id)
                    .OrderBy(x => x.RowOrder))
            {
                ws.Cell(row,1).Value =
                    upload.Branch?.BranchCode
                    ?? string.Empty;

                ws.Cell(row,2).Value =
                    GetBranchName(
                        upload.Branch,
                        language);

                ws.Cell(row,3).Value =
                    item.AccountCode;

                ws.Cell(row,4).Value =
                    item.AccountName;

                SetNullableDecimal(ws.Cell(row,5),item.Debit);
                SetNullableDecimal(ws.Cell(row,6),item.Credit);
                SetNullableDecimal(ws.Cell(row,7),item.Balance);
                SetNullableDecimal(ws.Cell(row,8),item.Assets);
                SetNullableDecimal(ws.Cell(row,9),item.Liabilities);

                ws.Cell(row,10).Value =
                    item.RowOrder;

                row++;
            }
        }

        ApplyLayout(
            ws,
            headerRow,
            headers.Length,
            row - 1);

        ws.Column(1).Width = 14;
        ws.Column(2).Width = 28;
        ws.Column(3).Width = 16;
        ws.Column(4).Width = 34;
        ws.Columns(5,9).Width = 16;
        ws.Column(10).Width = 12;
    }

    private static void BuildTransferSheet(
        XLWorkbook workbook,
        FinancialGender gender,
        int? branchId,
        List<TransferRecord> rows,
        string language)
    {
        var sheetName =
            gender == FinancialGender.Male
                ? T(LocalizationKeys.FinancialExport.TransfersMale,language)
                : T(LocalizationKeys.FinancialExport.TransfersFemale,language);

        var ws =
            workbook.Worksheets.Add(sheetName);

        ws.RightToLeft =
            IsArabic(language);

        WriteTitle(ws,sheetName,7);

        var selected =
            rows
                .Where(
                    x =>
                        x.Gender == gender &&
                        (!branchId.HasValue ||
                         x.BranchId == branchId.Value))
                .OrderBy(
                    x =>
                        x.Branch != null
                            ? x.Branch.BranchCode
                            : string.Empty)
                .ThenBy(x => x.RowOrder)
                .ToList();

        var total =
            selected.Sum(x => x.Value);

        var highest =
            selected
                .OrderByDescending(x => x.Value)
                .FirstOrDefault();

        var lowest =
            selected
                .OrderBy(x => x.Value)
                .FirstOrDefault();

        var average =
            selected.Count == 0
                ? 0
                : selected.Average(x => x.Value);

        ws.Cell(2,1).Value =
            $"{T(LocalizationKeys.FinancialExport.Total,language)}: {total:N2} | " +
            $"{T(LocalizationKeys.FinancialExport.Highest,language)}: {(highest?.Value ?? 0):N2} | " +
            $"{T(LocalizationKeys.FinancialExport.Lowest,language)}: {(lowest?.Value ?? 0):N2} | " +
            $"{T(LocalizationKeys.FinancialExport.Average,language)}: {average:N2} | " +
            $"{T(LocalizationKeys.FinancialExport.Records,language)}: {selected.Count}";

        ws.Range(2,1,2,7).Merge();
        ws.Cell(2,1).Style.Font.Bold = true;

        var headers =
            new[]
            {
                T(LocalizationKeys.FinancialExport.BranchCode,language),
                T(LocalizationKeys.FinancialExport.Branch,language),
                T(LocalizationKeys.FinancialExport.AccountCode,language),
                T(LocalizationKeys.FinancialExport.AccountName,language),
                T(LocalizationKeys.FinancialExport.Gender,language),
                T(LocalizationKeys.FinancialExport.Value,language),
                T(LocalizationKeys.FinancialExport.RowOrder,language)
            };

        const int headerRow = 4;

        WriteHeaders(ws,headerRow,headers);

        var row =
            headerRow + 1;

        foreach (var item in selected)
        {
            ws.Cell(row,1).Value =
                item.Branch?.BranchCode
                ?? string.Empty;

            ws.Cell(row,2).Value =
                GetBranchName(
                    item.Branch,
                    language);

            ws.Cell(row,3).Value =
                item.AccountCode;

            ws.Cell(row,4).Value =
                item.AccountName;

            ws.Cell(row,5).Value =
                GenderName(
                    item.Gender,
                    language);

            SetDecimal(
                ws.Cell(row,6),
                item.Value);

            ws.Cell(row,7).Value =
                item.RowOrder;

            row++;
        }

        var totalRow =
            row + 1;

        ws.Cell(totalRow,5).Value =
            T(
                LocalizationKeys.FinancialExport.Total,
                language);

        SetDecimal(
            ws.Cell(totalRow,6),
            total);

        ws.Range(totalRow,5,totalRow,6)
            .Style.Font.Bold =
            true;

        ApplyLayout(
            ws,
            headerRow,
            headers.Length,
            row - 1);

        ws.Column(1).Width = 14;
        ws.Column(2).Width = 28;
        ws.Column(3).Width = 16;
        ws.Column(4).Width = 34;
        ws.Column(5).Width = 14;
        ws.Column(6).Width = 18;
        ws.Column(7).Width = 12;
    }

    private static void BuildSettlementSheet(
        XLWorkbook workbook,
        FinancialGender gender,
        int? branchId,
        List<FinancialUpload> uploads,
        List<SettlementRow> rows,
        List<SettlementSummary> summaries,
        string language)
    {
        var sheetName =
            gender == FinancialGender.Male
                ? T(LocalizationKeys.FinancialExport.SettlementsMale,language)
                : T(LocalizationKeys.FinancialExport.SettlementsFemale,language);

        var ws =
            workbook.Worksheets.Add(sheetName);

        ws.RightToLeft =
            IsArabic(language);

        WriteTitle(ws,sheetName,12);

        var selectedUploads =
            uploads
                .Where(
                    x =>
                        x.FileType ==
                            FinancialFileType.Settlement &&
                        x.Gender == gender &&
                        (!branchId.HasValue ||
                         x.BranchId == branchId.Value))
                .OrderBy(
                    x =>
                        x.Branch != null
                            ? x.Branch.BranchCode
                            : string.Empty)
                .ToList();

        var ids =
            selectedUploads
                .Select(x => x.Id)
                .ToList();

        var selectedSummaries =
            summaries
                .Where(
                    x =>
                        ids.Contains(
                            x.FinancialUploadId))
                .ToList();

        var debit =
            selectedSummaries.Sum(x => x.PeriodDebit);

        var credit =
            selectedSummaries.Sum(x => x.PeriodCredit);

        var balance =
            selectedSummaries.Sum(x => x.PeriodBalance);

        ws.Cell(2,1).Value =
            $"{T(LocalizationKeys.FinancialExport.TotalDebit,language)}: {debit:N2} | " +
            $"{T(LocalizationKeys.FinancialExport.TotalCredit,language)}: {credit:N2} | " +
            $"{T(LocalizationKeys.FinancialExport.Balance,language)}: {balance:N2}";

        ws.Range(2,1,2,12).Merge();
        ws.Cell(2,1).Style.Font.Bold = true;

        var headers =
            new[]
            {
                T(LocalizationKeys.FinancialExport.BranchCode,language),
                T(LocalizationKeys.FinancialExport.Branch,language),
                T(LocalizationKeys.FinancialExport.Gender,language),
                T(LocalizationKeys.FinancialExport.TransactionDate,language),
                T(LocalizationKeys.FinancialExport.Description,language),
                T(LocalizationKeys.FinancialExport.EntryNumber,language),
                T(LocalizationKeys.FinancialExport.Debit,language),
                T(LocalizationKeys.FinancialExport.Credit,language),
                T(LocalizationKeys.FinancialExport.Balance,language),
                T(LocalizationKeys.FinancialExport.Marked,language),
                T(LocalizationKeys.FinancialExport.Matched,language),
                T(LocalizationKeys.FinancialExport.RowOrder,language)
            };

        const int headerRow = 4;

        WriteHeaders(ws,headerRow,headers);

        var row =
            headerRow + 1;

        foreach (var upload in selectedUploads)
        {
            foreach (
                var item in rows
                    .Where(
                        x =>
                            x.FinancialUploadId ==
                                upload.Id)
                    .OrderBy(x => x.RowOrder))
            {
                ws.Cell(row,1).Value =
                    upload.Branch?.BranchCode
                    ?? string.Empty;

                ws.Cell(row,2).Value =
                    GetBranchName(
                        upload.Branch,
                        language);

                ws.Cell(row,3).Value =
                    GenderName(
                        gender,
                        language);

                if (item.TransactionDate.HasValue)
                {
                    ws.Cell(row,4).Value =
                        item.TransactionDate.Value;

                    ws.Cell(row,4)
                        .Style.NumberFormat.Format =
                        "yyyy-MM-dd";
                }

                ws.Cell(row,5).Value =
                    item.Description;

                ws.Cell(row,6).Value =
                    item.EntryNumber
                    ?? string.Empty;

                SetNullableDecimal(
                    ws.Cell(row,7),
                    item.Debit);

                SetNullableDecimal(
                    ws.Cell(row,8),
                    item.Credit);

                SetNullableDecimal(
                    ws.Cell(row,9),
                    item.Balance);

                ws.Cell(row,10).Value =
                    YesNo(
                        item.IsMarked,
                        language);

                ws.Cell(row,11).Value =
                    YesNo(
                        item.IsMatched,
                        language);

                ws.Cell(row,12).Value =
                    item.RowOrder;

                row++;
            }
        }

        ApplyLayout(
            ws,
            headerRow,
            headers.Length,
            row - 1);

        ws.Column(1).Width = 14;
        ws.Column(2).Width = 28;
        ws.Column(3).Width = 14;
        ws.Column(4).Width = 14;
        ws.Column(5).Width = 40;
        ws.Column(6).Width = 16;
        ws.Columns(7,9).Width = 18;
        ws.Columns(10,12).Width = 13;
    }

    private static void BuildBranchStatus(
        XLWorkbook workbook,
        List<FinancialBranchInfo> branches,
        List<FinancialUpload> uploads,
        string language)
    {
        var ws =
            workbook.Worksheets.Add(
                T(
                    LocalizationKeys.FinancialExport.BranchStatus,
                    language));

        ws.RightToLeft =
            IsArabic(language);

        WriteTitle(
            ws,
            T(
                LocalizationKeys.FinancialExport.AllBranchFilesStatus,
                language),
            9);

        var headers =
            new[]
            {
                T(LocalizationKeys.FinancialExport.BranchCode,language),
                T(LocalizationKeys.FinancialExport.Branch,language),
                T(LocalizationKeys.FinancialExport.IncomeStatementFile,language),
                T(LocalizationKeys.FinancialExport.FinancialPositionFile,language),
                T(LocalizationKeys.FinancialExport.SettlementsMale,language),
                T(LocalizationKeys.FinancialExport.SettlementsFemale,language),
                T(LocalizationKeys.FinancialExport.CompletedFiles,language),
                T(LocalizationKeys.FinancialExport.MissingFiles,language),
                T(LocalizationKeys.FinancialExport.Completion,language)
            };

        const int headerRow = 3;

        WriteHeaders(ws,headerRow,headers);

        var row =
            headerRow + 1;

        foreach (var branch in branches)
        {
            var branchUploads =
                uploads
                    .Where(
                        x =>
                            x.BranchId ==
                                branch.Id)
                    .ToList();

            var income =
                branchUploads.Any(
                    x =>
                        x.FileType ==
                            FinancialFileType.IncomeStatement);

            var position =
                branchUploads.Any(
                    x =>
                        x.FileType ==
                            FinancialFileType.FinancialPosition);

            var male =
                branchUploads.Any(
                    x =>
                        x.FileType ==
                            FinancialFileType.Settlement &&
                        x.Gender ==
                            FinancialGender.Male);

            var female =
                branchUploads.Any(
                    x =>
                        x.FileType ==
                            FinancialFileType.Settlement &&
                        x.Gender ==
                            FinancialGender.Female);

            var completed =
                (income ? 1 : 0) +
                (position ? 1 : 0) +
                (male ? 1 : 0) +
                (female ? 1 : 0);

            ws.Cell(row,1).Value =
                branch.BranchCode;

            ws.Cell(row,2).Value =
                GetBranchName(
                    branch,
                    language);

            ws.Cell(row,3).Value =
                YesNo(income,language);

            ws.Cell(row,4).Value =
                YesNo(position,language);

            ws.Cell(row,5).Value =
                YesNo(male,language);

            ws.Cell(row,6).Value =
                YesNo(female,language);

            ws.Cell(row,7).Value =
                completed;

            ws.Cell(row,8).Value =
                4 - completed;

            ws.Cell(row,9).Value =
                Math.Round(
                    completed * 100m / 4m,
                    2);

            ws.Cell(row,9)
                .Style.NumberFormat.Format =
                "0.00\"%\"";

            row++;
        }

        ApplyLayout(
            ws,
            headerRow,
            headers.Length,
            row - 1);

        ws.Columns().AdjustToContents();
    }

    private static void BuildMissingFiles(
        XLWorkbook workbook,
        List<FinancialBranchInfo> branches,
        List<FinancialUpload> uploads,
        string language)
    {
        var ws =
            workbook.Worksheets.Add(
                T(
                    LocalizationKeys.FinancialExport.MissingFilesSheet,
                    language));

        ws.RightToLeft =
            IsArabic(language);

        WriteTitle(
            ws,
            T(
                LocalizationKeys.FinancialExport.RequiredMissingFiles,
                language),
            4);

        var headers =
            new[]
            {
                T(LocalizationKeys.FinancialExport.BranchCode,language),
                T(LocalizationKeys.FinancialExport.Branch,language),
                T(LocalizationKeys.FinancialExport.FileType,language),
                T(LocalizationKeys.FinancialExport.Gender,language)
            };

        const int headerRow = 3;

        WriteHeaders(ws,headerRow,headers);

        var row =
            headerRow + 1;

        foreach (var branch in branches)
        {
            var branchUploads =
                uploads
                    .Where(
                        x =>
                            x.BranchId ==
                                branch.Id)
                    .ToList();

            AddMissing(
                ws,
                ref row,
                branch,
                branchUploads,
                FinancialFileType.IncomeStatement,
                null,
                language);

            AddMissing(
                ws,
                ref row,
                branch,
                branchUploads,
                FinancialFileType.FinancialPosition,
                null,
                language);

            AddMissing(
                ws,
                ref row,
                branch,
                branchUploads,
                FinancialFileType.Settlement,
                FinancialGender.Male,
                language);

            AddMissing(
                ws,
                ref row,
                branch,
                branchUploads,
                FinancialFileType.Settlement,
                FinancialGender.Female,
                language);
        }

        var transfers =
            uploads
                .Where(
                    x =>
                        x.FileType ==
                            FinancialFileType.Transfer)
                .ToList();

        foreach (
            var gender in new[]
            {
                FinancialGender.Male,
                FinancialGender.Female
            })
        {
            if (transfers.Any(
                    x =>
                        x.Gender ==
                            gender))
            {
                continue;
            }

            ws.Cell(row,1).Value = "-";

            ws.Cell(row,2).Value =
                T(
                    LocalizationKeys.FinancialExport.AllBranches,
                    language);

            ws.Cell(row,3).Value =
                FileTypeName(
                    FinancialFileType.Transfer,
                    language);

            ws.Cell(row,4).Value =
                GenderName(
                    gender,
                    language);

            row++;
        }

        ApplyLayout(
            ws,
            headerRow,
            headers.Length,
            row - 1);

        ws.Columns().AdjustToContents();
    }

    private static void AddMissing(
        IXLWorksheet ws,
        ref int row,
        FinancialBranchInfo branch,
        List<FinancialUpload> uploads,
        FinancialFileType type,
        FinancialGender? gender,
        string language)
    {
        var exists =
            uploads.Any(
                x =>
                    x.FileType == type &&
                    x.Gender == gender);

        if (exists)
            return;

        ws.Cell(row,1).Value =
            branch.BranchCode;

        ws.Cell(row,2).Value =
            GetBranchName(
                branch,
                language);

        ws.Cell(row,3).Value =
            FileTypeName(
                type,
                language);

        ws.Cell(row,4).Value =
            gender.HasValue
                ? GenderName(
                    gender.Value,
                    language)
                : "-";

        row++;
    }

    private static string StatusName(
        FinancialPeriodStatus status,
        string language) =>
        status switch
        {
            FinancialPeriodStatus.Draft =>
                T(LocalizationKeys.FinancialExport.Draft,language),

            FinancialPeriodStatus.Uploading =>
                T(LocalizationKeys.FinancialExport.Uploading,language),

            FinancialPeriodStatus.Ready =>
                T(LocalizationKeys.FinancialExport.Ready,language),

            FinancialPeriodStatus.Approved =>
                T(LocalizationKeys.FinancialExport.Approved,language),

            _ => status.ToString()
        };

    private static string GenderName(
        FinancialGender gender,
        string language) =>
        gender == FinancialGender.Male
            ? T(LocalizationKeys.FinancialExport.Male,language)
            : T(LocalizationKeys.FinancialExport.Female,language);

    private static string FileTypeName(
        FinancialFileType type,
        string language) =>
        type switch
        {
            FinancialFileType.IncomeStatement =>
                T(LocalizationKeys.FinancialExport.IncomeStatementFile,language),

            FinancialFileType.FinancialPosition =>
                T(LocalizationKeys.FinancialExport.FinancialPositionFile,language),

            FinancialFileType.Transfer =>
                T(LocalizationKeys.FinancialExport.TransferFile,language),

            FinancialFileType.Settlement =>
                T(LocalizationKeys.FinancialExport.SettlementFile,language),

            _ => type.ToString()
        };

    private static string YesNo(
        bool value,
        string language) =>
        value
            ? T(LocalizationKeys.FinancialExport.Yes,language)
            : T(LocalizationKeys.FinancialExport.No,language);

    private static string GetBranchName(
        FinancialBranchInfo branch,
        string language)
    {
        if (IsArabic(language))
            return branch.BranchNameArabic;

        return string.IsNullOrWhiteSpace(
            branch.BranchNameEnglish)
                ? branch.BranchNameArabic
                : branch.BranchNameEnglish!;
    }

    private static string GetBranchName(
        dynamic? branch,
        string language)
    {
        if (branch is null)
            return string.Empty;

        if (IsArabic(language))
            return branch.BranchNameArabic
                ?? string.Empty;

        string? english =
            branch.BranchNameEnglish;

        return string.IsNullOrWhiteSpace(english)
            ? branch.BranchNameArabic
                ?? string.Empty
            : english;
    }

    private static void WriteTitle(
        IXLWorksheet ws,
        string title,
        int columns)
    {
        ws.Cell(1,1).Value = title;
        ws.Range(1,1,1,columns).Merge();

        var cell = ws.Cell(1,1);

        cell.Style.Font.Bold = true;
        cell.Style.Font.FontSize = 18;
        cell.Style.Font.FontColor = XLColor.White;
        cell.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#1F4E78");
        cell.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        ws.Row(1).Height = 28;
    }

    private static void WriteSection(
        IXLWorksheet ws,
        int row,
        string title,
        int columns)
    {
        ws.Cell(row,1).Value = title;
        ws.Range(row,1,row,columns).Merge();
        ws.Cell(row,1).Style.Font.Bold = true;
        ws.Cell(row,1).Style.Font.FontColor = XLColor.White;
        ws.Cell(row,1).Style.Fill.BackgroundColor =
            XLColor.FromHtml("#2E75B6");
    }

    private static void WriteHeaders(
        IXLWorksheet ws,
        int row,
        string[] headers)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            var cell =
                ws.Cell(row,i + 1);

            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#2E75B6");
            cell.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
        }

        ws.Row(row).Height = 30;
    }

    private static void WriteMetric(
        IXLWorksheet ws,
        int row,
        string label,
        decimal value,
        bool percentage = false)
    {
        ws.Cell(row,1).Value = label;
        StyleLabel(ws.Cell(row,1));

        ws.Cell(row,2).Value = value;

        ws.Cell(row,2)
            .Style.NumberFormat.Format =
            percentage
                ? "0.00\"%\""
                : "#,##0.00;[Red]-#,##0.00";
    }

    private static void WriteTextMetric(
        IXLWorksheet ws,
        int row,
        string label,
        string value)
    {
        ws.Cell(row,1).Value = label;
        StyleLabel(ws.Cell(row,1));
        ws.Cell(row,2).Value = value;
    }

    private static void StyleLabel(
        IXLCell cell)
    {
        cell.Style.Font.Bold = true;
        cell.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#DDEBF7");
    }

    private static void WriteStats(
        IXLWorksheet ws,
        int row,
        string label,
        decimal male,
        decimal female,
        decimal total,
        decimal debit,
        decimal credit,
        decimal balance,
        decimal? completion)
    {
        ws.Cell(row,1).Value = label;
        ws.Range(row,1,row,2).Merge();

        SetDecimal(ws.Cell(row,3),male);
        SetDecimal(ws.Cell(row,4),female);
        SetDecimal(ws.Cell(row,5),total);
        SetDecimal(ws.Cell(row,6),debit);
        SetDecimal(ws.Cell(row,7),credit);
        SetDecimal(ws.Cell(row,8),balance);

        if (completion.HasValue)
        {
            ws.Cell(row,11).Value =
                completion.Value;

            ws.Cell(row,11)
                .Style.NumberFormat.Format =
                "0.00\"%\"";
        }

        ws.Range(row,1,row,12)
            .Style.Font.Bold =
            true;

        ws.Range(row,1,row,12)
            .Style.Fill.BackgroundColor =
            XLColor.FromHtml("#DDEBF7");
    }

    private static void ApplyLayout(
        IXLWorksheet ws,
        int headerRow,
        int columnCount,
        int lastRow)
    {
        if (lastRow >= headerRow)
        {
            ws.Range(
                    headerRow,
                    1,
                    lastRow,
                    columnCount)
                .SetAutoFilter();
        }

        ws.SheetView.FreezeRows(headerRow);

        for (
            var row = headerRow + 1;
            row <= lastRow;
            row++)
        {
            if (row % 2 == 0)
            {
                ws.Range(
                        row,
                        1,
                        row,
                        columnCount)
                    .Style.Fill.BackgroundColor =
                    XLColor.FromHtml("#F7F9FC");
            }
        }
    }

    private static void SetNullableDecimal(
        IXLCell cell,
        decimal? value)
    {
        if (value.HasValue)
            SetDecimal(cell,value.Value);
    }

    private static void SetDecimal(
        IXLCell cell,
        decimal value)
    {
        cell.Value = value;
        cell.Style.NumberFormat.Format =
            "#,##0.00;[Red]-#,##0.00";
    }

    private static decimal SafeMax(
        IEnumerable<decimal> values)
    {
        var list = values.ToList();
        return list.Count == 0 ? 0 : list.Max();
    }

    private static decimal SafeMin(
        IEnumerable<decimal> values)
    {
        var list = values.ToList();
        return list.Count == 0 ? 0 : list.Min();
    }

    private static decimal SafeAverage(
        IEnumerable<decimal> values)
    {
        var list = values.ToList();
        return list.Count == 0 ? 0 : list.Average();
    }
}
