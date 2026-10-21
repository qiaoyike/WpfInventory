namespace WpfInventory.Domain.Entities;

/// <summary>
/// 用户角色关联实体。
/// </summary>
public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
