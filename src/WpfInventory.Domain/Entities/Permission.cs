namespace WpfInventory.Domain.Entities;

/// <summary>
/// 权限实体。
/// 权限通常对应一个具体的操作，例如 product.view、product.edit。
/// </summary>
public class Permission : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
