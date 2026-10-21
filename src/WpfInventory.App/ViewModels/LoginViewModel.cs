using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Identity;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 登录窗口 ViewModel。
/// 负责收集用户名密码并调用认证服务。
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 登录成功后触发，View 层据此切换到主窗口。
    /// </summary>
    public event Action? LoginSucceeded;

    /// <summary>
    /// 用户名。
    /// </summary>
    [ObservableProperty]
    private string username = "admin";

    /// <summary>
    /// 密码。
    /// </summary>
    [ObservableProperty]
    private string password = string.Empty;

    /// <summary>
    /// 是否正在执行登录请求。
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// 登录失败时的提示信息。
    /// </summary>
    [ObservableProperty]
    private string? errorMessage;

    /// <summary>
    /// 执行登录。
    /// AsyncRelayCommand 会自动处理异步等待，并在执行期间禁止重复点击。
    /// </summary>
    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "请输入用户名和密码";
            return;
        }

        IsBusy = true;
        try
        {
            var user = await _authService.LoginAsync(
                new LoginRequest { Username = Username, Password = Password });

            if (user is null)
            {
                ErrorMessage = "用户名或密码错误，或账号已停用";
                return;
            }

            LoginSucceeded?.Invoke();
        }
        catch (Exception ex)
        {
            // FluentValidation 校验异常、数据库异常等统一在这里提示。
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}