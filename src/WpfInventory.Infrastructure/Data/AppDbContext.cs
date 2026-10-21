using Microsoft.EntityFrameworkCore;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Infrastructure.Data;

/// <summary>
/// EF Core 数据库上下文。
/// 它负责把领域实体映射到数据库表，并管理查询、保存等操作。
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// 通过依赖注入接收配置，例如 SQLite 连接字符串。
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>
    /// 模型配置入口。
    /// 自动扫描当前程序集中所有实现 IEntityTypeConfiguration 的配置类。
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
