using System.Text.Json;
using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Commands.CreateStore;

public class CreateStoreCommandHandler
    : IRequestHandler<CreateStoreCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLogService;

    public CreateStoreCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IAuditLogService auditLogService)
    {
        _context = context;
        _mapper = mapper;
        _auditLogService = auditLogService;
    }

    public async Task<int> Handle(
        CreateStoreCommand request,
        CancellationToken cancellationToken)
    {
        var branchExists = await _context.Branches
            .AnyAsync(
                x => x.BranchCode == request.Store.BranchCode,
                cancellationToken);

        if (!branchExists)
            throw new KeyNotFoundException("Branch not found.");

        var store = _mapper.Map<Store>(request.Store);

        _context.Stores.Add(store);

        await _context.SaveChangesAsync(cancellationToken);

        var details = JsonSerializer.Serialize(new
        {
            storeNameArabic = store.StoreNameArabic,
            storeNameEnglish = store.StoreNameEnglish,
            storeCode = store.StoreCode,
            branchCode = store.BranchCode
        });

        await _auditLogService.LogAsync(
            action: "Create",
            entity: "Store",
            entityId: store.Id.ToString(),
            details: details,
            cancellationToken: cancellationToken);

        return store.Id;
    }
}