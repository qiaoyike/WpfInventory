using WpfInventory.Domain.Entities;

namespace WpfInventory.Domain.Repositories;

/// <summary>
/// 用户专用仓储接口。
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithRolesAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllWithRolesAsync(CancellationToken cancellationToken = default);
}