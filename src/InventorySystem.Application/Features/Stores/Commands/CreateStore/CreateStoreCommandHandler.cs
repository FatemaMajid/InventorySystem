using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Commands.CreateStore;

/// <summary>
/// Handles store creation requests.
/// </summary>
public class CreateStoreCommandHandler
    : IRequestHandler<CreateStoreCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateStoreCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(
        CreateStoreCommand request,
        CancellationToken cancellationToken)
    {
        // Verify that the specified branch exists.
        var branchExists = await _context.Branches
            .AnyAsync(
                x => x.BranchCode == request.Store.BranchCode,
                cancellationToken);

        if (!branchExists)
            throw new KeyNotFoundException("Branch not found.");

        // Map the DTO to the Store entity.
        var store = _mapper.Map<Store>(request.Store);

        // Add the new store to the database context.
        _context.Stores.Add(store);

        // Save the changes to the database.
        await _context.SaveChangesAsync(cancellationToken);

        // Return the generated store ID.
        return store.Id;
    }
}