using InventorySystem.Application.DTOs.Branch;
using MediatR;

namespace InventorySystem.Application.Features.Branches.Commands.CreateBranch;

/// <summary>
/// Command used to create a new branch.
/// This command carries the data entered by the user
/// and is sent to MediatR for processing.
/// </summary>
public class CreateBranchCommand : IRequest<int>
{
    /// <summary>
    /// Contains all information required to create a branch.
    /// The data comes from the API request body.
    /// </summary>
    public CreateBranchDto Branch { get; set; } = default!;
}
