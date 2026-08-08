using AutoMapper;
using InventorySystem.Application.DTOs.Branch;
using InventorySystem.Domain.Entities;

namespace InventorySystem.Application.Mappings;

public class BranchProfile : Profile
{
    public BranchProfile()
    {
        CreateMap<Branch, BranchDto>();

        CreateMap<CreateBranchDto, Branch>();

        CreateMap<UpdateBranchDto, Branch>();

        CreateMap<Branch, UpdateBranchDto>();
    }
}
