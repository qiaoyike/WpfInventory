using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WpfInventory.Domain.Entities;
using WpfInventory.Infrastructure.Data;
using WpfInventory.Infrastructure.Persistence.Repositories;

namespace WpfInventory.Infrastructure.Tests;

/// <summary>
/// 仓储层集成测试。
/// 使用 SQLite 内存数据库验证 EF Core 映射和 Include 查询。
/// </summary>
public class RepositoryTests
{
    [Fact]
    public async Task ProductRepository_GetAllAsync_IncludesCategory()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new AppDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var category = new Category { Name = "电子", CreatedAt = DateTime.UtcNow };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var product = new Product
        {
            Name = "手机",
            Sku = "SKU-1001",
            UnitPrice = 1000,
            StockQuantity = 5,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow
        };
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var repository = new ProductRepository(dbContext);
        var items = await repository.GetAllAsync();

        var item = Assert.Single(items);
        Assert.Equal("电子", item.Category?.Name);
    }

    [Fact]
    public async Task UserRepository_GetByUsernameAsync_LoadsRolesAndPermissions()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new AppDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var permission = new Permission { Code = "product.view", Name = "商品查看", CreatedAt = DateTime.UtcNow };
        var role = new Role
        {
            Code = "admin",
            Name = "管理员",
            CreatedAt = DateTime.UtcNow,
            RolePermissions = new List<RolePermission>
            {
                new RolePermission { Permission = permission }
            }
        };
        var user = new User
        {
            Username = "admin",
            DisplayName = "系统管理员",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new UserRole { Role = role }
            }
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var repository = new UserRepository(dbContext);
        var found = await repository.GetByUsernameAsync("admin");

        Assert.NotNull(found);
        var permissionCodes = found.UserRoles
            .SelectMany(x => x.Role.RolePermissions)
            .Select(x => x.Permission.Code)
            .ToList();

        Assert.Contains("product.view", permissionCodes);
    }
}