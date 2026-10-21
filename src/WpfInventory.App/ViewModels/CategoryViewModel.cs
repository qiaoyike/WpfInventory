using CommunityToolkit.Mvvm.ComponentModel;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Categories;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 分类管理页面 ViewModel。
/// 复用通用 CRUD 基类，只需要补充分类特有的表单字段。
/// </summary>
public partial class CategoryViewModel : CrudViewModelBase<CategoryDto, CreateCategoryRequest, UpdateCategoryRequest>
{
    public CategoryViewModel(
        ICrudAppService<CategoryDto, CreateCategoryRequest, UpdateCategoryRequest> service,
        IDialogService dialogService)
        : base(service, dialogService)
    {
    }

    /// <summary>
    /// 表单：分类名称。
    /// </summary>
    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>
    /// 表单：分类描述。
    /// </summary>
    [ObservableProperty]
    private string? description;

    /// <summary>
    /// 表单：是否启用。
    /// </summary>
    [ObservableProperty]
    private bool isActive = true;

    protected override CreateCategoryRequest BuildCreateRequest()
    {
        return new CreateCategoryRequest
        {
            Name = Name,
            Description = Description
        };
    }

    protected override UpdateCategoryRequest BuildUpdateRequest()
    {
        return new UpdateCategoryRequest
        {
            Id = EditingId,
            Name = Name,
            Description = Description,
            IsActive = IsActive
        };
    }

    protected override void LoadFromItem(CategoryDto item)
    {
        Name = item.Name;
        Description = item.Description;
        IsActive = item.IsActive;
    }

    protected override void ClearForm()
    {
        Name = string.Empty;
        Description = null;
        IsActive = true;
    }

    protected override int GetId(CategoryDto item)
    {
        return item.Id;
    }
}