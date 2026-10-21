using WpfInventory.Application.Abstractions.Identity;
using WpfInventory.Application.Abstractions.Persistence;
using WpfInventory.Domain.Repositories;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Services;

/// <summary>
/// 审计服务实现。
/// 把关键操作写入 AuditLogs 表。
/// </summary>
public class AuditService : IAuditService
{
    private readonly IRepository<AuditLog> _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserSession _currentUserSession;

    public AuditService(
        IRepository<AuditLog> auditLogRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserSession currentUserSession)
    {
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
        _currentUserSession = currentUserSession;
    }

    public async Task RecordAsync(string action, string entityName, int? entityId, string? message, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            UserId = _currentUserSession.UserId,
            Username = _currentUserSession.Username,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        await _auditLogRepository.AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
