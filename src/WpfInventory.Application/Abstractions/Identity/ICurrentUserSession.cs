namespace WpfInventory.Application.Abstractions.Identity;

/// <summary>
/// 当前登录用户会话。
/// </summary>
public interface ICurrentUserSession
{
    int? UserId { get; }
    string? Username { get; }
    string? DisplayName { get; }
    IReadOnlySet<string> Permissions { get; }
    bool IsAuthenticated { get; }

    /// <summary>
    /// 判断当前用户是否拥有某个权限。
    /// </summary>
    bool HasPermission(string permission);
}
