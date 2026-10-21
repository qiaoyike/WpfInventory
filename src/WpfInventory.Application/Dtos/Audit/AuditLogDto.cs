namespace WpfInventory.Application.Dtos.Audit;

public class AuditLogDto
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
}
