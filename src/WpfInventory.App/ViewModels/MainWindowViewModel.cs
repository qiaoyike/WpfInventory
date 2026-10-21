using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WpfInventory.Application.Abstractions.Identity;
using WpfInventory.Domain.Authorization;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 主窗口 ViewModel。
/// 它相当于应用的外壳：管理左侧导航菜单和右侧当前页面。
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICurrentUserSession _session;
    private readonly IUserSessionManager _sessionManager;
    private readonly ILogger<MainWindowViewModel> _logger;
    private IServiceScope? _currentScope;

    public MainWindowViewModel(
        IServiceScopeFactory scopeFactory,
        ICurrentUserSession session,
        IUserSessionManager sessionManager,
        ILogger<MainWindowViewModel> logger)
    {
        _scopeFactory = scopeFactory;
        _session = session;
        _sessionManager = sessionManager;
        _logger = logger;

        // 根据当前登录用户是否拥有权限，动态生成菜单。
        BuildNavigationItems();

        // 默认显示仪表盘。
        SelectedItem = NavigationItems.FirstOrDefault();
    }

    /// <summary>
    /// 左侧菜单集合。
    /// </summary>
    public ObservableCollection<NavigationItemViewModel> NavigationItems { get; } = new();

    /// <summary>
    /// 当前选中的菜单项。
    /// </summary>
    [ObservableProperty]
    private NavigationItemViewModel? selectedItem;

    /// <summary>
    /// 右侧内容区当前显示的页面 ViewModel。
    /// </summary>
    [ObservableProperty]
    private object? currentViewModel;

    /// <summary>
    /// 当前登录用户显示名。
    /// </summary>
    public string CurrentUserDisplayName => _session.DisplayName ?? "未登录";

    /// <summary>
    /// 用户点击退出登录后触发，由 MainWindow 协调窗口切换。
    /// </summary>
    public event Action? LogoutRequested;

    /// <summary>
    /// 菜单选中变化时切换页面。
    /// 这个方法是 CommunityToolkit 根据 SelectedItem 自动生成的回调。
    /// </summary>
    partial void OnSelectedItemChanged(NavigationItemViewModel? value)
    {
        if (value is null)
        {
            return;
        }

        Navigate(value.ViewModelType);
    }

    /// <summary>
    /// 退出登录：清除会话并通知 View 返回登录窗口。
    /// </summary>
    [RelayCommand]
    private void Logout()
    {
        _sessionManager.Clear();
        _currentScope?.Dispose();
        _currentScope = null;
        CurrentViewModel = null;
        _logger.LogInformation("用户已退出登录");
        LogoutRequested?.Invoke();
    }

    /// <summary>
    /// 根据权限生成导航菜单。
    /// 只有拥有对应 view 权限的菜单才会显示。
    /// </summary>
    private void BuildNavigationItems()
    {
        NavigationItems.Clear();
        AddIfAllowed("📊", "仪表盘", AppPermissions.DashboardView, typeof(DashboardViewModel));
        AddIfAllowed("📦", "商品管理", AppPermissions.ProductView, typeof(ProductViewModel));
        AddIfAllowed("🏷️", "分类管理", AppPermissions.CategoryView, typeof(CategoryViewModel));
        AddIfAllowed("🏬", "仓库管理", AppPermissions.WarehouseView, typeof(WarehouseViewModel));
        AddIfAllowed("🚚", "供应商管理", AppPermissions.SupplierView, typeof(SupplierViewModel));
        AddIfAllowed("👥", "客户管理", AppPermissions.CustomerView, typeof(CustomerViewModel));
        AddIfAllowed("🔄", "库存流水", AppPermissions.StockTransactionView, typeof(StockTransactionViewModel));
        AddIfAllowed("📈", "报表中心", AppPermissions.ReportView, typeof(ReportViewModel));
        AddIfAllowed("👤", "用户管理", AppPermissions.UserView, typeof(UserViewModel));
        AddIfAllowed("🛡️", "角色管理", AppPermissions.RoleView, typeof(RoleViewModel));
        AddIfAllowed("🧾", "审计日志", AppPermissions.AuditLogView, typeof(AuditLogViewModel));
    }

    private void AddIfAllowed(string icon, string title, string permission, Type viewModelType)
    {
        // admin 角色拥有全部权限，普通角色按权限常量过滤。
        if (_session.HasPermission(permission))
        {
            NavigationItems.Add(new NavigationItemViewModel(title, icon, viewModelType));
        }
    }

    /// <summary>
    /// 创建新的 DI 作用域来解析页面 ViewModel。
    /// 这样每个页面拥有独立的 DbContext/仓储/服务生命周期，
    /// 切换页面时旧的资源会被释放，避免长时间占用数据库连接。
    /// </summary>
    private void Navigate(Type viewModelType)
    {
        _currentScope?.Dispose();
        _currentScope = _scopeFactory.CreateScope();

        // 从新作用域解析页面 ViewModel，页面内所有 Scoped 依赖共享该作用域。
        CurrentViewModel = _currentScope.ServiceProvider.GetRequiredService(viewModelType);

        // 页面打开后自动触发 LoadCommand，避免用户必须手动点“刷新”。
        // 所有页面 ViewModel 都遵循 LoadCommand 命名约定，这里用反射统一调用。
        var loadCommandProperty = CurrentViewModel?.GetType().GetProperty("LoadCommand");
        if (loadCommandProperty?.GetValue(CurrentViewModel) is System.Windows.Input.ICommand command
            && command.CanExecute(null))
        {
            command.Execute(null);
        }
    }
}