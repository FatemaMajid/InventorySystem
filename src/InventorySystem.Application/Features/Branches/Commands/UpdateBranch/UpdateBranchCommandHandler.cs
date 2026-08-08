// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Branches
// File         : UpdateBranchCommandHandler.cs
// Description  : Handles branch update requests.
// Author       : Fatema Majid
// ============================================================

using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Branches.Commands.UpdateBranch;

/// <summary>
/// Handles the UpdateBranchCommand.
/// </summary>
public class UpdateBranchCommandHandler
    : IRequestHandler<UpdateBranchCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the handler with the required dependencies.
    /// </summary>
    public UpdateBranchCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Updates an existing branch.
    /// </summary>
    public async Task<bool> Handle(
        UpdateBranchCommand request,
        CancellationToken cancellationToken)
    {
        // Find the existing branch in the database.
        var branch = await _context.Branches
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        // Return false if the branch does not exist.
        if (branch == null)
            return false;

        // Update the entity using the values from the DTO.
        _mapper.Map(request.Branch, branch);

        // Save the changes to the database.
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}