namespace WpfInventory.Application.Abstractions.Services;

/// <summary>
/// 审计服务接口。
/// </summary>
public interface IAuditService
{
    Task RecordAsync(string action, string entityName, int? entityId, string? message, CancellationToken cancellationToken = default);
}
