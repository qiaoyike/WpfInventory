using WpfInventory.Domain.Entities;

namespace WpfInventory.Domain.Repositories;

/// <summary>
/// 分类仓储接口。
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// 根据主键查询分类。
    /// </summary>
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询全部分类。
    /// </summary>
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增分类。
    /// </summary>
    Task AddAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新分类。
    /// </summary>
    void Update(Category category);

    /// <summary>
    /// 删除分类。
    /// </summary>
    void Remove(Category category);
}