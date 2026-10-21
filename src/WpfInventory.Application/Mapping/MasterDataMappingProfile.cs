using AutoMapper;
using WpfInventory.Application.Dtos.MasterData;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Mapping;

public class MasterDataMappingProfile : Profile
{
    public MasterDataMappingProfile()
    {
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<CreateWarehouseRequest, Warehouse>();
        CreateMap<UpdateWarehouseRequest, Warehouse>();

        CreateMap<Supplier, SupplierDto>();
        CreateMap<CreateSupplierRequest, Supplier>();
        CreateMap<UpdateSupplierRequest, Supplier>();

        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerRequest, Customer>();
        CreateMap<UpdateCustomerRequest, Customer>();
    }
}
