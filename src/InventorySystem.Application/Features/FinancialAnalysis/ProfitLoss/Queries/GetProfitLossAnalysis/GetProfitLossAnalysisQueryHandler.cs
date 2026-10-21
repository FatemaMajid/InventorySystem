using System.Text;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.ProfitLoss;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.ProfitLoss.Queries.GetProfitLossAnalysis;

public sealed class GetProfitLossAnalysisQueryHandler
    : IRequestHandler<
        GetProfitLossAnalysisQuery,
        ProfitLossAnalysisDto?>
{
    private const string ResultAccountCode =
        "RESULT";

    private readonly IApplicationDbContext _context;

    public GetProfitLossAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProfitLossAnalysisDto?> Handle(
        GetProfitLossAnalysisQuery request,
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
                            FinancialFileType.IncomeStatement &&
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
                .GroupBy(x => x.BranchId)
                .ToDictionary(
                    x => x.Key,
                    x =>
                        x.OrderByDescending(
                                y => y.Version)
                            .ThenByDescending(
                                y => y.Id)
                            .First());

        var activeUploadIds =
            activeUploadsByBranch
                .Values
                .Select(x => x.Id)
                .ToList();

        var resultRows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        activeUploadIds.Contains(
                            x.FinancialUploadId) &&
                        x.AccountCode ==
                            ResultAccountCode)
                .OrderBy(
                    x => x.FinancialUploadId)
                .ThenByDescending(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var resultRowByUpload =
            resultRows
                .GroupBy(
                    x => x.FinancialUploadId)
                .ToDictionary(
                    x => x.Key,
                    x => x.First());

        var branchResults =
            new List<BranchProfitLossDto>();

        foreach (var branch in branches)
        {
            if (!activeUploadsByBranch
                    .TryGetValue(
                        branch.Id,
                        out var upload))
            {
                branchResults.Add(
                    EmptyBranch(
                        branch.Id,
                        branch.BranchCode,
                        branch.BranchNameArabic,
                        false));

                continue;
            }

            if (!resultRowByUpload
                    .TryGetValue(
                        upload.Id,
                        out var resultRow))
            {
                branchResults.Add(
                    EmptyBranch(
                        branch.Id,
                        branch.BranchCode,
                        branch.BranchNameArabic,
                        true));

                continue;
            }

            var sourceType =
                GetSourceResultType(
                    resultRow.AccountName);

            var amount =
                GetResultAmount(
                    resultRow);

            var resultType =
                amount == 0
                    ? "BreakEven"
                    : sourceType;

            decimal signedValue =
                resultType switch
                {
                    "Profit" =>
                        amount,

                    "Loss" =>
                        -amount,

                    _ =>
                        0
                };

            branchResults.Add(
                new BranchProfitLossDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasIncomeStatement =
                        true,

                    HasProfitLossResult =
                        true,

                    ResultType =
                        resultType,

                    ResultValue =
                        amount,

                    SignedValue =
                        signedValue,

                    SourceLabel =
                        resultRow.AccountName
                });
        }

        var branchesWithData =
            branchResults
                .Where(
                    x =>
                        x.HasProfitLossResult)
                .ToList();

        var profitable =
            branchesWithData
                .Where(
                    x =>
                        x.SignedValue > 0)
                .ToList();

        var losses =
            branchesWithData
                .Where(
                    x =>
                        x.SignedValue < 0)
                .ToList();

        var breakEven =
            branchesWithData
                .Where(
                    x =>
                        x.SignedValue == 0)
                .ToList();

        var overall =
            branchesWithData
                .Sum(
                    x => x.SignedValue);

        var highestProfit =
            profitable
                .OrderByDescending(
                    x => x.SignedValue)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        /*
         * Most negative SignedValue =
         * largest financial loss.
         */
        var highestLoss =
            losses
                .OrderBy(
                    x => x.SignedValue)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        return new ProfitLossAnalysisDto
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

            ProfitableBranches =
                profitable.Count,

            LossBranches =
                losses.Count,

            BreakEvenBranches =
                breakEven.Count,

            TotalProfit =
                profitable.Sum(
                    x => x.ResultValue),

            /*
             * Human-readable total loss:
             * positive magnitude.
             */
            TotalLoss =
                losses.Sum(
                    x => x.ResultValue),

            /*
             * Signed period result:
             *
             * Profit => +
             * Loss   => -
             */
            OverallResult =
                overall,

            OverallResultType =
                GetOverallResultType(
                    overall),

            AverageResult =
                branchesWithData.Count == 0
                    ? 0
                    : Math.Round(
                        overall /
                        branchesWithData.Count,
                        2),

            HighestProfitBranch =
                ToRanking(
                    highestProfit),

            HighestLossBranch =
                ToRanking(
                    highestLoss),

            Branches =
                branchResults
        };
    }

    private static BranchProfitLossDto EmptyBranch(
        int branchId,
        string branchCode,
        string branchName,
        bool hasIncomeStatement)
    {
        return new BranchProfitLossDto
        {
            BranchId =
                branchId,

            BranchCode =
                branchCode,

            BranchName =
                branchName,

            HasIncomeStatement =
                hasIncomeStatement,

            HasProfitLossResult =
                false,

            ResultType =
                string.Empty,

            ResultValue =
                0,

            SignedValue =
                0,

            SourceLabel =
                null
        };
    }

    private static decimal GetResultAmount(
        FinancialAccountRow row)
    {
        /*
         * Profit/Loss final row does not rely
         * on Balance.
         *
         * Example from the actual file:
         *
         * الربح
         * Debit   = 0
         * Credit  = 43,667,295.03
         * Balance = 0
         *
         * A loss file can place its value in
         * Debit instead.
         *
         * Take the non-zero numeric magnitude.
         */

        var debit =
            Math.Abs(
                row.Debit ?? 0);

        var credit =
            Math.Abs(
                row.Credit ?? 0);

        var balance =
            Math.Abs(
                row.Balance ?? 0);

        return Math.Max(
            balance,
            Math.Max(
                debit,
                credit));
    }

    private static string GetSourceResultType(
        string value)
    {
        var normalized =
            NormalizeArabic(
                value);

        if (normalized.Contains(
                NormalizeArabic("خسارة"),
                StringComparison.Ordinal))
        {
            return "Loss";
        }

        if (normalized.Contains(
                NormalizeArabic("ربح"),
                StringComparison.Ordinal))
        {
            return "Profit";
        }

        if (normalized.Contains(
                "loss",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Loss";
        }

        if (normalized.Contains(
                "profit",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Profit";
        }

        return "BreakEven";
    }

    private static string GetOverallResultType(
        decimal value)
    {
        if (value > 0)
            return "Profit";

        if (value < 0)
            return "Loss";

        return "BreakEven";
    }

    private static ProfitLossBranchRankingDto? ToRanking(
        BranchProfitLossDto? branch)
    {
        if (branch is null)
            return null;

        return new ProfitLossBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            ResultType =
                branch.ResultType,

            ResultValue =
                branch.ResultValue,

            SignedValue =
                branch.SignedValue
        };
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
                .Replace("ئ","ي");

        var builder =
            new StringBuilder();

        foreach (var character in normalized)
        {
            var category =
                System.Globalization.CharUnicodeInfo
                    .GetUnicodeCategory(
                        character);

            if (category !=
                    System.Globalization
                        .UnicodeCategory
                        .NonSpacingMark &&
                category !=
                    System.Globalization
                        .UnicodeCategory
                        .Format)
            {
                builder.Append(
                    character);
            }
        }

        return builder
            .ToString()
            .Trim();
    }
}
