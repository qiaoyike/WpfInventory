using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WpfInventory.Domain.Entities;
using WpfInventory.Infrastructure.Data;
using WpfInventory.Infrastructure.Persistence;

namespace WpfInventory.Infrastructure.Tests;

/// <summary>
/// 工作单元事务测试。
/// </summary>
public class EfUnitOfWorkTests
{
    [Fact]
    public async Task ExecuteInTransactionAsync_CommitsChanges()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var dbContext = new AppDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var unitOfWork = new EfUnitOfWork(dbContext);
        await unitOfWork.ExecuteInTransactionAsync(() =>
        {
            dbContext.Categories.Add(new Category { Name = "电子", CreatedAt = DateTime.UtcNow });
            return Task.CompletedTask;
        });

        Assert.Equal(1, await dbContext.Categories.CountAsync());
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_RollsBackOnError()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var dbContext = new AppDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var unitOfWork = new EfUnitOfWork(dbContext);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            unitOfWork.ExecuteInTransactionAsync(() =>
            {
                dbContext.Categories.Add(new Category { Name = "电子", CreatedAt = DateTime.UtcNow });
                throw new InvalidOperationException("模拟失败");
            }));

        Assert.Equal(0, await dbContext.Categories.CountAsync());
    }
}