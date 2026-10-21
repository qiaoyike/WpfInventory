using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Services;
using WpfInventory.Application.Dtos.MasterData;
using WpfInventory.Application.Dtos.Products;
using WpfInventory.Application.Dtos.Stock;
using WpfInventory.Domain.Enums;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 库存流水页面 ViewModel。
/// 库存流水一旦生成通常不允许随意修改，因此这里只提供查询和新增。
/// </summary>
public partial class StockTransactionViewModel : ObservableObject
{
    private readonly IStockTransactionService _stockService;
    private readonly IProductAppService _productService;
    private readonly ICrudAppService<WarehouseDto, CreateWarehouseRequest, UpdateWarehouseRequest> _warehouseService;
    private readonly IDialogService _dialogService;

    public StockTransactionViewModel(
        IStockTransactionService stockService,
        IProductAppService productService,
        ICrudAppService<WarehouseDto, CreateWarehouseRequest, UpdateWarehouseRequest> warehouseService,
        IDialogService dialogService)
    {
        _stockService = stockService;
        _productService = productService;
        _warehouseService = warehouseService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// 流水列表。
    /// </summary>
    public ObservableCollection<StockTransactionDto> Items { get; } = new();

    /// <summary>
    /// 商品下拉数据。
    /// </summary>
    public ObservableCollection<ProductDto> Products { get; } = new();

    /// <summary>
    /// 仓库下拉数据。
    /// </summary>
    public ObservableCollection<WarehouseDto> Warehouses { get; } = new();

    /// <summary>
    /// 流水类型下拉数据。
    /// </summary>
    public IReadOnlyList<StockTransactionType> TransactionTypes { get; } = Enum.GetValues<StockTransactionType>();

    /// <summary>
    /// 表单：商品主键。
    /// </summary>
    [ObservableProperty]
    private int productId;

    /// <summary>
    /// 表单：仓库主键。
    /// </summary>
    [ObservableProperty]
    private int warehouseId;

    /// <summary>
    /// 表单：流水类型。
    /// </summary>
    [ObservableProperty]
    private StockTransactionType transactionType = StockTransactionType.Inbound;

    /// <summary>
    /// 表单：数量。
    /// </summary>
    [ObservableProperty]
    private int quantity = 1;

    /// <summary>
    /// 表单：单价。
    /// </summary>
    [ObservableProperty]
    private decimal unitPrice;

    /// <summary>
    /// 表单：单据号。
    /// </summary>
    [ObservableProperty]
    private string? referenceNo;

    /// <summary>
    /// 表单：备注。
    /// </summary>
    [ObservableProperty]
    private string? remark;

    /// <summary>
    /// 是否正在执行异步操作。
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// 页面内错误提示。
    /// </summary>
    [ObservableProperty]
    private string? errorMessage;

    /// <summary>
    /// 加载流水和下拉数据。
    /// </summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            Items.Clear();
            Products.Clear();
            Warehouses.Clear();

            foreach (var item in await _stockService.GetAllAsync())
            {
                Items.Add(item);
            }

            foreach (var product in await _productService.GetAllAsync())
            {
                Products.Add(product);
            }

            foreach (var warehouse in await _warehouseService.GetAllAsync())
            {
                Warehouses.Add(warehouse);
            }

            if (ProductId == 0 && Products.Count > 0)
            {
                ProductId = Products[0].Id;
            }

            if (WarehouseId == 0 && Warehouses.Count > 0)
            {
                WarehouseId = Warehouses[0].Id;
            }
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// 新增库存流水。
    /// </summary>
    [RelayCommand]
    private async Task CreateAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            await _stockService.CreateAsync(new CreateStockTransactionRequest
            {
                ProductId = ProductId,
                WarehouseId = WarehouseId,
                TransactionType = TransactionType,
                Quantity = Quantity,
                UnitPrice = UnitPrice,
                ReferenceNo = ReferenceNo,
                Remark = Remark
            });

            _dialogService.ShowInfo("流水创建成功");
            await LoadAsync();
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void HandleError(Exception ex)
    {
        ErrorMessage = ex.Message;
        _dialogService.ShowError(ex.Message);
    }
}