using FluentValidation;
using WpfInventory.Application.Abstractions.Identity;
using WpfInventory.Application.Abstractions.Persistence;
using WpfInventory.Domain.Repositories;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Identity;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Services;

/// <summary>
/// 用户管理服务。
/// </summary>
public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserSession _currentUserSession;
    private readonly IValidator<CreateUserRequest> _createValidator;
    private readonly IValidator<UpdateUserRequest> _updateValidator;

    public UserManagementService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ICurrentUserSession currentUserSession,
        IValidator<CreateUserRequest> createValidator,
        IValidator<UpdateUserRequest> updateValidator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _currentUserSession = currentUserSession;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllWithRolesAsync(cancellationToken);
        return users.Select(Map).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(id, cancellationToken);
        return user is null ? null : Map(user);
    }

    public async Task CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var existing = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("用户名已存在");
        }

        var user = new User
        {
            Username = request.Username,
            DisplayName = request.DisplayName,
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UserRoles = request.RoleIds.Select(roleId => new UserRole { RoleId = roleId }).ToList()
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await _userRepository.GetByIdWithRolesAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"未找到 Id 为 {request.Id} 的用户");

        user.DisplayName = request.DisplayName;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        user.UserRoles = request.RoleIds.Select(roleId => new UserRole { RoleId = roleId, UserId = user.Id }).ToList();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (_currentUserSession.UserId == id)
        {
            throw new InvalidOperationException("不能删除当前登录用户");
        }

        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"未找到 Id 为 {id} 的用户");

        _userRepository.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static UserDto Map(User user)
    {
        var roles = user.UserRoles.Select(x => x.Role).ToList();
        var permissionCodes = roles
            .SelectMany(x => x.RolePermissions)
            .Select(x => x.Permission.Code)
            .Distinct()
            .ToList();

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            IsActive = user.IsActive,
            RoleCodes = roles.Select(x => x.Code).ToList(),
            PermissionCodes = permissionCodes
        };
    }
}
