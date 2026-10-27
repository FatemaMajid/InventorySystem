// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Stores
// File         : DeleteStoreCommand.cs
// Description  : Command used to delete an existing store.
// Author       : Fatema Majid
// ============================================================

using MediatR;

namespace InventorySystem.Application.Features.Stores.Commands.DeleteStore;

/// <summary>
/// Represents a request to delete an existing store.
/// </summary>
public class DeleteStoreCommand : IRequest<bool>
{
    /// <summary>
    /// The unique identifier of the store to delete.
    /// </summary>
    public int Id { get; set; }
}