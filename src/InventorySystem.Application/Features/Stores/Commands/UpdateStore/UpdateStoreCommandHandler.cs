// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Stores
// File         : UpdateStoreCommandHandler.cs
// Description  : Handles store update requests.
// Author       : Fatema Majid
// ============================================================

using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Commands.UpdateStore;

/// <summary>
/// Handles the UpdateStoreCommand.
/// </summary>
public class UpdateStoreCommandHandler
    : IRequestHandler<UpdateStoreCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateStoreCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Updates an existing store.
    /// </summary>
    public async Task<bool> Handle(
        UpdateStoreCommand request,
        CancellationToken cancellationToken)
    {
        // Find the existing store.
        var store = await _context.Stores
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        // Return false when the store does not exist.
        if (store == null)
            return false;

        // Verify that the specified branch exists.
        var branchExists = await _context.Branches
            .AnyAsync(
                x => x.BranchCode == request.Store.BranchCode,
                cancellationToken);

        if (!branchExists)
            throw new KeyNotFoundException("Branch not found.");

        // Update the existing entity using the DTO values.
        _mapper.Map(request.Store, store);

        // Save changes to the database.
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}