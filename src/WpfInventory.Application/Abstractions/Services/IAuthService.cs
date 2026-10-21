using WpfInventory.Application.Dtos.Identity;

namespace WpfInventory.Application.Abstractions.Services;

/// <summary>
/// 认证服务接口。
/// </summary>
public interface IAuthService
{
    Task<UserDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    void Logout();
}
