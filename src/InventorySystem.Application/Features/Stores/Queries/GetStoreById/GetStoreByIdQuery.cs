using InventorySystem.Application.DTOs.Store;
using MediatR;

namespace InventorySystem.Application.Features.Stores.Queries.GetStoreById;

/// <summary>
/// Query used to retrieve a store by its ID.
/// </summary>
public class GetStoreByIdQuery : IRequest<StoreDto>
{
    /// <summary>
    /// The unique identifier of the store.
    /// </summary>
    public int Id { get; set; }
}