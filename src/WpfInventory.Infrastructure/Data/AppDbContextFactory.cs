using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WpfInventory.Infrastructure.Data;

/// <summary>
/// 设计时 DbContext 工厂。
/// 当使用 dotnet ef 命令生成迁移时，EF 工具会通过它创建 AppDbContext。
/// 普通应用运行时不会使用这个类。
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // 这里先使用固定连接串；后续会改为从 appsettings.json 读取。
        optionsBuilder.UseSqlite("Data Source=wpf-inventory.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}
