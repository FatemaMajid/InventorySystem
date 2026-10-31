
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Queries.GetAllStores;

public class GetAllStoresQueryHandler : IRequestHandler<GetAllStoresQuery, List<StoreDto>> {
    private readonly IApplicationDbContext _context;

    public GetAllStoresQueryHandler(IApplicationDbContext context){
        _context = context;
    }

    public async Task<List<StoreDto>> Handle(GetAllStoresQuery request, CancellationToken cancellationToken){
        var stores = await _context.Stores
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return stores.Select(store => new StoreDto
        {
            Id = store.Id,
            StoreCode = store.StoreCode,
            StoreNameArabic = store.StoreNameArabic,
            StoreNameEnglish = store.StoreNameEnglish,
            BranchCode = store.BranchCode,
            IsActive = store.IsActive
        }).ToList();
    }
}
