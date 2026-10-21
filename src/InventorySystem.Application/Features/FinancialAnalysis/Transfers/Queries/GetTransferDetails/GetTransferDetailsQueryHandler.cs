using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.Transfers;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.Transfers.Queries.GetTransferDetails;

public sealed class GetTransferDetailsQueryHandler
    : IRequestHandler<
        GetTransferDetailsQuery,
        BranchTransferDetailsDto?>
{
    private readonly IApplicationDbContext _context;

    public GetTransferDetailsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BranchTransferDetailsDto?> Handle(
        GetTransferDetailsQuery request,
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
                    x =>
                        new
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
                            FinancialFileType.Transfer &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Status ==
                            FinancialUploadStatus.Valid &&
                        x.Gender.HasValue)
                .Select(
                    x =>
                        new
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

        TransferGenderDetailDto? male =
            null;

        TransferGenderDetailDto? female =
            null;

        if (maleUpload is not null)
        {
            var maleRecord =
                await _context.TransferRecords
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.FinancialUploadId ==
                                maleUpload.Id &&
                            x.BranchId ==
                                request.BranchId &&
                            x.Gender ==
                                FinancialGender.Male)
                    .OrderBy(
                        x => x.RowOrder)
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (maleRecord is not null)
            {
                male =
                    new TransferGenderDetailDto
                    {
                        FinancialUploadId =
                            maleRecord.FinancialUploadId,

                        AccountCode =
                            maleRecord.AccountCode,

                        AccountName =
                            maleRecord.AccountName,

                        Gender =
                            maleRecord.Gender,

                        Value =
                            maleRecord.Value,

                        RowOrder =
                            maleRecord.RowOrder
                    };
            }
        }

        if (femaleUpload is not null)
        {
            var femaleRecord =
                await _context.TransferRecords
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.FinancialUploadId ==
                                femaleUpload.Id &&
                            x.BranchId ==
                                request.BranchId &&
                            x.Gender ==
                                FinancialGender.Female)
                    .OrderBy(
                        x => x.RowOrder)
                    .FirstOrDefaultAsync(
                        cancellationToken);

            if (femaleRecord is not null)
            {
                female =
                    new TransferGenderDetailDto
                    {
                        FinancialUploadId =
                            femaleRecord.FinancialUploadId,

                        AccountCode =
                            femaleRecord.AccountCode,

                        AccountName =
                            femaleRecord.AccountName,

                        Gender =
                            femaleRecord.Gender,

                        Value =
                            femaleRecord.Value,

                        RowOrder =
                            femaleRecord.RowOrder
                    };
            }
        }

        return new BranchTransferDetailsDto
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
}
