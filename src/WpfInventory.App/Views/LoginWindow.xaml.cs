using System.Windows;
using System.Windows.Controls;
using WpfInventory.App.ViewModels;

namespace WpfInventory.App.Views;

/// <summary>
/// 登录窗口。只负责显示和收集输入，登录逻辑在 ViewModel 中。
/// </summary>
public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = viewModel;
    }

    /// <summary>
    /// PasswordBox 不能直接绑定 Password 字符串，
    /// 所以这里把每次输入同步到 ViewModel 的 Password 属性。
    /// </summary>
    private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.Password = PasswordInput.Password;
    }
}