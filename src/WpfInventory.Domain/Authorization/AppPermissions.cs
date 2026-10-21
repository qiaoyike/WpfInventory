namespace WpfInventory.Domain.Authorization;

/// <summary>
/// 系统权限代码常量。
/// 菜单和操作按钮都可以用它来判断权限。
/// </summary>
public static class AppPermissions
{
    public const string DashboardView = "dashboard.view";

    public const string ProductView = "product.view";
    public const string ProductEdit = "product.edit";

    public const string CategoryView = "category.view";
    public const string CategoryEdit = "category.edit";

    public const string CustomerView = "customer.view";
    public const string CustomerEdit = "customer.edit";

    public const string SupplierView = "supplier.view";
    public const string SupplierEdit = "supplier.edit";

    public const string WarehouseView = "warehouse.view";
    public const string WarehouseEdit = "warehouse.edit";

    public const string StockTransactionView = "stock.view";
    public const string StockTransactionEdit = "stock.edit";

    public const string UserView = "user.view";
    public const string UserEdit = "user.edit";

    public const string RoleView = "role.view";
    public const string RoleEdit = "role.edit";

    public const string AuditLogView = "audit.view";

    public const string ReportView = "report.view";
}
