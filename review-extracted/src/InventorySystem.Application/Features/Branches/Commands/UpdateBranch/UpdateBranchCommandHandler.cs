// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Branches
// File         : UpdateBranchCommandHandler.cs
// Description  : Handles branch update requests.
// Author       : Fatema Majid
// ============================================================

using System.Text.Json;
using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Branches.Commands.UpdateBranch;

/// <summary>
/// Handles the UpdateBranchCommand.
/// </summary>
public class UpdateBranchCommandHandler
    : IRequestHandler<UpdateBranchCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes the handler with the required dependencies.
    /// </summary>
    public UpdateBranchCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IAuditLogService auditLogService)
    {
        _context = context;
        _mapper = mapper;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Updates an existing branch.
    /// </summary>
    public async Task<bool> Handle(
        UpdateBranchCommand request,
        CancellationToken cancellationToken)
    {
        var branch = await _context.Branches
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (branch == null)
            return false;

        _mapper.Map(request.Branch, branch);

        await _context.SaveChangesAsync(cancellationToken);

        var details = JsonSerializer.Serialize(new
        {
            branchNameArabic = branch.BranchNameArabic,
            branchNameEnglish = branch.BranchNameEnglish,
            branchCode = branch.BranchCode
        });

        await _auditLogService.LogAsync(
            action: "Update",
            entity: "Branch",
            entityId: branch.Id.ToString(),
            details: details,
            cancellationToken: cancellationToken);

        return true;
    }
}