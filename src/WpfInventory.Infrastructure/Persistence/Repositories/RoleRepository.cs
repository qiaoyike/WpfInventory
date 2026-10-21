using Microsoft.EntityFrameworkCore;
using WpfInventory.Domain.Repositories;
using WpfInventory.Domain.Entities;
using WpfInventory.Infrastructure.Data;

namespace WpfInventory.Infrastructure.Persistence.Repositories;

/// <summary>
/// 角色仓储实现。
/// </summary>
public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Role?> GetByIdWithPermissionsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Roles
            .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Roles
            .AsNoTracking()
            .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .ToListAsync(cancellationToken);
    }
}
