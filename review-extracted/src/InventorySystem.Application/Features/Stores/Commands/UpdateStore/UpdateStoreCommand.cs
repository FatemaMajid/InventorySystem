// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Stores
// File         : UpdateStoreCommand.cs
// Description  : Command used to update an existing store.
// Author       : Fatema Majid
// ============================================================

using InventorySystem.Application.DTOs.Store;
using MediatR;

namespace InventorySystem.Application.Features.Stores.Commands.UpdateStore;

/// <summary>
/// Represents a request to update an existing store.
/// </summary>
public class UpdateStoreCommand : IRequest<bool>
{
    /// <summary>
    /// The unique identifier of the store to update.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Contains the updated store information.
    /// </summary>
    public UpdateStoreDto Store { get; set; } = default!;
}