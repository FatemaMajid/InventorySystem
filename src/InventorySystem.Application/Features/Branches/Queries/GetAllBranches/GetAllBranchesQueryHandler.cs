
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.Branch;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Branches.Queries.GetAllBranches;

public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, List<BranchDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllBranchesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
    {
        var branches = await _context.Branches
            .ToListAsync(cancellationToken);

        return branches.Select(branch => new BranchDto
        {
            Id = branch.Id,
            BranchCode = branch.BranchCode,
            BranchNameArabic = branch.BranchNameArabic,
            BranchNameEnglish = branch.BranchNameEnglish,
            Address = branch.Address,
            Phone = branch.Phone,
            IsActive = branch.IsActive
        }).ToList();
    }
}
