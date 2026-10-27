using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialUpload;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetFinancialUploads;

public class GetFinancialUploadsQueryHandler
    : IRequestHandler<
        GetFinancialUploadsQuery,
        List<FinancialUploadDto>>
{
    private readonly IApplicationDbContext _context;

    public GetFinancialUploadsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FinancialUploadDto>> Handle(
        GetFinancialUploadsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.FinancialUploads
            .AsNoTracking()
            .Where(
                x =>
                    x.FinancialPeriodId ==
                        request.FinancialPeriodId &&
                    x.IsActive &&
                    !x.IsDeleted)
            .OrderBy(x => x.FileType)
            .ThenBy(x =>
                x.Branch != null
                    ? x.Branch.BranchCode
                    : string.Empty)
            .ThenBy(x => x.Gender)
            .Select(
                x =>
                    new FinancialUploadDto
                    {
                        Id =
                            x.Id,

                        FinancialPeriodId =
                            x.FinancialPeriodId,

                        FileType =
                            x.FileType,

                        BranchId =
                            x.BranchId,

                        BranchCode =
                            x.Branch != null
                                ? x.Branch.BranchCode
                                : null,

                        BranchNameArabic =
                            x.Branch != null
                                ? x.Branch.BranchNameArabic
                                : null,

                        Gender =
                            x.Gender,

                        Scope =
                            x.Scope,

                        OriginalFileName =
                            x.OriginalFileName,

                        FileSize =
                            x.FileSize,

                        Status =
                            x.Status,

                        Version =
                            x.Version,

                        IsActive =
                            x.IsActive,

                        UploadedAt =
                            x.UploadedAt,

                        UploadedBy =
                            x.UploadedBy,

                        ValidationMessage =
                            x.ValidationMessage
                    })
            .ToListAsync(
                cancellationToken);
    }
}
