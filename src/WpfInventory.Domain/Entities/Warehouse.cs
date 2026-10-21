namespace WpfInventory.Domain.Entities;

/// <summary>
/// 仓库实体。
/// </summary>
public class Warehouse : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}
