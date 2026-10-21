using FluentValidation;
using WpfInventory.Application.Abstractions.Identity;
using WpfInventory.Domain.Repositories;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Identity;
using WpfInventory.Application.Identity;

namespace WpfInventory.Application.Services;

/// <summary>
/// 认证服务实现。
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserSessionManager _sessionManager;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUserSessionManager sessionManager,
        IValidator<LoginRequest> loginValidator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _sessionManager = sessionManager;
        _loginValidator = loginValidator;
    }

    public async Task<UserDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        await _loginValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null || !user.IsActive || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        var roleCodes = user.UserRoles.Select(x => x.Role.Code).ToList();
        var permissionCodes = user.UserRoles
            .SelectMany(x => x.Role.RolePermissions)
            .Select(x => x.Permission.Code)
            .Distinct()
            .ToList();

        _sessionManager.Set(new UserSessionInfo
        {
            UserId = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Permissions = new HashSet<string>(permissionCodes)
        });

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            IsActive = user.IsActive,
            RoleCodes = roleCodes,
            PermissionCodes = permissionCodes
        };
    }

    public void Logout()
    {
        _sessionManager.Clear();
    }
}
