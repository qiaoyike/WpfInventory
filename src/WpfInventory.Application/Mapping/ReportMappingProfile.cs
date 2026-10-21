using AutoMapper;
using WpfInventory.Application.Dtos.Reports;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Mapping;

public class ReportMappingProfile : Profile
{
    public ReportMappingProfile()
    {
        CreateMap<Product, ProductStockReportDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        CreateMap<StockTransaction, StockTransactionReportDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : null))
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType.ToString()));
    }
}
