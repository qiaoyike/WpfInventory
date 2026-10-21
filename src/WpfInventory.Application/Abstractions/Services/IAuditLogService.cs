using WpfInventory.Application.Dtos.Audit;

namespace WpfInventory.Application.Abstractions.Services;

public interface IAuditLogService
{
    Task<IReadOnlyList<AuditLogDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
