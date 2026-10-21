using Microsoft.EntityFrameworkCore;
using WpfInventory.Domain.Repositories;
using WpfInventory.Domain.Entities;
using WpfInventory.Infrastructure.Data;

namespace WpfInventory.Infrastructure.Persistence.Repositories;

/// <summary>
/// 用户仓储实现。
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        // 登录时需要读取用户、角色以及角色下的权限，因此一次性 Include 到位。
        return await DbContext.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public async Task<User?> GetByIdWithRolesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllWithRolesAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Users
            .AsNoTracking()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .ToListAsync(cancellationToken);
    }
}