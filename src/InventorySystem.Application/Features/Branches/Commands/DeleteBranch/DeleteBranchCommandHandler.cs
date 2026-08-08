using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Branches.Commands.DeleteBranch;

/// <summary>
/// Handles the DeleteBranchCommand.
/// </summary>
public class DeleteBranchCommandHandler
    : IRequestHandler<DeleteBranchCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteBranchCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        DeleteBranchCommand request,
        CancellationToken cancellationToken)
    {
        // Find the branch by ID.
        var branch = await _context.Branches
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        // Branch does not exist.
        if (branch == null)
            return false;

        // Remove the branch.
        _context.Branches.Remove(branch);

        // Save changes to the database.
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}