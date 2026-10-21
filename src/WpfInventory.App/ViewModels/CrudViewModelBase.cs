using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 通用 CRUD 页面 ViewModel 基类。
/// 把“加载列表、新增、修改、删除、选中回填”这些重复流程集中起来，
/// 子类只需要描述表单字段和如何组装请求 DTO。
/// 这是“模板方法模式”：基类定义算法骨架，子类填充具体步骤。
/// </summary>
public abstract partial class CrudViewModelBase<TDto, TCreateRequest, TUpdateRequest> : ObservableObject
{
    private readonly ICrudAppService<TDto, TCreateRequest, TUpdateRequest> _service;
    private readonly IDialogService _dialogService;
    private int? _editingId;

    protected CrudViewModelBase(
        ICrudAppService<TDto, TCreateRequest, TUpdateRequest> service,
        IDialogService dialogService)
    {
        _service = service;
        _dialogService = dialogService;
    }

    /// <summary>
    /// 列表数据。
    /// </summary>
    public ObservableCollection<TDto> Items { get; } = new();

    /// <summary>
    /// 当前选中的行。
    /// </summary>
    [ObservableProperty]
    private TDto? selectedItem;

    /// <summary>
    /// 是否正在执行异步操作，用于禁用按钮和显示进度。
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// 页面内错误提示。
    /// </summary>
    [ObservableProperty]
    private string? errorMessage;

    /// <summary>
    /// 当前是“编辑模式”还是“新增模式”。
    /// </summary>
    [ObservableProperty]
    private bool isEditing;

    /// <summary>
    /// 表单顶部显示的模式提示，让用户清楚当前点击“保存”会发生什么。
    /// </summary>
    public string ModeText => IsEditing
        ? "编辑模式：修改后点“保存”"
        : "新增模式：填写后点“保存”";

    partial void OnIsEditingChanged(bool value)
    {
        OnPropertyChanged(nameof(ModeText));
    }

    /// <summary>
    /// 当前正在编辑的记录主键。
    /// </summary>
    protected int EditingId => _editingId
        ?? throw new InvalidOperationException("当前不在编辑状态");

    /// <summary>
    /// 加载列表。
    /// </summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            SelectedItem = default;
            Items.Clear();

            var data = await _service.GetAllAsync();
            foreach (var item in data)
            {
                Items.Add(item);
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
    /// 新增。子类通过 BuildCreateRequest 提供请求数据。
    /// </summary>
    [RelayCommand]
    private async Task CreateAsync()
    {
        await ExecuteWriteAsync(async () =>
        {
            await _service.CreateAsync(BuildCreateRequest());
            _dialogService.ShowInfo("保存成功");
        });
    }

    /// <summary>
    /// 更新。子类通过 BuildUpdateRequest 提供请求数据。
    /// </summary>
    [RelayCommand]
    private async Task UpdateAsync()
    {
        await ExecuteWriteAsync(async () =>
        {
            await _service.UpdateAsync(BuildUpdateRequest());
            _dialogService.ShowInfo("保存成功");
        });
    }

    /// <summary>
    /// 保存按钮：根据当前是新增还是编辑模式，自动选择新增或更新。
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsEditing)
        {
            await UpdateAsync();
        }
        else
        {
            await CreateAsync();
        }
    }

    /// <summary>
    /// 删除当前选中记录。
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

        await ExecuteWriteAsync(async () =>
        {
            await _service.DeleteAsync(GetId(SelectedItem));
            _dialogService.ShowInfo("删除成功");
        });
    }

    /// <summary>
    /// 切换到新增模式，清空表单。
    /// </summary>
    [RelayCommand]
    private void PrepareCreate()
    {
        _editingId = null;
        IsEditing = false;
        SelectedItem = default;
        ClearForm();
    }

    /// <summary>
    /// 选中列表行时自动回填表单。
    /// </summary>
    partial void OnSelectedItemChanged(TDto? value)
    {
        if (value is null)
        {
            return;
        }

        _editingId = GetId(value);
        IsEditing = true;
        LoadFromItem(value);
    }

    /// <summary>
    /// 统一封装“写操作 + 刷新列表”的公共流程。
    /// </summary>
    private async Task ExecuteWriteAsync(Func<Task> action)
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            await action();
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

    /// <summary>
    /// 根据当前表单组装“新增”请求。
    /// </summary>
    protected abstract TCreateRequest BuildCreateRequest();

    /// <summary>
    /// 根据当前表单组装“更新”请求。
    /// </summary>
    protected abstract TUpdateRequest BuildUpdateRequest();

    /// <summary>
    /// 把选中记录回填到表单。
    /// </summary>
    protected abstract void LoadFromItem(TDto item);

    /// <summary>
    /// 清空表单。
    /// </summary>
    protected abstract void ClearForm();

    /// <summary>
    /// 从 DTO 中取出主键。
    /// </summary>
    protected abstract int GetId(TDto item);
}