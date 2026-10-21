using AutoMapper;
using WpfInventory.Domain.Repositories;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Audit;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Services;

/// <summary>
/// 审计日志查询服务。
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly IRepository<AuditLog> _auditLogRepository;
    private readonly IMapper _mapper;

    public AuditLogService(IRepository<AuditLog> auditLogRepository, IMapper mapper)
    {
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var logs = await _auditLogRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<AuditLogDto>>(logs);
    }
}
