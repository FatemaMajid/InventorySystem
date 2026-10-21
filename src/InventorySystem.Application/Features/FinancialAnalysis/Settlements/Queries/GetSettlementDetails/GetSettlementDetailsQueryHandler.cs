using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Settlements;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Settlements.Queries.GetSettlementDetails;

public sealed class GetSettlementDetailsQueryHandler
    : IRequestHandler<
        GetSettlementDetailsQuery,
        BranchSettlementDetailsDto?>
{
    private readonly IApplicationDbContext _context;

    public GetSettlementDetailsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BranchSettlementDetailsDto?> Handle(
        GetSettlementDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var periodExists =
            await _context.FinancialPeriods
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.Id ==
                            request.FinancialPeriodId &&
                        !x.IsDeleted,
                    cancellationToken);

        if (!periodExists)
            return null;

        var branch =
            await _context.Branches
                .AsNoTracking()
                .Where(
                    x =>
                        x.Id ==
                            request.BranchId &&
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.BranchCode != "0" &&
                        x.BranchCode != "001" &&
                        x.BranchCode != "002")
                .Select(
                    x => new
                    {
                        x.Id,
                        x.BranchCode,
                        x.BranchNameArabic
                    })
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (branch is null)
            return null;

        var uploads =
            await _context.FinancialUploads
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialPeriodId ==
                            request.FinancialPeriodId &&
                        x.FileType ==
                            FinancialFileType.Settlement &&
                        x.BranchId ==
                            request.BranchId &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid &&
                        x.Gender.HasValue)
                .Select(
                    x => new
                    {
                        x.Id,
                        Gender =
                            x.Gender!.Value,
                        x.Version
                    })
                .ToListAsync(
                    cancellationToken);

        var maleUpload =
            uploads
                .Where(
                    x =>
                        x.Gender ==
                            FinancialGender.Male)
                .OrderByDescending(
                    x => x.Version)
                .ThenByDescending(
                    x => x.Id)
                .FirstOrDefault();

        var femaleUpload =
            uploads
                .Where(
                    x =>
                        x.Gender ==
                            FinancialGender.Female)
                .OrderByDescending(
                    x => x.Version)
                .ThenByDescending(
                    x => x.Id)
                .FirstOrDefault();

        var male =
            await BuildGenderDetails(
                maleUpload?.Id,
                FinancialGender.Male,
                cancellationToken);

        var female =
            await BuildGenderDetails(
                femaleUpload?.Id,
                FinancialGender.Female,
                cancellationToken);

        return new BranchSettlementDetailsDto
        {
            FinancialPeriodId =
                request.FinancialPeriodId,

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
        };
    }

    private async Task<SettlementGenderDetailsDto>
        BuildGenderDetails(
            int? uploadId,
            FinancialGender gender,
            CancellationToken cancellationToken)
    {
        if (!uploadId.HasValue)
        {
            return new SettlementGenderDetailsDto
            {
                Gender =
                    gender,

                HasFile =
                    false
            };
        }

        var summary =
            await _context.SettlementSummaries
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.FinancialUploadId ==
                            uploadId.Value,
                    cancellationToken);

        if (summary is null)
        {
            return new SettlementGenderDetailsDto
            {
                Gender =
                    gender,

                HasFile =
                    false,

                FinancialUploadId =
                    uploadId
            };
        }

        var rows =
            await _context.SettlementRows
                .AsNoTracking()
                .Where(
                    x =>
                        x.FinancialUploadId ==
                            uploadId.Value)
                .OrderBy(
                    x => x.RowOrder)
                .Select(
                    x =>
                        new SettlementTransactionDto
                        {
                            Id =
                                x.Id,

                            TransactionDate =
                                x.TransactionDate ?? DateTime.MinValue,

                            Description =
                                x.Description,

                            EntryNumber =
                                x.EntryNumber,

                            Debit =
                                x.Debit ?? 0,

                            Credit =
                                x.Credit ?? 0,

                            Balance =
                                x.Balance ?? 0,

                            IsMarked =
                                x.IsMarked,

                            IsMatched =
                                x.IsMatched,

                            RowOrder =
                                x.RowOrder
                        })
                .ToListAsync(
                    cancellationToken);

        return new SettlementGenderDetailsDto
        {
            Gender =
                gender,

            HasFile =
                true,

            FinancialUploadId =
                uploadId,

            PeriodDebit =
                summary.PeriodDebit,

            PeriodCredit =
                summary.PeriodCredit,

            PeriodBalance =
                summary.PeriodBalance,

            TransactionsCount =
                rows.Count,

            Transactions =
                rows
        };
    }
}
