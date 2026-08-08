// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Branches
// File         : CreateBranchCommandHandler.cs
// Description  : Handles branch creation requests.
// ============================================================

using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities;
using MediatR;

namespace InventorySystem.Application.Features.Branches.Commands.CreateBranch;

/// <summary>
/// Handles the CreateBranchCommand.
/// Responsible for creating a new branch in the database.
/// </summary>
public class CreateBranchCommandHandler
    : IRequestHandler<CreateBranchCommand, int>
{
    #region Fields

    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes the handler with the required dependencies.
    /// </summary>
    public CreateBranchCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    #endregion

    #region Command Handler

    /// <summary>
    /// Handles the incoming CreateBranchCommand.
    /// </summary>
    /// <param name="request">
    /// Contains the branch information received from the API.
    /// </param>
    /// <param name="cancellationToken">
    /// Used to cancel the database operation if the request is aborted.
    /// </param>
    /// <returns>
    /// Returns the ID of the newly created branch.
    /// </returns>
    public async Task<int> Handle(
        CreateBranchCommand request,
        CancellationToken cancellationToken)
    {
        // Convert DTO into a Branch entity.
        var branch = _mapper.Map<Branch>(request.Branch);

        // Add the new entity to the DbContext.
        _context.Branches.Add(branch);

        // Save changes to the database.
        await _context.SaveChangesAsync(cancellationToken);

        // Return the generated primary key.
        return branch.Id;
    }

    #endregion
}