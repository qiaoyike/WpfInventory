using System.Windows;
using WpfInventory.App.ViewModels;

namespace WpfInventory.App.Views;

/// <summary>
/// 主窗口外壳：左侧导航 + 右侧内容区。
/// 页面切换由 MainWindowViewModel 通过 DI 作用域完成。
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = viewModel;

        // 用户点击退出登录后，协调窗口切换回登录页。
        _viewModel.LogoutRequested += OnLogoutRequested;
    }

    private void OnLogoutRequested()
    {
        var app = (App)System.Windows.Application.Current;
        app.ShowLoginWindow();
        Close();
    }
}