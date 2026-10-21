using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Identity;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 用户管理页面 ViewModel。
/// 支持用户新增、修改、删除，并通过复选框维护用户角色。
/// </summary>
public partial class UserViewModel : ObservableObject
{
    private readonly IUserManagementService _userService;
    private readonly IRoleManagementService _roleService;
    private readonly IDialogService _dialogService;

    public UserViewModel(
        IUserManagementService userService,
        IRoleManagementService roleService,
        IDialogService dialogService)
    {
        _userService = userService;
        _roleService = roleService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// 用户列表。
    /// </summary>
    public ObservableCollection<UserDto> Items { get; } = new();

    /// <summary>
    /// 角色复选框集合。
    /// </summary>
    public ObservableCollection<SelectableRoleItem> RoleItems { get; } = new();

    /// <summary>
    /// 当前选中用户。
    /// </summary>
    [ObservableProperty]
    private UserDto? selectedItem;

    /// <summary>
    /// 表单：用户名。
    /// </summary>
    [ObservableProperty]
    private string username = string.Empty;

    /// <summary>
    /// 表单：初始密码（仅新增时使用）。
    /// </summary>
    [ObservableProperty]
    private string password = string.Empty;

    /// <summary>
    /// 表单：显示名。
    /// </summary>
    [ObservableProperty]
    private string displayName = string.Empty;

    /// <summary>
    /// 表单：是否启用。
    /// </summary>
    [ObservableProperty]
    private bool isActive = true;

    /// <summary>
    /// 是否正在执行异步操作。
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// 加载用户和角色。
    /// </summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            SelectedItem = null;
            Items.Clear();
            RoleItems.Clear();

            foreach (var user in await _userService.GetAllAsync())
            {
                Items.Add(user);
            }

            foreach (var role in await _roleService.GetAllAsync())
            {
                RoleItems.Add(new SelectableRoleItem(role));
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
    /// 新增用户。
    /// </summary>
    [RelayCommand]
    private async Task CreateAsync()
    {
        IsBusy = true;
        try
        {
            await _userService.CreateAsync(new CreateUserRequest
            {
                Username = Username,
                Password = Password,
                DisplayName = DisplayName,
                RoleIds = SelectedRoleIds()
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
    /// 更新选中用户。
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
            await _userService.UpdateAsync(new UpdateUserRequest
            {
                Id = SelectedItem.Id,
                DisplayName = DisplayName,
                IsActive = IsActive,
                RoleIds = SelectedRoleIds()
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
    /// 删除选中用户。
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
            await _userService.DeleteAsync(SelectedItem.Id);
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
    /// 选中用户时回填表单。
    /// </summary>
    partial void OnSelectedItemChanged(UserDto? value)
    {
        if (value is null)
        {
            return;
        }

        Username = value.Username;
        Password = string.Empty;
        DisplayName = value.DisplayName;
        IsActive = value.IsActive;

        foreach (var roleItem in RoleItems)
        {
            roleItem.IsSelected = value.RoleCodes.Contains(roleItem.Role.Code);
        }
    }

    private void PrepareCreateForm()
    {
        Username = string.Empty;
        Password = string.Empty;
        DisplayName = string.Empty;
        IsActive = true;

        foreach (var roleItem in RoleItems)
        {
            roleItem.IsSelected = false;
        }
    }

    private IReadOnlyList<int> SelectedRoleIds()
    {
        return RoleItems
            .Where(x => x.IsSelected)
            .Select(x => x.Role.Id)
            .ToList();
    }

    private void HandleError(Exception ex)
    {
        _dialogService.ShowError(ex.Message);
    }

    /// <summary>
    /// 角色复选框条目。
    /// 把“角色数据”和“是否勾选”包装成一个可绑定对象。
    /// </summary>
    public partial class SelectableRoleItem : ObservableObject
    {
        public SelectableRoleItem(RoleDto role)
        {
            Role = role;
        }

        public RoleDto Role { get; }

        /// <summary>
        /// 是否勾选。
        /// </summary>
        [ObservableProperty]
        private bool isSelected;
    }
}