using WpfInventory.Application.Dtos.Identity;

namespace WpfInventory.Application.Abstractions.Services;

public interface IRoleManagementService
{
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);
    Task CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
