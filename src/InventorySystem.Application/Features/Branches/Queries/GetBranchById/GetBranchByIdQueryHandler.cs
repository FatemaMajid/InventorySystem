
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.Branch;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Branches.Queries.GetBranchById;

public class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, BranchDto> {
    private readonly IApplicationDbContext _context;

    public GetBranchByIdQueryHandler(IApplicationDbContext context){
        _context = context;
    }

    public async Task<BranchDto> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken){
        var branch = await _context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (branch == null)
            return null!;

        return new BranchDto
        {
            Id = branch.Id,
            BranchCode = branch.BranchCode,
            BranchNameArabic = branch.BranchNameArabic,
            BranchNameEnglish = branch.BranchNameEnglish,
            Address = branch.Address,
            Phone = branch.Phone,
            IsActive = branch.IsActive
        };
    }
}
