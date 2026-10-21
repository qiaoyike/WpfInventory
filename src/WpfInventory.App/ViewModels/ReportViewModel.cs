using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Reports;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 报表中心页面 ViewModel。
/// 展示库存报表和流水报表，并支持导出 CSV。
/// </summary>
public partial class ReportViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly IExportService _exportService;
    private readonly IDialogService _dialogService;

    public ReportViewModel(
        IReportService reportService,
        IExportService exportService,
        IDialogService dialogService)
    {
        _reportService = reportService;
        _exportService = exportService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// 商品库存报表。
    /// </summary>
    public ObservableCollection<ProductStockReportDto> ProductStockReport { get; } = new();

    /// <summary>
    /// 库存流水报表。
    /// </summary>
    public ObservableCollection<StockTransactionReportDto> StockTransactionReport { get; } = new();

    /// <summary>
    /// 是否正在加载。
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// 加载两张报表。
    /// </summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            ProductStockReport.Clear();
            StockTransactionReport.Clear();

            foreach (var item in await _reportService.GetProductStockReportAsync())
            {
                ProductStockReport.Add(item);
            }

            foreach (var item in await _reportService.GetStockTransactionReportAsync())
            {
                StockTransactionReport.Add(item);
            }
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

    /// <summary>
    /// 导出商品库存 CSV。
    /// </summary>
    [RelayCommand]
    private async Task ExportProductStockAsync()
    {
        await ExportAsync(_exportService.ExportProductStockAsync, "商品库存报表.csv");
    }

    /// <summary>
    /// 导出库存流水 CSV。
    /// </summary>
    [RelayCommand]
    private async Task ExportStockTransactionsAsync()
    {
        await ExportAsync(_exportService.ExportStockTransactionsAsync, "库存流水报表.csv");
    }

    /// <summary>
    /// 统一的导出流程：选路径 -> 调用服务 -> 提示结果。
    /// 这是“策略模式”的轻量应用，把不同的导出策略作为委托传入。
    /// </summary>
    private async Task ExportAsync(Func<string, CancellationToken, Task<string>> exportAction, string defaultName)
    {
        var path = _dialogService.PickCsvFile(defaultName);
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        IsBusy = true;
        try
        {
            var savedPath = await exportAction(path, CancellationToken.None);
            _dialogService.ShowInfo($"导出成功：{savedPath}");
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