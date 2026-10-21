using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Settlements;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Settlements.Queries.GetSettlementsAnalysis;

public sealed class GetSettlementsAnalysisQueryHandler
    : IRequestHandler<
        GetSettlementsAnalysisQuery,
        SettlementsAnalysisDto?>
{
    private readonly IApplicationDbContext _context;

    public GetSettlementsAnalysisQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SettlementsAnalysisDto?> Handle(
        GetSettlementsAnalysisQuery request,
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
                    x => new
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
                            FinancialFileType.Settlement &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid &&
                        x.BranchId.HasValue &&
                        x.Gender.HasValue &&
                        branchIds.Contains(
                            x.BranchId.Value))
                .Select(
                    x => new
                    {
                        x.Id,
                        BranchId =
                            x.BranchId!.Value,
                        Gender =
                            x.Gender!.Value,
                        x.Version
                    })
                .ToListAsync(
                    cancellationToken);

        var activeUploads =
            uploads
                .GroupBy(
                    x => new
                    {
                        x.BranchId,
                        x.Gender
                    })
                .Select(
                    group =>
                        group
                            .OrderByDescending(
                                x => x.Version)
                            .ThenByDescending(
                                x => x.Id)
                            .First())
                .ToList();

        var uploadIds =
            activeUploads
                .Select(x => x.Id)
                .ToList();

        var summaries =
            await _context.SettlementSummaries
                .AsNoTracking()
                .Where(
                    x =>
                        uploadIds.Contains(
                            x.FinancialUploadId))
                .ToListAsync(
                    cancellationToken);

        var results =
            new List<BranchSettlementDto>();

        foreach (var branch in branches)
        {
            var maleUpload =
                activeUploads
                    .FirstOrDefault(
                        x =>
                            x.BranchId ==
                                branch.Id &&
                            x.Gender ==
                                FinancialGender.Male);

            var femaleUpload =
                activeUploads
                    .FirstOrDefault(
                        x =>
                            x.BranchId ==
                                branch.Id &&
                            x.Gender ==
                                FinancialGender.Female);

            var maleSummary =
                maleUpload is null
                    ? null
                    : summaries.FirstOrDefault(
                        x =>
                            x.FinancialUploadId ==
                                maleUpload.Id);

            var femaleSummary =
                femaleUpload is null
                    ? null
                    : summaries.FirstOrDefault(
                        x =>
                            x.FinancialUploadId ==
                                femaleUpload.Id);

            var male =
                new SettlementGenderSummaryDto
                {
                    HasFile =
                        maleUpload is not null &&
                        maleSummary is not null,

                    FinancialUploadId =
                        maleUpload?.Id,

                    Debit =
                        maleSummary?.PeriodDebit ?? 0,

                    Credit =
                        maleSummary?.PeriodCredit ?? 0,

                    Balance =
                        maleSummary?.PeriodBalance ?? 0
                };

            var female =
                new SettlementGenderSummaryDto
                {
                    HasFile =
                        femaleUpload is not null &&
                        femaleSummary is not null,

                    FinancialUploadId =
                        femaleUpload?.Id,

                    Debit =
                        femaleSummary?.PeriodDebit ?? 0,

                    Credit =
                        femaleSummary?.PeriodCredit ?? 0,

                    Balance =
                        femaleSummary?.PeriodBalance ?? 0
                };

            results.Add(
                new BranchSettlementDto
                {
                    BranchId =
                        branch.Id,

                    BranchCode =
                        branch.BranchCode,

                    BranchName =
                        branch.BranchNameArabic,

                    Male =
                        male,

                    Female =
                        female
                });
        }

        var anyData =
            results
                .Where(
                    x => x.HasAnyData)
                .ToList();

        var complete =
            results
                .Where(
                    x => x.HasCompleteData)
                .ToList();

        var maleRows =
            results
                .Where(
                    x => x.Male.HasFile)
                .ToList();

        var femaleRows =
            results
                .Where(
                    x => x.Female.HasFile)
                .ToList();

        var maleDebit =
            maleRows.Sum(
                x => x.Male.Debit);

        var maleCredit =
            maleRows.Sum(
                x => x.Male.Credit);

        var maleBalance =
            maleRows.Sum(
                x => x.Male.Balance);

        var femaleDebit =
            femaleRows.Sum(
                x => x.Female.Debit);

        var femaleCredit =
            femaleRows.Sum(
                x => x.Female.Credit);

        var femaleBalance =
            femaleRows.Sum(
                x => x.Female.Balance);

        var highest =
            complete
                .OrderByDescending(
                    x => x.TotalBalance)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        var lowest =
            complete
                .OrderBy(
                    x => x.TotalBalance)
                .ThenBy(
                    x => x.BranchCode)
                .FirstOrDefault();

        return new SettlementsAnalysisDto
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

            MaleFilesCount =
                maleRows.Count,

            FemaleFilesCount =
                femaleRows.Count,

            BranchesWithAnyData =
                anyData.Count,

            BranchesWithCompleteData =
                complete.Count,

            BranchesWithoutCompleteData =
                branches.Count -
                complete.Count,

            AllBranchesMaleDebit =
                maleDebit,

            AllBranchesMaleCredit =
                maleCredit,

            AllBranchesMaleBalance =
                maleBalance,

            AllBranchesFemaleDebit =
                femaleDebit,

            AllBranchesFemaleCredit =
                femaleCredit,

            AllBranchesFemaleBalance =
                femaleBalance,

            AllBranchesDebit =
                maleDebit +
                femaleDebit,

            AllBranchesCredit =
                maleCredit +
                femaleCredit,

            AllBranchesBalance =
                maleBalance +
                femaleBalance,

            AverageCompleteBranchBalance =
                complete.Count == 0
                    ? 0
                    : Math.Round(
                        complete.Sum(
                            x => x.TotalBalance) /
                        complete.Count,
                        2),

            HighestBalanceBranch =
                ToRanking(
                    highest),

            LowestBalanceBranch =
                ToRanking(
                    lowest),

            Branches =
                results
        };
    }

    private static SettlementBranchRankingDto? ToRanking(
        BranchSettlementDto? branch)
    {
        if (branch is null)
            return null;

        return new SettlementBranchRankingDto
        {
            BranchId =
                branch.BranchId,

            BranchCode =
                branch.BranchCode,

            BranchName =
                branch.BranchName,

            TotalDebit =
                branch.TotalDebit,

            TotalCredit =
                branch.TotalCredit,

            TotalBalance =
                branch.TotalBalance
        };
    }
}
