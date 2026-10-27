using InventorySystem.Application.DTOs.Store;
using MediatR;

namespace InventorySystem.Application.Features.Stores.Queries.GetAllStores;

/// <summary>
/// Query used to retrieve all stores.
/// </summary>
public class GetAllStoresQuery : IRequest<List<StoreDto>>
{
}