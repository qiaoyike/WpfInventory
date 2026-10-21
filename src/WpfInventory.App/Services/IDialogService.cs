namespace WpfInventory.App.Services;

/// <summary>
/// 对话框服务接口。
/// ViewModel 通过它发起确认、报错和文件选择，
/// 从而避免直接依赖 MessageBox / SaveFileDialog 这类 UI 控件。
/// 这是“依赖倒置原则”在表现层的一种体现。
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// 弹出确认框，返回用户是否选择“是”。
    /// </summary>
    bool Confirm(string message, string title = "确认");

    /// <summary>
    /// 弹出错误提示。
    /// </summary>
    void ShowError(string message, string title = "错误");

    /// <summary>
    /// 弹出普通提示。
    /// </summary>
    void ShowInfo(string message, string title = "提示");

    /// <summary>
    /// 弹出 CSV 文件保存对话框，返回用户选择的路径；取消时返回 null。
    /// </summary>
    string? PickCsvFile(string defaultFileName);
}