using InventorySystem.Application.DTOs.Store;
using MediatR;

namespace InventorySystem.Application.Features.Stores.Commands.CreateStore;

/// <summary>
/// Command used to create a new store.
/// </summary>
public class CreateStoreCommand : IRequest<int>
{
    /// <summary>
    /// Contains the information required to create the store.
    /// </summary>
    public CreateStoreDto Store { get; set; } = default!;
}