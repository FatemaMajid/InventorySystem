using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.Branch;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Branches.Queries.GetAllBranches;

public class GetAllBranchesQueryHandler
    : IRequestHandler<GetAllBranchesQuery, List<BranchDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllBranchesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<BranchDto>> Handle(
        GetAllBranchesQuery request,
        CancellationToken cancellationToken)
    {
        var branches = await _context.Branches
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<BranchDto>>(branches);
    }
}