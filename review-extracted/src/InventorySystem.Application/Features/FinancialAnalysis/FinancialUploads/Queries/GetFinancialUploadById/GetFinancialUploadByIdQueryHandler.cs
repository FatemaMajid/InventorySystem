using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialUpload;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialUploads.Queries.GetFinancialUploadById;

public class GetFinancialUploadByIdQueryHandler
    : IRequestHandler<
        GetFinancialUploadByIdQuery,
        FinancialUploadDto?>
{
    private readonly IApplicationDbContext _context;

    public GetFinancialUploadByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialUploadDto?> Handle(
        GetFinancialUploadByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.FinancialUploads
            .AsNoTracking()
            .Where(
                x =>
                    x.Id == request.UploadId &&
                    !x.IsDeleted)
            .Select(
                x =>
                    new FinancialUploadDto
                    {
                        Id = x.Id,
                        FinancialPeriodId =
                            x.FinancialPeriodId,
                        FileType = x.FileType,
                        BranchId = x.BranchId,
                        BranchCode =
                            x.Branch != null
                                ? x.Branch.BranchCode
                                : null,
                        BranchNameArabic =
                            x.Branch != null
                                ? x.Branch.BranchNameArabic
                                : null,
                        Gender = x.Gender,
                        Scope = x.Scope,
                        OriginalFileName =
                            x.OriginalFileName,
                        FileSize = x.FileSize,
                        Status = x.Status,
                        Version = x.Version,
                        IsActive = x.IsActive,
                        UploadedAt = x.UploadedAt,
                        UploadedBy = x.UploadedBy,
                        ValidationMessage =
                            x.ValidationMessage
                    })
            .FirstOrDefaultAsync(
                cancellationToken);
    }
}
