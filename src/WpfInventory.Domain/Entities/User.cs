namespace WpfInventory.Domain.Entities;

/// <summary>
/// 系统用户实体。
/// </summary>
public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码哈希，绝不保存明文密码。
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 用户和角色的多对多关系。
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
