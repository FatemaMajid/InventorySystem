using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Commands.DeleteStore;

public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, bool>
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
        // Find the store
        var store = await _context.Stores
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (store == null)
            return false;

        // Delete the store
        _context.Stores.Remove(store);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}