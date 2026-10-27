using System.Text.Json;
using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Commands.DeleteStore;

public class DeleteStoreCommandHandler
    : IRequestHandler<DeleteStoreCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public DeleteStoreCommandHandler(
        IApplicationDbContext context,
        IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    public async Task<bool> Handle(
        DeleteStoreCommand request,
        CancellationToken cancellationToken)
    {
        var store = await _context.Stores
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (store == null)
            return false;

        var details = JsonSerializer.Serialize(new
        {
            storeNameArabic = store.StoreNameArabic,
            storeNameEnglish = store.StoreNameEnglish,
            storeCode = store.StoreCode,
            branchCode = store.BranchCode
        });

        _context.Stores.Remove(store);

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(
            action: "Delete",
            entity: "Store",
            entityId: request.Id.ToString(),
            details: details,
            cancellationToken: cancellationToken);

        return true;
    }
}