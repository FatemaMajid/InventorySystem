using System.Globalization;
using System.Text;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Cash;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Cash.Queries.GetCashAnalysis;

public sealed class GetCashAnalysisQueryHandler
    : IRequestHandler<
        GetCashAnalysisQuery,
        CashAnalysisDto?>
{
    private const string CashRootCode =
        "1801";

    private readonly IApplicationDbContext _context;

    public GetCashAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CashAnalysisDto?> Handle(
        GetCashAnalysisQuery request,
        CancellationToken cancellationToken)
    {
        var period =
            await _context.FinancialPeriods
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id ==
                            request.FinancialPeriodId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (period is null)
            return null;

        var branches =
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
                    x => x.BranchCode)
                .Select(
                    x =>
                        new
                        {
                            x.Id,
                            x.BranchCode,
                            x.BranchNameArabic
                        })
                .ToListAsync(
                    cancellationToken);

        var branchIds =
            branches
                .Select(x => x.Id)
                .ToList();

        var uploads =
            await _context.FinancialUploads
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialPeriodId ==
                            request.FinancialPeriodId &&
                        x.FileType ==
                            FinancialFileType.FinancialPosition &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid &&
                        x.BranchId.HasValue &&
                        branchIds.Contains(
                            x.BranchId.Value))
                .Select(
                    x =>
                        new
                        {
                            x.Id,
                            BranchId =
                                x.BranchId!.Value,
                            x.Version
                        })
                .ToListAsync(
                    cancellationToken);

        var activeUploadsByBranch =
            uploads
                .GroupBy(
                    x => x.BranchId)
                .ToDictionary(
                    group =>
                        group.Key,
                    group =>
                        group
                            .OrderByDescending(
                                x => x.Version)
                            .ThenByDescending(
                                x => x.Id)
                            .First());

        var uploadIds =
            activeUploadsByBranch
                .Values
                .Select(x => x.Id)
                .ToList();

        /*
         * Cash hierarchy only.
         *
         * Example:
         *
         * 1801      صناديق الفروع
         * 180102    صناديق الجمعية      <- Group
         * 1801021   صندوق مركز          <- Child
         * 1801022   صندوق كاشير         <- Child
         * 1801025   مردود               <- Child
         * 1801026   خردة                <- Child
         * 1801028   كي كارد             <- Child
         *
         * TotalCash = SUM(children)
         *
         * Parent value is NOT added.
         */
        var cashRows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId) &&
                        x.AccountCode.StartsWith(
                            CashRootCode))
                .OrderBy(
                    x => x.FinancialUploadId)
                .ThenBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var rowsByUpload =
            cashRows
                .GroupBy(
                    x => x.FinancialUploadId)
                .ToDictionary(
                    group =>
                        group.Key,
                    group =>
                        group.ToList());

        var results =
            new List<BranchCashDto>();

        foreach (var branch in branches)
        {
            if (!activeUploadsByBranch.TryGetValue(
                    branch.Id,
                    out var upload))
            {
                results.Add(
                    EmptyBranch(
                        branch.Id,
                        branch.BranchCode,
                        branch.BranchNameArabic,
                        false));

                continue;
            }

            rowsByUpload.TryGetValue(
                upload.Id,
                out var rows);

            rows ??=
                new List<FinancialAccountRow>();

            var cashGroup =
                FindBranchCashGroup(
                    rows,
                    branch.BranchNameArabic);

            if (cashGroup is null)
            {
                results.Add(
                    EmptyBranch(
                        branch.Id,
                        branch.BranchCode,
                        branch.BranchNameArabic,
                        true));

                continue;
            }

            /*
             * Only children below the branch
             * cash group are included.
             *
             * The group/parent itself is excluded.
             */
            var cashboxes =
                rows
                    .Where(
                        x =>
                            x.AccountCode !=
                                cashGroup.AccountCode &&
                            x.AccountCode.StartsWith(
                                cashGroup.AccountCode,
                                StringComparison.Ordinal))
                    .OrderBy(
                        x => x.RowOrder)
                    .ToList();

            var totalCash =
                cashboxes.Sum(
                    GetCashValue);

            results.Add(
                new BranchCashDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasFinancialPosition =
                        true,

                    HasCashGroup =
                        true,

                    CashGroupAccountCode =
                        cashGroup.AccountCode,

                    CashGroupAccountName =
                        cashGroup.AccountName,

                    CashboxesCount =
                        cashboxes.Count,

                    TotalCash =
                        totalCash
                });
        }

        var branchesWithData =
            results
                .Where(
                    x =>
                        x.HasFinancialPosition &&
                        x.HasCashGroup)
                .ToList();

        var total =
            branchesWithData
                .Sum(
                    x => x.TotalCash);

        var highest =
            branchesWithData
                .OrderByDescending(
                    x => x.TotalCash)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        var lowest =
            branchesWithData
                .OrderBy(
                    x => x.TotalCash)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        return new CashAnalysisDto
        {
            FinancialPeriodId =
                period.Id,

            StartDate =
                period.StartDate,

            EndDate =
                period.EndDate,

            Status =
                period.Status,

            ActiveBranches =
                branches.Count,

            BranchesWithData =
                branchesWithData.Count,

            BranchesWithoutData =
                branches.Count -
                branchesWithData.Count,

            AllBranchesCash =
                total,

            AverageCash =
                branchesWithData.Count == 0
                    ? 0
                    : Math.Round(
                        total /
                        branchesWithData.Count,
                        2),

            HighestCashBranch =
                ToRanking(
                    highest),

            LowestCashBranch =
                ToRanking(
                    lowest),

            Branches =
                results
        };
    }

    private static FinancialAccountRow? FindBranchCashGroup(
        IReadOnlyCollection<FinancialAccountRow> rows,
        string branchName)
    {
        var branchToken =
            NormalizeBranchName(
                branchName);

        /*
         * Select the branch cash group under 1801.
         *
         * We do not hardcode 180102 because other
         * branches have their own group code.
         */
        return rows
            .Where(
                x =>
                    x.AccountCode !=
                        CashRootCode &&
                    x.AccountCode.StartsWith(
                        CashRootCode,
                        StringComparison.Ordinal) &&
                    NormalizeBranchName(
                        x.AccountName)
                        .Contains(
                            branchToken,
                            StringComparison.Ordinal))
            .OrderBy(
                x => x.AccountCode.Length)
            .ThenBy(
                x => x.RowOrder)
            .FirstOrDefault();
    }

    private static decimal GetCashValue(
        FinancialAccountRow row)
    {
        /*
         * Cash is read from the Assets value
         * stored by FinancialPosition parser.
         *
         * No Math.Abs()
         * No Parent calculation
         * No additional account mapping
         */
        return row.Assets ?? 0;
    }

    private static string NormalizeBranchName(
        string value)
    {
        var normalized =
            NormalizeArabic(
                value);

        var wordsToRemove =
            new[]
            {
                "فرع",
                "صناديق",
                "صندوق",
                "مركز",
                "كاشير",
                "مردود",
                "الخردة",
                "خردة",
                "كي"
            };

        foreach (var word in wordsToRemove)
        {
            normalized =
                normalized.Replace(
                    NormalizeArabic(word),
                    " ",
                    StringComparison.Ordinal);
        }

        return string.Join(
            ' ',
            normalized.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries));
    }

    private static string NormalizeArabic(
        string value)
    {
        if (string.IsNullOrWhiteSpace(
                value))
        {
            return string.Empty;
        }

        var normalized =
            value
                .Trim()
                .Replace("ـ","")
                .Replace("أ","ا")
                .Replace("إ","ا")
                .Replace("آ","ا")
                .Replace("ى","ي")
                .Replace("ؤ","و")
                .Replace("ئ","ي")
                .Replace("-"," ")
                .Replace("_"," ");

        var builder =
            new StringBuilder();

        foreach (var character in normalized)
        {
            var category =
                CharUnicodeInfo.GetUnicodeCategory(
                    character);

            if (category !=
                    UnicodeCategory.NonSpacingMark &&
                category !=
                    UnicodeCategory.Format)
            {
                builder.Append(
                    character);
            }
        }

        return string.Join(
            ' ',
            builder
                .ToString()
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries));
    }

    private static BranchCashDto EmptyBranch(
        int branchId,
        string branchCode,
        string branchName,
        bool hasFinancialPosition)
    {
        return new BranchCashDto
        {
            BranchId =
                branchId,

            BranchCode =
                branchCode,

            BranchName =
                branchName,

            HasFinancialPosition =
                hasFinancialPosition,

            HasCashGroup =
                false,

            CashGroupAccountCode =
                null,

            CashGroupAccountName =
                null,

            CashboxesCount =
                0,

            TotalCash =
                0
        };
    }

    private static CashBranchRankingDto? ToRanking(
        BranchCashDto? branch)
    {
        if (branch is null)
            return null;

        return new CashBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            TotalCash =
                branch.TotalCash
        };
    }
}
