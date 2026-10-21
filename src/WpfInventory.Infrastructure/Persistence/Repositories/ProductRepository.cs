using Microsoft.EntityFrameworkCore;
using WpfInventory.Domain.Repositories;
using WpfInventory.Domain.Entities;
using WpfInventory.Infrastructure.Data;

namespace WpfInventory.Infrastructure.Persistence.Repositories;

/// <summary>
/// 商品仓储实现。
/// 使用 EF Core 的 AppDbContext 完成实际数据库访问。
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        _dbContext.Products.Update(product);
    }

    public void Remove(Product product)
    {
        _dbContext.Products.Remove(product);
    }
}
