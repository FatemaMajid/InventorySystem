// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Stores
// File         : DeleteStoreCommandHandler.cs
// Description  : Handles store deletion requests.
// Author       : Fatema Majid
// ============================================================

using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Commands.DeleteStore;

/// <summary>
/// Handles the DeleteStoreCommand.
/// </summary>
public class DeleteStoreCommandHandler
    : IRequestHandler<DeleteStoreCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteStoreCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        DeleteStoreCommand request,
        CancellationToken cancellationToken)
    {
        // Find the store by ID.
        var store = await _context.Stores
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        // Return false when the store does not exist.
        if (store == null)
            return false;

        // Remove the store from the database context.
        _context.Stores.Remove(store);

        // Save the changes to the database.
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}