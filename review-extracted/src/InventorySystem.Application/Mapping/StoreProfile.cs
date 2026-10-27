using AutoMapper;
using InventorySystem.Application.DTOs.Store;
using InventorySystem.Domain.Entities;

namespace InventorySystem.Application.Mapping;

public class StoreProfile : Profile
{
    public StoreProfile()
    {
        // Create
        CreateMap<CreateStoreDto, Store>();

        // Update
        CreateMap<UpdateStoreDto, Store>();

        // Read
        CreateMap<Store, StoreDto>();
    }
}