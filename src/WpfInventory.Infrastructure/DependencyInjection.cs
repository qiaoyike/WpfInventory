using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WpfInventory.Application.Abstractions.Persistence;
using WpfInventory.Domain.Repositories;
using WpfInventory.Infrastructure.Data;
using WpfInventory.Infrastructure.Persistence;
using WpfInventory.Infrastructure.Persistence.Repositories;

namespace WpfInventory.Infrastructure;

/// <summary>
/// Infrastructure 层的依赖注入扩展。
/// 这一层负责数据库、仓储、工作单元等基础设施组件的装配。
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("未找到连接字符串 ConnectionStrings:Default。");

        // 注册 EF Core DbContext，并指定使用 SQLite。
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        // 通用仓储：任何 BaseEntity 都可以解析到 Repository<TEntity>。
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // 专用仓储：有额外查询需求的实体单独注册。
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();

        // 工作单元：统一提交与事务控制。
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // 数据库初始化器：启动时执行迁移并播种种子数据。
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}