using WpfInventory.Domain.Entities;

namespace WpfInventory.Domain.Repositories;

/// <summary>
/// 库存流水专用仓储接口。
/// </summary>
public interface IStockTransactionRepository : IRepository<StockTransaction>
{
    Task<IReadOnlyList<StockTransaction>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockTransaction>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
}