using CommunityToolkit.Mvvm.ComponentModel;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.MasterData;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 供应商管理页面 ViewModel。
/// </summary>
public partial class SupplierViewModel : CrudViewModelBase<SupplierDto, CreateSupplierRequest, UpdateSupplierRequest>
{
    public SupplierViewModel(
        ICrudAppService<SupplierDto, CreateSupplierRequest, UpdateSupplierRequest> service,
        IDialogService dialogService)
        : base(service, dialogService)
    {
    }

    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string? contactPerson;

    [ObservableProperty]
    private string? phone;

    [ObservableProperty]
    private string? address;

    [ObservableProperty]
    private bool isActive = true;

    protected override CreateSupplierRequest BuildCreateRequest()
    {
        return new CreateSupplierRequest
        {
            Code = Code,
            Name = Name,
            ContactPerson = ContactPerson,
            Phone = Phone,
            Address = Address
        };
    }

    protected override UpdateSupplierRequest BuildUpdateRequest()
    {
        return new UpdateSupplierRequest
        {
            Id = EditingId,
            Code = Code,
            Name = Name,
            ContactPerson = ContactPerson,
            Phone = Phone,
            Address = Address,
            IsActive = IsActive
        };
    }

    protected override void LoadFromItem(SupplierDto item)
    {
        Code = item.Code;
        Name = item.Name;
        ContactPerson = item.ContactPerson;
        Phone = item.Phone;
        Address = item.Address;
        IsActive = item.IsActive;
    }

    protected override void ClearForm()
    {
        Code = string.Empty;
        Name = string.Empty;
        ContactPerson = null;
        Phone = null;
        Address = null;
        IsActive = true;
    }

    protected override int GetId(SupplierDto item)
    {
        return item.Id;
    }
}