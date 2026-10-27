using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Debt;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Debt.Queries.GetBranchDebtAnalysis;

public sealed class GetBranchDebtAnalysisQueryHandler
    : IRequestHandler<
        GetBranchDebtAnalysisQuery,
        BranchDebtAnalysisDto?>
{
    private const string MaleDebtAccountCode =
        "122104";

    private const string FemaleDebtAccountCode =
        "121104";

    private static readonly string[] DebtAccountCodes =
    {
        MaleDebtAccountCode,
        FemaleDebtAccountCode
    };

    private readonly IApplicationDbContext _context;

    public GetBranchDebtAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BranchDebtAnalysisDto?> Handle(
        GetBranchDebtAnalysisQuery request,
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

        /*
         * Only active valid Financial Position
         * uploads participate.
         */
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
         * EXACT business mapping:
         *
         * 122104 = Male Branch Debt
         * 121104 = Female Branch Debt
         *
         * No name matching.
         * No extra account codes.
         */
        var debtRows =
            await _context.FinancialAccountRows
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId) &&
                        DebtAccountCodes.Contains(
                            x.AccountCode))
                .OrderBy(
                    x => x.FinancialUploadId)
                .ThenBy(
                    x => x.RowOrder)
                .ToListAsync(
                    cancellationToken);

        var rowsByUpload =
            debtRows
                .GroupBy(
                    x => x.FinancialUploadId)
                .ToDictionary(
                    group =>
                        group.Key,
                    group =>
                        group.ToList());

        var results =
            new List<BranchDebtDto>();

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

            var maleRow =
                rows.FirstOrDefault(
                    x =>
                        x.AccountCode ==
                            MaleDebtAccountCode);

            var femaleRow =
                rows.FirstOrDefault(
                    x =>
                        x.AccountCode ==
                            FemaleDebtAccountCode);

            var maleDebt =
                maleRow is null
                    ? 0
                    : GetSourceValue(
                        maleRow);

            var femaleDebt =
                femaleRow is null
                    ? 0
                    : GetSourceValue(
                        femaleRow);

            results.Add(
                new BranchDebtDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    HasFinancialPosition =
                        true,

                    HasMaleDebtAccount =
                        maleRow is not null,

                    HasFemaleDebtAccount =
                        femaleRow is not null,

                    MaleDebt =
                        maleDebt,

                    FemaleDebt =
                        femaleDebt,

                    TotalDebt =
                        maleDebt +
                        femaleDebt
                });
        }

        /*
         * A branch is considered to have Debt Data
         * only when both required fixed accounts
         * are present.
         */
        var branchesWithData =
            results
                .Where(
                    x =>
                        x.HasFinancialPosition &&
                        x.HasMaleDebtAccount &&
                        x.HasFemaleDebtAccount)
                .ToList();

        var maleTotal =
            branchesWithData
                .Sum(
                    x => x.MaleDebt);

        var femaleTotal =
            branchesWithData
                .Sum(
                    x => x.FemaleDebt);

        var totalDebt =
            branchesWithData
                .Sum(
                    x => x.TotalDebt);

        /*
         * Business Rule:
         *
         * Debt values are negative.
         *
         * The more negative the value,
         * the higher the debt.
         *
         * Example:
         *
         * -500,000,000 = Higher Debt
         * -100,000,000 = Lower Debt
         */

        var highest =
            branchesWithData
                .OrderBy(
                    x => x.TotalDebt)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        var lowest =
            branchesWithData
                .OrderByDescending(
                    x => x.TotalDebt)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        return new BranchDebtAnalysisDto
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

            AllBranchesMaleDebt =
                maleTotal,

            AllBranchesFemaleDebt =
                femaleTotal,

            AllBranchesTotalDebt =
                totalDebt,

            AverageDebt =
                branchesWithData.Count == 0
                    ? 0
                    : Math.Round(
                        totalDebt /
                        branchesWithData.Count,
                        2),

            HighestDebtBranch =
                ToRanking(
                    highest),

            LowestDebtBranch =
                ToRanking(
                    lowest),

            Branches =
                results
        };
    }

    private static decimal GetSourceValue(
        FinancialAccountRow row)
    {
        /*
         * Financial Position stores values in:
         *
         * Assets / Liabilities
         *
         * We preserve the exact accounting sign.
         *
         * No Math.Abs().
         * No sign inversion.
         *
         * Current debt rows are stored in Assets
         * as negative numbers, so they remain
         * negative in the analysis result.
         */

        if (row.Assets.HasValue &&
            row.Assets.Value != 0)
        {
            return row.Assets.Value;
        }

        if (row.Liabilities.HasValue &&
            row.Liabilities.Value != 0)
        {
            return row.Liabilities.Value;
        }

        if (row.Assets.HasValue)
            return row.Assets.Value;

        if (row.Liabilities.HasValue)
            return row.Liabilities.Value;

        return 0;
    }

    private static BranchDebtDto EmptyBranch(
        int branchId,
        string branchCode,
        string branchName,
        bool hasFinancialPosition)
    {
        return new BranchDebtDto
        {
            BranchId =
                branchId,

            BranchCode =
                branchCode,

            BranchName =
                branchName,

            HasFinancialPosition =
                hasFinancialPosition,

            HasMaleDebtAccount =
                false,

            HasFemaleDebtAccount =
                false,

            MaleDebt =
                0,

            FemaleDebt =
                0,

            TotalDebt =
                0
        };
    }

    private static DebtBranchRankingDto? ToRanking(
        BranchDebtDto? branch)
    {
        if (branch is null)
            return null;

        return new DebtBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            MaleDebt =
                branch.MaleDebt,

            FemaleDebt =
                branch.FemaleDebt,

            TotalDebt =
                branch.TotalDebt
        };
    }
}
