namespace WpfInventory.Application.Identity;

/// <summary>
/// 登录成功后的会话信息。
/// </summary>
public class UserSessionInfo
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public IReadOnlySet<string> Permissions { get; set; } = new HashSet<string>();
}
