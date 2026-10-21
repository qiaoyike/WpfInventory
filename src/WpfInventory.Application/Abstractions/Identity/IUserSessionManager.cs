using WpfInventory.Application.Identity;

namespace WpfInventory.Application.Abstractions.Identity;

/// <summary>
/// 用户会话管理器接口。
/// 只暴露给认证流程使用，用于写入或清空会话。
/// </summary>
public interface IUserSessionManager
{
    void Set(UserSessionInfo info);
    void Clear();
}
