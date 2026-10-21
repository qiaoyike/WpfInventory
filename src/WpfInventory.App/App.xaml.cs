using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WpfInventory.App.Services;
using WpfInventory.App.ViewModels;
using WpfInventory.App.Views;
using WpfInventory.Application;
using WpfInventory.Infrastructure;
using WpfInventory.Infrastructure.Data;

namespace WpfInventory.App;

/// <summary>
/// WPF 应用入口。
/// 这里只负责启动流程和依赖装配，不写具体业务逻辑。
/// </summary>
public partial class App : System.Windows.Application
{
    private IHost? _host;
    private IServiceScope? _loginScope;
    private IServiceScope? _mainScope;
    private LoginWindow? _loginWindow;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 全局异常兜底：未捕获的 UI 线程异常统一记录并提示。
        DispatcherUnhandledException += OnDispatcherUnhandledException;

        // 先初始化日志系统，方便后续启动异常也能写入日志。
        ConfigureSerilog();

        // Generic Host：统一管理配置、日志和依赖注入。
        _host = Host.CreateDefaultBuilder(e.Args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                // Composition Root：所有依赖都在这里集中注册。
                ConfigureUiServices(services);

                // 注册应用层业务服务。
                services.AddApplication();

                // 注册 EF Core、仓储和工作单元。
                services.AddInfrastructure(context.Configuration);
            })
            .Build();

        _host.Start();

        try
        {
            // 数据库初始化放在独立作用域中，完成后即可释放。
            using var scope = _host.Services.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await initializer.InitializeAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "数据库初始化失败");
            MessageBox.Show("数据库初始化失败：" + ex.Message, "启动错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        ShowLoginWindow();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _loginScope?.Dispose();
        _mainScope?.Dispose();
        _host?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    /// <summary>
    /// 注册表现层自己的服务和 ViewModel。
    /// 页面 ViewModel 注册为 Scoped，导航时由主窗口按作用域解析，
    /// 从而获得独立的 DbContext / 仓储生命周期。
    /// </summary>
    private static void ConfigureUiServices(IServiceCollection services)
    {
        services.AddSingleton<IDialogService, DialogService>();

        services.AddScoped<MainWindow>();
        services.AddScoped<MainWindowViewModel>();

        services.AddScoped<LoginWindow>();
        services.AddScoped<LoginViewModel>();

        services.AddScoped<DashboardViewModel>();
        services.AddScoped<ProductViewModel>();
        services.AddScoped<CategoryViewModel>();
        services.AddScoped<WarehouseViewModel>();
        services.AddScoped<SupplierViewModel>();
        services.AddScoped<CustomerViewModel>();
        services.AddScoped<StockTransactionViewModel>();
        services.AddScoped<UserViewModel>();
        services.AddScoped<RoleViewModel>();
        services.AddScoped<AuditLogViewModel>();
        services.AddScoped<ReportViewModel>();
    }

    /// <summary>
    /// 显示登录窗口。
    /// 登录窗口及其依赖放在独立作用域中，登录完成后统一释放。
    /// </summary>
    internal void ShowLoginWindow()
    {
        _mainScope?.Dispose();
        _mainScope = null;
        _loginScope = _host!.Services.CreateScope();

        var loginViewModel = _loginScope.ServiceProvider.GetRequiredService<LoginViewModel>();
        _loginWindow = _loginScope.ServiceProvider.GetRequiredService<LoginWindow>();

        loginViewModel.LoginSucceeded += OnLoginSucceeded;
        _loginWindow.Show();
    }

    /// <summary>
    /// 登录成功后切换到主窗口。
    /// </summary>
    private void OnLoginSucceeded()
    {
        // 先创建并显示主窗口，再关闭登录窗口。
        // 否则在 WPF 默认的 OnLastWindowClose 模式下，
        // 登录窗口一关闭、主窗口还没显示时会触发整个应用退出。
        _mainScope = _host!.Services.CreateScope();
        var mainWindow = _mainScope.ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        _loginWindow?.Close();
        _loginScope?.Dispose();
        _loginScope = null;
        _loginWindow = null;
    }

    /// <summary>
    /// 处理未捕获的 UI 线程异常。
    /// </summary>
    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error(e.Exception, "未处理的界面线程异常");
        MessageBox.Show("发生未处理异常：" + e.Exception.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private static void ConfigureSerilog()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // 日志实现由 Serilog 提供，业务代码仍然使用 ILogger<T> 抽象。
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
}