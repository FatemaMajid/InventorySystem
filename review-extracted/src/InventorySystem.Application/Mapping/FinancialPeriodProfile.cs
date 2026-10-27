using AutoMapper;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using InventorySystem.Domain.FinancialAnalysis.Entities;

namespace InventorySystem.Application.Mappings;

public class FinancialPeriodProfile : Profile
{
    public FinancialPeriodProfile()
    {
        CreateMap<FinancialPeriod, FinancialPeriodDto>();
    }
}
