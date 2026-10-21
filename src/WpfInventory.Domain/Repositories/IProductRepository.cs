using WpfInventory.Domain.Entities;

namespace WpfInventory.Domain.Repositories;

/// <summary>
/// 商品仓储接口。
/// 只定义“我需要什么能力”，不关心底层是 EF Core 还是别的数据库。
/// 实现放在 Infrastructure 层。
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// 根据主键查询商品。
    /// </summary>
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询全部商品。
    /// </summary>
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据分类查询商品。
    /// </summary>
    Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增商品。注意：这里只做跟踪，不立即保存数据库。
    /// </summary>
    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新商品。这里只做跟踪，不立即保存数据库。
    /// </summary>
    void Update(Product product);

    /// <summary>
    /// 删除商品。这里只做跟踪，不立即保存数据库。
    /// </summary>
    void Remove(Product product);
}