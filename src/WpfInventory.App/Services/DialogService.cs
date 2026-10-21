using System.Windows;
using Microsoft.Win32;

namespace WpfInventory.App.Services;

/// <summary>
/// 对话框服务的 WPF 实现。
/// 把 MessageBox 和 SaveFileDialog 封装成接口，方便测试和替换。
/// </summary>
public class DialogService : IDialogService
{
    public bool Confirm(string message, string title = "确认")
    {
        return MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    public void ShowError(string message, string title = "错误")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowInfo(string message, string title = "提示")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public string? PickCsvFile(string defaultFileName)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*",
            FileName = defaultFileName,
            AddExtension = true,
            DefaultExt = ".csv"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}