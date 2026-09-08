using System.Text.Json;
using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Commands.UpdateStore;

public class UpdateStoreCommandHandler
    : IRequestHandler<UpdateStoreCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLogService;

    public UpdateStoreCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IAuditLogService auditLogService)
    {
        _context = context;
        _mapper = mapper;
        _auditLogService = auditLogService;
    }

    public async Task<bool> Handle(
        UpdateStoreCommand request,
        CancellationToken cancellationToken)
    {
        var store = await _context.Stores
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (store == null)
            return false;

        var branchExists = await _context.Branches
            .AnyAsync(
                x => x.BranchCode == request.Store.BranchCode,
                cancellationToken);

        if (!branchExists)
            throw new KeyNotFoundException("Branch not found.");

        _mapper.Map(request.Store, store);

        await _context.SaveChangesAsync(cancellationToken);

        var details = JsonSerializer.Serialize(new
        {
            storeNameArabic = store.StoreNameArabic,
            storeNameEnglish = store.StoreNameEnglish,
            storeCode = store.StoreCode,
            branchCode = store.BranchCode
        });

        await _auditLogService.LogAsync(
            action: "Update",
            entity: "Store",
            entityId: store.Id.ToString(),
            details: details,
            cancellationToken: cancellationToken);

        return true;
    }
}