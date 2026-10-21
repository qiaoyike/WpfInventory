using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WpfInventory.Application.Security;
using WpfInventory.Infrastructure.Data;

namespace WpfInventory.Infrastructure.Tests;

/// <summary>
/// 数据库初始化器集成测试。
/// 验证迁移可以应用到 SQLite，并能正确播种权限、角色和管理员账号。
/// </summary>
public class DbInitializerTests
{
    [Fact]
    public async Task InitializeAsync_CreatesSchemaAndSeedData()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new AppDbContext(options);
        var initializer = new DbInitializer(dbContext, new Pbkdf2PasswordHasher());

        // MigrateAsync 会执行 Data/Migrations 下的全部迁移。
        await initializer.InitializeAsync();

        Assert.Equal(1, await dbContext.Users.CountAsync());
        Assert.Equal(2, await dbContext.Roles.CountAsync());
        Assert.Equal(19, await dbContext.Permissions.CountAsync());

        var admin = await dbContext.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Username == "admin");

        Assert.NotNull(admin);
        Assert.Contains(admin.UserRoles, x => x.Role.Code == "admin");
    }
}