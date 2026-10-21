using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WpfInventory.Application.Abstractions.Identity;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Categories;
using WpfInventory.Application.Dtos.MasterData;
using WpfInventory.Application.Identity;
using WpfInventory.Application.Security;
using WpfInventory.Application.Services;
using WpfInventory.Application.Services.Crud;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application;

/// <summary>
/// Application 层的依赖注入扩展。
/// 这一层只负责业务用例的装配，不依赖具体 UI 或数据库实现。
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 自动注册当前程序集中的 AutoMapper Profile。
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // 自动注册当前程序集中的 FluentValidation 校验器。
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // 当前用户会话为 Singleton，整个应用共享一份。
        services.AddSingleton<CurrentUserSession>();
        services.AddSingleton<ICurrentUserSession>(sp => sp.GetRequiredService<CurrentUserSession>());
        services.AddSingleton<IUserSessionManager>(sp => sp.GetRequiredService<CurrentUserSession>());

        // 密码哈希器。
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

        // 应用服务。
        services.AddScoped<IProductAppService, ProductAppService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IRoleManagementService, RoleManagementService>();
        services.AddScoped<IStockTransactionService, StockTransactionService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IExportService, CsvExportService>();

        // 通用 CRUD 应用服务。
        // 因为 C# 开放泛型无法直接映射不同参数数量的实现类，
        // 这里为每个主数据模块显式指定实体类型，保证 DI 容器能正确解析。
        services.AddScoped<
            ICrudAppService<CategoryDto, CreateCategoryRequest, UpdateCategoryRequest>,
            CrudAppService<Category, CategoryDto, CreateCategoryRequest, UpdateCategoryRequest>>();

        services.AddScoped<
            ICrudAppService<WarehouseDto, CreateWarehouseRequest, UpdateWarehouseRequest>,
            CrudAppService<Warehouse, WarehouseDto, CreateWarehouseRequest, UpdateWarehouseRequest>>();

        services.AddScoped<
            ICrudAppService<SupplierDto, CreateSupplierRequest, UpdateSupplierRequest>,
            CrudAppService<Supplier, SupplierDto, CreateSupplierRequest, UpdateSupplierRequest>>();

        services.AddScoped<
            ICrudAppService<CustomerDto, CreateCustomerRequest, UpdateCustomerRequest>,
            CrudAppService<Customer, CustomerDto, CreateCustomerRequest, UpdateCustomerRequest>>();

        return services;
    }
}