using Microsoft.EntityFrameworkCore;
using WpfInventory.Application.Abstractions.Persistence;
using WpfInventory.Infrastructure.Data;

namespace WpfInventory.Infrastructure.Persistence;

/// <summary>
/// EF Core 版工作单元。
/// AppDbContext 本身就像一个工作单元，这里把它明确封装成接口，方便业务层使用。
/// </summary>
public class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public EfUnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        // 开启一个数据库事务。
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await action();

            // 在事务内统一提交所有已跟踪的变更，确保原子性。
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            // 出现异常时回滚，保持数据一致性。
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
