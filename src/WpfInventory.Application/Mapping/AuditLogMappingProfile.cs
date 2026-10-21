using AutoMapper;
using WpfInventory.Application.Dtos.Audit;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Mapping;

public class AuditLogMappingProfile : Profile
{
    public AuditLogMappingProfile()
    {
        CreateMap<AuditLog, AuditLogDto>();
    }
}
