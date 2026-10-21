using WpfInventory.Application.Abstractions.Identity;

namespace WpfInventory.Application.Identity;

/// <summary>
/// 当前用户会话实现。
/// 注册为 Singleton，登录后写入，退出时清空。
/// </summary>
public class CurrentUserSession : ICurrentUserSession, IUserSessionManager
{
    private readonly HashSet<string> _permissions = new();

    public int? UserId { get; private set; }
    public string? Username { get; private set; }
    public string? DisplayName { get; private set; }
    public IReadOnlySet<string> Permissions => _permissions;
    public bool IsAuthenticated => UserId.HasValue;

    public bool HasPermission(string permission)
    {
        return IsAuthenticated && _permissions.Contains(permission);
    }

    public void Set(UserSessionInfo info)
    {
        UserId = info.UserId;
        Username = info.Username;
        DisplayName = info.DisplayName;

        _permissions.Clear();
        foreach (var permission in info.Permissions)
        {
            _permissions.Add(permission);
        }
    }

    public void Clear()
    {
        UserId = null;
        Username = null;
        DisplayName = null;
        _permissions.Clear();
    }
}
