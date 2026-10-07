using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialMonitoring;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetMissingFinancialFiles;

public class GetMissingFinancialFilesQueryHandler
    : IRequestHandler<GetMissingFinancialFilesQuery, List<MissingFinancialFileDto>?>
{
    private readonly IApplicationDbContext _context;

    public GetMissingFinancialFilesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MissingFinancialFileDto>?> Handle(
        GetMissingFinancialFilesQuery request,
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
                    x.IsActive &&
                    x.Status ==
                        FinancialUploadStatus.Valid)
            .ToListAsync(cancellationToken);

        var result =
            new List<MissingFinancialFileDto>();

        AddTransferIfMissing(
            result,
            uploads,
            FinancialGender.Male);

        AddTransferIfMissing(
            result,
            uploads,
            FinancialGender.Female);

        foreach (var branch in branches)
        {
            AddBranchFileIfMissing(
                result,
                uploads,
                branch.Id,
                branch.BranchCode,
                branch.BranchNameArabic,
                FinancialFileType.IncomeStatement,
                null);

            AddBranchFileIfMissing(
                result,
                uploads,
                branch.Id,
                branch.BranchCode,
                branch.BranchNameArabic,
                FinancialFileType.FinancialPosition,
                null);

            AddBranchFileIfMissing(
                result,
                uploads,
                branch.Id,
                branch.BranchCode,
                branch.BranchNameArabic,
                FinancialFileType.Settlement,
                FinancialGender.Male);

            AddBranchFileIfMissing(
                result,
                uploads,
                branch.Id,
                branch.BranchCode,
                branch.BranchNameArabic,
                FinancialFileType.Settlement,
                FinancialGender.Female);
        }

        return result;
    }

    private static void AddTransferIfMissing(
        List<MissingFinancialFileDto> result,
        IEnumerable<FinancialUpload> uploads,
        FinancialGender gender)
    {
        var exists = uploads.Any(
            x =>
                x.FileType ==
                    FinancialFileType.Transfer &&
                x.BranchId == null &&
                x.Gender == gender);

        if (exists)
            return;

        result.Add(
            new MissingFinancialFileDto
            {
                BranchId = null,
                BranchCode = null,
                BranchNameArabic = null,
                FileType =
                    FinancialFileType.Transfer,
                FileTypeName = "Transfer",
                Gender = gender,
                GenderName =
                    GetGenderName(gender),
                Scope =
                    FinancialFileScope.AllBranches
            });
    }

    private static void AddBranchFileIfMissing(
        List<MissingFinancialFileDto> result,
        IEnumerable<FinancialUpload> uploads,
        int branchId,
        string branchCode,
        string branchNameArabic,
        FinancialFileType fileType,
        FinancialGender? gender)
    {
        var exists = uploads.Any(
            x =>
                x.BranchId == branchId &&
                x.FileType == fileType &&
                x.Gender == gender);

        if (exists)
            return;

        result.Add(
            new MissingFinancialFileDto
            {
                BranchId = branchId,
                BranchCode = branchCode,
                BranchNameArabic =
                    branchNameArabic,
                FileType = fileType,
                FileTypeName =
                    GetFileTypeName(fileType),
                Gender = gender,
                GenderName =
                    gender.HasValue
                        ? GetGenderName(
                            gender.Value)
                        : null,
                Scope =
                    FinancialFileScope.SingleBranch
            });
    }

    private static string GetFileTypeName(
        FinancialFileType fileType)
    {
        return fileType switch
        {
            FinancialFileType.IncomeStatement =>
                "IncomeStatement",
            FinancialFileType.FinancialPosition =>
                "FinancialPosition",
            FinancialFileType.Transfer =>
                "Transfer",
            FinancialFileType.Settlement =>
                "Settlement",
            _ => fileType.ToString()
        };
    }

    private static string GetGenderName(
        FinancialGender gender)
    {
        return gender switch
        {
            FinancialGender.Male => "Male",
            FinancialGender.Female => "Female",
            _ => gender.ToString()
        };
    }
}
