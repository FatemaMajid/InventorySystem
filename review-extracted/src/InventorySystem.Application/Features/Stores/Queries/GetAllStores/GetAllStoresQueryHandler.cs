using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Queries.GetAllStores;

/// <summary>
/// Handles the request to retrieve all stores.
/// </summary>
public class GetAllStoresQueryHandler
    : IRequestHandler<GetAllStoresQuery, List<StoreDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllStoresQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<StoreDto>> Handle(
        GetAllStoresQuery request,
        CancellationToken cancellationToken)
    {
        var stores = await _context.Stores
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<StoreDto>>(stores);
    }
}