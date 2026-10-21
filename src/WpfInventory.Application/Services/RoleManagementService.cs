using FluentValidation;
using WpfInventory.Application.Abstractions.Persistence;
using WpfInventory.Domain.Repositories;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Identity;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Services;

/// <summary>
/// 角色管理服务。
/// </summary>
public class RoleManagementService : IRoleManagementService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateRoleRequest> _createValidator;
    private readonly IValidator<UpdateRoleRequest> _updateValidator;

    public RoleManagementService(
        IRoleRepository roleRepository,
        IRepository<Permission> permissionRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateRoleRequest> createValidator,
        IValidator<UpdateRoleRequest> updateValidator)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllWithPermissionsAsync(cancellationToken);
        return roles.Select(Map).ToList();
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(id, cancellationToken);
        return role is null ? null : Map(role);
    }

    public async Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        return permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description
        }).ToList();
    }

    public async Task CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var role = new Role
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            RolePermissions = request.PermissionIds
                .Distinct()
                .Select(permissionId => new RolePermission { PermissionId = permissionId })
                .ToList()
        };

        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var role = await _roleRepository.GetByIdWithPermissionsAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"未找到 Id 为 {request.Id} 的角色");

        role.Code = request.Code;
        role.Name = request.Name;
        role.Description = request.Description;
        role.UpdatedAt = DateTime.UtcNow;
        role.RolePermissions = request.PermissionIds
            .Distinct()
            .Select(permissionId => new RolePermission { PermissionId = permissionId, RoleId = role.Id })
            .ToList();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"未找到 Id 为 {id} 的角色");

        _roleRepository.Remove(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static RoleDto Map(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Code = role.Code,
            Name = role.Name,
            Description = role.Description,
            PermissionIds = role.RolePermissions.Select(x => x.PermissionId).ToList()
        };
    }
}
