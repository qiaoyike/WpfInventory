using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Identity;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 角色管理页面 ViewModel。
/// 支持角色增删改，并通过复选框维护角色拥有的权限。
/// </summary>
public partial class RoleViewModel : ObservableObject
{
    private readonly IRoleManagementService _roleService;
    private readonly IDialogService _dialogService;

    public RoleViewModel(IRoleManagementService roleService, IDialogService dialogService)
    {
        _roleService = roleService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// 角色列表。
    /// </summary>
    public ObservableCollection<RoleDto> Items { get; } = new();

    /// <summary>
    /// 权限复选框集合。
    /// </summary>
    public ObservableCollection<SelectablePermissionItem> PermissionItems { get; } = new();

    /// <summary>
    /// 当前选中角色。
    /// </summary>
    [ObservableProperty]
    private RoleDto? selectedItem;

    /// <summary>
    /// 表单：角色编码。
    /// </summary>
    [ObservableProperty]
    private string code = string.Empty;

    /// <summary>
    /// 表单：角色名称。
    /// </summary>
    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>
    /// 表单：角色描述。
    /// </summary>
    [ObservableProperty]
    private string? description;

    /// <summary>
    /// 是否正在执行异步操作。
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// 加载角色和权限。
    /// </summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            SelectedItem = null;
            Items.Clear();
            PermissionItems.Clear();

            foreach (var role in await _roleService.GetAllAsync())
            {
                Items.Add(role);
            }

            foreach (var permission in await _roleService.GetAllPermissionsAsync())
            {
                PermissionItems.Add(new SelectablePermissionItem(permission));
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
    /// 新增角色。
    /// </summary>
    [RelayCommand]
    private async Task CreateAsync()
    {
        IsBusy = true;
        try
        {
            await _roleService.CreateAsync(new CreateRoleRequest
            {
                Code = Code,
                Name = Name,
                Description = Description,
                PermissionIds = SelectedPermissionIds()
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
    /// 更新选中角色。
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
        try
        {
            await _roleService.UpdateAsync(new UpdateRoleRequest
            {
                Id = SelectedItem.Id,
                Code = Code,
                Name = Name,
                Description = Description,
                PermissionIds = SelectedPermissionIds()
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
    /// 删除选中角色。
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
            await _roleService.DeleteAsync(SelectedItem.Id);
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
    /// 选中角色时回填表单。
    /// </summary>
    partial void OnSelectedItemChanged(RoleDto? value)
    {
        if (value is null)
        {
            return;
        }

        Code = value.Code;
        Name = value.Name;
        Description = value.Description;

        foreach (var permissionItem in PermissionItems)
        {
            permissionItem.IsSelected = value.PermissionIds.Contains(permissionItem.Permission.Id);
        }
    }

    private void PrepareCreateForm()
    {
        Code = string.Empty;
        Name = string.Empty;
        Description = null;

        foreach (var permissionItem in PermissionItems)
        {
            permissionItem.IsSelected = false;
        }
    }

    private IReadOnlyList<int> SelectedPermissionIds()
    {
        return PermissionItems
            .Where(x => x.IsSelected)
            .Select(x => x.Permission.Id)
            .ToList();
    }

    private void HandleError(Exception ex)
    {
        _dialogService.ShowError(ex.Message);
    }

    /// <summary>
    /// 权限复选框条目。
    /// </summary>
    public partial class SelectablePermissionItem : ObservableObject
    {
        public SelectablePermissionItem(PermissionDto permission)
        {
            Permission = permission;
        }

        public PermissionDto Permission { get; }

        /// <summary>
        /// 是否勾选。
        /// </summary>
        [ObservableProperty]
        private bool isSelected;
    }
}