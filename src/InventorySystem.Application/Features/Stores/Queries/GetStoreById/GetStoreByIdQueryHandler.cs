using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Stores.Queries.GetStoreById;

/// <summary>
/// Handles requests to retrieve a store by its ID.
/// </summary>
public class GetStoreByIdQueryHandler
    : IRequestHandler<GetStoreByIdQuery, StoreDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStoreByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<StoreDto> Handle(
        GetStoreByIdQuery request,
        CancellationToken cancellationToken)
    {
        // Retrieve the store without tracking because this is a read-only operation.
        var store = await _context.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        // Return null when the requested store does not exist.
        if (store == null)
            return null!;

        // Map the entity to the DTO.
        return _mapper.Map<StoreDto>(store);
    }
}