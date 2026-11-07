using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialMonitoring;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodBranches;

public class GetFinancialPeriodBranchesQueryHandler
    : IRequestHandler<GetFinancialPeriodBranchesQuery, List<FinancialBranchStatusDto>?>
{
    private readonly IApplicationDbContext _context;

    public GetFinancialPeriodBranchesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FinancialBranchStatusDto>?> Handle(
        GetFinancialPeriodBranchesQuery request,
        CancellationToken cancellationToken)
    {
        var periodExists = await _context.FinancialPeriods
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.Id == request.FinancialPeriodId &&
                    !x.IsDeleted,
                cancellationToken);

        if (!periodExists)
            return null;

        var branches = await _context.Branches
            .AsNoTracking()
            .Where(
                x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.BranchCode != "0" &&
                    x.BranchCode != "001" &&
                    x.BranchCode != "002")
            .OrderBy(x => x.BranchCode)
            .Select(
                x => new
                {
                    x.Id,
                    x.BranchCode,
                    x.BranchNameArabic
                })
            .ToListAsync(cancellationToken);

        var uploads = await _context.FinancialUploads
            .AsNoTracking()
            .Where(
                x =>
                    x.FinancialPeriodId ==
                        request.FinancialPeriodId &&
                    !x.IsDeleted &&
                    x.IsActive)
            .ToListAsync(cancellationToken);

        var result =
            new List<FinancialBranchStatusDto>();

        foreach (var branch in branches)
        {
            var incomeStatement =
                GetSlot(
                    uploads,
                    branch.Id,
                    FinancialFileType.IncomeStatement,
                    null);

            var financialPosition =
                GetSlot(
                    uploads,
                    branch.Id,
                    FinancialFileType.FinancialPosition,
                    null);

            var settlementMale =
                GetSlot(
                    uploads,
                    branch.Id,
                    FinancialFileType.Settlement,
                    FinancialGender.Male);

            var settlementFemale =
                GetSlot(
                    uploads,
                    branch.Id,
                    FinancialFileType.Settlement,
                    FinancialGender.Female);

            var slots = new[]
            {
                incomeStatement,
                financialPosition,
                settlementMale,
                settlementFemale
            };

            var uploadedFiles =
                slots.Count(
                    x =>
                        x.IsUploaded &&
                        x.Status ==
                            FinancialUploadStatus.Valid);

            result.Add(
                new FinancialBranchStatusDto
                {
                    BranchId = branch.Id,
                    BranchCode =
                        branch.BranchCode,
                    BranchNameArabic =
                        branch.BranchNameArabic,
                    UploadedFiles =
                        uploadedFiles,
                    RequiredFiles = 4,
                    IsComplete =
                        uploadedFiles == 4,
                    IncomeStatement =
                        incomeStatement,
                    FinancialPosition =
                        financialPosition,
                    SettlementMale =
                        settlementMale,
                    SettlementFemale =
                        settlementFemale
                });
        }

        return result;
    }

    private static FinancialUploadSlotDto GetSlot(
        IEnumerable<FinancialUpload> uploads,
        int branchId,
        FinancialFileType fileType,
        FinancialGender? gender)
    {
        var upload = uploads
            .Where(
                x =>
                    x.BranchId == branchId &&
                    x.FileType == fileType &&
                    x.Gender == gender)
            .OrderByDescending(
                x => x.Version)
            .FirstOrDefault();

        if (upload is null)
        {
            return new FinancialUploadSlotDto
            {
                FileType = fileType,
                Gender = gender,
                IsUploaded = false,
                UploadId = null,
                Status = null,
                Version = null,
                OriginalFileName = null,
                ValidationMessage = null
            };
        }

        return new FinancialUploadSlotDto
        {
            FileType = fileType,
            Gender = gender,
            IsUploaded = true,
            UploadId = upload.Id,
            Status = upload.Status,
            Version = upload.Version,
            OriginalFileName =
                upload.OriginalFileName,
            ValidationMessage =
                upload.ValidationMessage
        };
    }
}
