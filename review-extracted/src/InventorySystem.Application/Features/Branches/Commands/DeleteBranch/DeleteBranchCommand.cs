using MediatR;

namespace InventorySystem.Application.Features.Branches.Commands.DeleteBranch;

/// <summary>
/// Command used to delete an existing branch.
/// </summary>
public class DeleteBranchCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the branch to delete.
    /// </summary>
    public int Id { get; set; }
}