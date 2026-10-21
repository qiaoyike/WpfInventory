namespace WpfInventory.Domain.Entities;

/// <summary>
/// 角色权限关联实体。
/// </summary>
public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
