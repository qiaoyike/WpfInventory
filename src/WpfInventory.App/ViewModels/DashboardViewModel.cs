using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Reports;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 仪表盘页面 ViewModel。
/// 展示商品、分类、客户、供应商、仓库数量和今日出入库汇总。
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly IDialogService _dialogService;

    public DashboardViewModel(IReportService reportService, IDialogService dialogService)
    {
        _reportService = reportService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// 仪表盘汇总数据。
    /// </summary>
    [ObservableProperty]
    private DashboardSummaryDto? summary;

    /// <summary>
    /// 是否正在加载。
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// 加载统计数据。
    /// </summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            Summary = await _reportService.GetDashboardSummaryAsync();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}