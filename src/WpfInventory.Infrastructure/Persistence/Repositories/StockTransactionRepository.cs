using Microsoft.EntityFrameworkCore;
using WpfInventory.Domain.Repositories;
using WpfInventory.Domain.Entities;
using WpfInventory.Infrastructure.Data;

namespace WpfInventory.Infrastructure.Persistence.Repositories;

/// <summary>
/// 库存流水仓储实现。
/// </summary>
public class StockTransactionRepository : Repository<StockTransaction>, IStockTransactionRepository
{
    public StockTransactionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<StockTransaction>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.StockTransactions
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockTransaction>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await DbContext.StockTransactions
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
