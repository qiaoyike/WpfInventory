using CommunityToolkit.Mvvm.ComponentModel;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.MasterData;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 仓库管理页面 ViewModel。
/// </summary>
public partial class WarehouseViewModel : CrudViewModelBase<WarehouseDto, CreateWarehouseRequest, UpdateWarehouseRequest>
{
    public WarehouseViewModel(
        ICrudAppService<WarehouseDto, CreateWarehouseRequest, UpdateWarehouseRequest> service,
        IDialogService dialogService)
        : base(service, dialogService)
    {
    }

    /// <summary>
    /// 表单：仓库编码。
    /// </summary>
    [ObservableProperty]
    private string code = string.Empty;

    /// <summary>
    /// 表单：仓库名称。
    /// </summary>
    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>
    /// 表单：仓库地址。
    /// </summary>
    [ObservableProperty]
    private string? address;

    /// <summary>
    /// 表单：是否启用。
    /// </summary>
    [ObservableProperty]
    private bool isActive = true;

    protected override CreateWarehouseRequest BuildCreateRequest()
    {
        return new CreateWarehouseRequest { Code = Code, Name = Name, Address = Address };
    }

    protected override UpdateWarehouseRequest BuildUpdateRequest()
    {
        return new UpdateWarehouseRequest
        {
            Id = EditingId,
            Code = Code,
            Name = Name,
            Address = Address,
            IsActive = IsActive
        };
    }

    protected override void LoadFromItem(WarehouseDto item)
    {
        Code = item.Code;
        Name = item.Name;
        Address = item.Address;
        IsActive = item.IsActive;
    }

    protected override void ClearForm()
    {
        Code = string.Empty;
        Name = string.Empty;
        Address = null;
        IsActive = true;
    }

    protected override int GetId(WarehouseDto item)
    {
        return item.Id;
    }
}