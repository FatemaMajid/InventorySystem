// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Branches
// File         : UpdateBranchCommand.cs
// Description  : Command used to update an existing branch.
// Author       : Fatema Majid
// ============================================================

using InventorySystem.Application.DTOs.Branch;
using MediatR;

namespace InventorySystem.Application.Features.Branches.Commands.UpdateBranch;

/// <summary>
/// Represents a request to update an existing branch.
/// </summary>
public class UpdateBranchCommand : IRequest<bool>
{
    /// <summary>
    /// The unique identifier of the branch to update.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Contains the updated branch information.
    /// </summary>
    public UpdateBranchDto Branch { get; set; } = default!;
}