// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Branches
// File         : GetBranchByIdQuery.cs
// Description  : Query used to retrieve a branch by its ID.
// Author       : Fatema Majid
// ============================================================

using InventorySystem.Application.DTOs.Branch;
using MediatR;

namespace InventorySystem.Application.Features.Branches.Queries.GetBranchById;

/// <summary>
/// Represents a request to retrieve a branch by its ID.
/// </summary>
public class GetBranchByIdQuery : IRequest<BranchDto>
{
    /// <summary>
    /// The unique identifier of the branch to retrieve.
    /// </summary>
    public int Id { get; set; }
}