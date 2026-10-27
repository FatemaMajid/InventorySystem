// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Branches
// File         : GetBranchByIdQueryHandler.cs
// Description  : Handles requests to retrieve a branch by ID.
// Author       : Fatema Majid
// ============================================================

using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.Branch;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Branches.Queries.GetBranchById;

/// <summary>
/// Handles the GetBranchByIdQuery.
/// </summary>
public class GetBranchByIdQueryHandler
    : IRequestHandler<GetBranchByIdQuery, BranchDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the handler with the required dependencies.
    /// </summary>
    public GetBranchByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a branch by its unique identifier.
    /// </summary>
    public async Task<BranchDto> Handle(
        GetBranchByIdQuery request,
        CancellationToken cancellationToken)
    {
        // Retrieve the branch from the database.
        var branch = await _context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        // Return null if the branch does not exist.
        if (branch == null)
            return null!;

        // Map the entity to a DTO before returning it.
        return _mapper.Map<BranchDto>(branch);
    }
}