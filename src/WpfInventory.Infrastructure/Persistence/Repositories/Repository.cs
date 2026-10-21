using Microsoft.EntityFrameworkCore;
using WpfInventory.Domain.Repositories;
using WpfInventory.Domain.Entities;
using WpfInventory.Infrastructure.Data;

namespace WpfInventory.Infrastructure.Persistence.Repositories;

/// <summary>
/// 通用仓储实现。
/// 使用 EF Core 的 DbSet 完成标准 CRUD。
/// </summary>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly AppDbContext DbContext;

    public Repository(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }
}
