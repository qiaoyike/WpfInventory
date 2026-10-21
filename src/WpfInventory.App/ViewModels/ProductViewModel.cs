using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Services;
using WpfInventory.Application.Dtos.Categories;
using WpfInventory.Application.Dtos.Products;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 商品管理页面 ViewModel。
/// 商品模块有自己的应用服务，因此这里不继承通用 CRUD 基类，
/// 而是直接依赖 IProductAppService，并额外加载分类下拉数据。
/// </summary>
public partial class ProductViewModel : ObservableObject
{
    private readonly IProductAppService _productService;
    private readonly ICrudAppService<CategoryDto, CreateCategoryRequest, UpdateCategoryRequest> _categoryService;
    private readonly IDialogService _dialogService;

    public ProductViewModel(
        IProductAppService productService,
        ICrudAppService<CategoryDto, CreateCategoryRequest, UpdateCategoryRequest> categoryService,
        IDialogService dialogService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// 商品列表。
    /// </summary>
    public ObservableCollection<ProductDto> Items { get; } = new();

    /// <summary>
    /// 分类下拉数据。
    /// </summary>
    public ObservableCollection<CategoryDto> Categories { get; } = new();

    /// <summary>
    /// 当前选中商品。
    /// </summary>
    [ObservableProperty]
    private ProductDto? selectedItem;

    /// <summary>
    /// 表单：商品名称。
    /// </summary>
    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>
    /// 表单：SKU。
    /// </summary>
    [ObservableProperty]
    private string sku = string.Empty;

    /// <summary>
    /// 表单：商品描述。
    /// </summary>
    [ObservableProperty]
    private string? description;

    /// <summary>
    /// 表单：单价。
    /// </summary>
    [ObservableProperty]
    private decimal unitPrice;

    /// <summary>
    /// 表单：库存数量。
    /// </summary>
    [ObservableProperty]
    private int stockQuantity;

    /// <summary>
    /// 表单：是否启用。
    /// </summary>
    [ObservableProperty]
    private bool isActive = true;

    /// <summary>
    /// 表单：所属分类主键。
    /// </summary>
    [ObservableProperty]
    private int categoryId;

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
    /// 加载商品和分类。
    /// </summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            SelectedItem = null;
            Items.Clear();
            Categories.Clear();

            foreach (var product in await _productService.GetAllAsync())
            {
                Items.Add(product);
            }

            foreach (var category in await _categoryService.GetAllAsync())
            {
                Categories.Add(category);
            }

            // 新增时默认选中第一个分类，减少误提交无分类商品。
            if (CategoryId == 0 && Categories.Count > 0)
            {
                CategoryId = Categories[0].Id;
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
    /// 新增商品。
    /// </summary>
    [RelayCommand]
    private async Task CreateAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            await _productService.CreateAsync(new CreateProductRequest
            {
                Name = Name,
                Sku = Sku,
                Description = Description,
                UnitPrice = UnitPrice,
                StockQuantity = StockQuantity,
                CategoryId = CategoryId
            });

            _dialogService.ShowInfo("保存成功");
            await LoadAsync();
            PrepareCreateForm();
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
    /// 更新选中商品。
    /// </summary>
    [RelayCommand]
    private async Task UpdateAsync()
    {
        if (SelectedItem is null)
        {
            _dialogService.ShowInfo("请先选择一条记录");
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            await _productService.UpdateAsync(new UpdateProductRequest
            {
                Id = SelectedItem.Id,
                Name = Name,
                Sku = Sku,
                Description = Description,
                UnitPrice = UnitPrice,
                StockQuantity = StockQuantity,
                IsActive = IsActive,
                CategoryId = CategoryId
            });

            _dialogService.ShowInfo("保存成功");
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

    /// <summary>
    /// 保存按钮：根据当前是否选中记录，自动选择新增或更新。
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedItem is null)
        {
            await CreateAsync();
        }
        else
        {
            await UpdateAsync();
        }
    }

    /// <summary>
    /// 删除选中商品。
    /// </summary>
    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedItem is null)
        {
            _dialogService.ShowInfo("请先选择一条记录");
            return;
        }

        if (!_dialogService.Confirm("确定要删除这条记录吗？"))
        {
            return;
        }

        IsBusy = true;
        try
        {
            await _productService.DeleteAsync(SelectedItem.Id);
            _dialogService.ShowInfo("删除成功");
            await LoadAsync();
            PrepareCreateForm();
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
    /// 清空表单，进入新增模式。
    /// </summary>
    [RelayCommand]
    private void PrepareCreate()
    {
        SelectedItem = null;
        PrepareCreateForm();
    }

    /// <summary>
    /// 选中商品时回填表单。
    /// </summary>
    partial void OnSelectedItemChanged(ProductDto? value)
    {
        if (value is null)
        {
            return;
        }

        Name = value.Name;
        Sku = value.Sku;
        Description = value.Description;
        UnitPrice = value.UnitPrice;
        StockQuantity = value.StockQuantity;
        IsActive = value.IsActive;
        CategoryId = value.CategoryId;
    }

    private void PrepareCreateForm()
    {
        Name = string.Empty;
        Sku = string.Empty;
        Description = null;
        UnitPrice = 0;
        StockQuantity = 0;
        IsActive = true;
        CategoryId = Categories.Count > 0 ? Categories[0].Id : 0;
    }

    private void HandleError(Exception ex)
    {
        ErrorMessage = ex.Message;
        _dialogService.ShowError(ex.Message);
    }
}