using Microsoft.EntityFrameworkCore;
using WpfInventory.Application.Abstractions.Identity;
using WpfInventory.Domain.Authorization;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Infrastructure.Data;

/// <summary>
/// 数据库初始化器。
/// 首次启动时执行迁移，并写入默认角色、权限和管理员账号。
/// </summary>
public class DbInitializer : IDbInitializer
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public DbInitializer(AppDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // 自动应用所有迁移，确保数据库表结构最新。
        await _dbContext.Database.MigrateAsync(cancellationToken);

        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var permissions = CreatePermissions();
        _dbContext.Permissions.AddRange(permissions);

        var adminRole = new Role
        {
            Code = "admin",
            Name = "系统管理员",
            Description = "拥有全部权限",
            CreatedAt = DateTime.UtcNow,
            RolePermissions = permissions.Select(p => new RolePermission { Permission = p }).ToList()
        };

        var operatorRole = new Role
        {
            Code = "operator",
            Name = "仓库操作员",
            Description = "负责商品、库存和基础资料的日常操作",
            CreatedAt = DateTime.UtcNow,
            RolePermissions = permissions
                .Where(p => p.Code is AppPermissions.ProductView or AppPermissions.ProductEdit
                    or AppPermissions.CategoryView or AppPermissions.CategoryEdit
                    or AppPermissions.StockTransactionView or AppPermissions.StockTransactionEdit
                    or AppPermissions.CustomerView or AppPermissions.SupplierView
                    or AppPermissions.WarehouseView)
                .Select(p => new RolePermission { Permission = p })
                .ToList()
        };

        _dbContext.Roles.AddRange(adminRole, operatorRole);

        var adminUser = new User
        {
            Username = "admin",
            DisplayName = "系统管理员",
            PasswordHash = _passwordHasher.Hash("Admin@123456"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new UserRole { Role = adminRole }
            }
        };

        _dbContext.Users.Add(adminUser);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static List<Permission> CreatePermissions()
    {
        var codes = new[]
        {
            AppPermissions.DashboardView,
            AppPermissions.ProductView,
            AppPermissions.ProductEdit,
            AppPermissions.CategoryView,
            AppPermissions.CategoryEdit,
            AppPermissions.CustomerView,
            AppPermissions.CustomerEdit,
            AppPermissions.SupplierView,
            AppPermissions.SupplierEdit,
            AppPermissions.WarehouseView,
            AppPermissions.WarehouseEdit,
            AppPermissions.StockTransactionView,
            AppPermissions.StockTransactionEdit,
            AppPermissions.UserView,
            AppPermissions.UserEdit,
            AppPermissions.RoleView,
            AppPermissions.RoleEdit,
            AppPermissions.AuditLogView,
            AppPermissions.ReportView
        };

        return codes.Select(code => new Permission
        {
            Code = code,
            Name = code,
            CreatedAt = DateTime.UtcNow
        }).ToList();
    }
}
