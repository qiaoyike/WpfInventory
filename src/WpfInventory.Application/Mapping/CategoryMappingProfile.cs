using AutoMapper;
using WpfInventory.Application.Dtos.Categories;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Mapping;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<UpdateCategoryRequest, Category>();
    }
}
