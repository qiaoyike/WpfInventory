using WpfInventory.Domain.Entities;

namespace WpfInventory.Domain.Repositories;

/// <summary>
/// 角色专用仓储接口。
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByIdWithPermissionsAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync(CancellationToken cancellationToken = default);
}