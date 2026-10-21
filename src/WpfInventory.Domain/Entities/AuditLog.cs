namespace WpfInventory.Domain.Entities;

/// <summary>
/// 审计日志实体。
/// 记录关键操作，方便后续追踪。
/// </summary>
public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Message { get; set; }
}
