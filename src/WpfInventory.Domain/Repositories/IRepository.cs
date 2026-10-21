using WpfInventory.Domain.Entities;

namespace WpfInventory.Domain.Repositories;

/// <summary>
/// 通用仓储接口。
/// 这是领域层定义的数据访问“端口”，标准 CRUD 场景统一使用它。
/// 具体实现放在 Infrastructure 层（例如 EF Core）。
/// </summary>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}