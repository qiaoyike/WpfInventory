using AutoMapper;
using WpfInventory.Application.Dtos.Stock;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Mapping;

public class StockTransactionMappingProfile : Profile
{
    public StockTransactionMappingProfile()
    {
        CreateMap<StockTransaction, StockTransactionDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : null));

        CreateMap<CreateStockTransactionRequest, StockTransaction>();
    }
}
