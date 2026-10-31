
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Queries.GetStoreById;

public class GetStoreByIdQueryHandler : IRequestHandler<GetStoreByIdQuery, StoreDto> {
    private readonly IApplicationDbContext _context;

    public GetStoreByIdQueryHandler(IApplicationDbContext context){
        _context = context;
    }

    public async Task<StoreDto> Handle(GetStoreByIdQuery request, CancellationToken cancellationToken){
        var store = await _context.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (store == null)
            return null!;

        return new StoreDto
        {
            Id = store.Id,
            StoreCode = store.StoreCode,
            StoreNameArabic = store.StoreNameArabic,
            StoreNameEnglish = store.StoreNameEnglish,
            BranchCode = store.BranchCode,
            IsActive = store.IsActive
        };
    }
}
