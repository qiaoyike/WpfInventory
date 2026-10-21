using AutoMapper;
using WpfInventory.Application.Dtos.Products;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Mapping;

/// <summary>
/// 商品相关的对象映射配置。
/// 实体和 DTO 的转换都集中在这里，避免到处手写赋值。
/// </summary>
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        CreateMap<CreateProductRequest, Product>();

        CreateMap<UpdateProductRequest, Product>();
    }
}
