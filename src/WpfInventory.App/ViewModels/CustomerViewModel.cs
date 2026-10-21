using CommunityToolkit.Mvvm.ComponentModel;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.MasterData;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 客户管理页面 ViewModel。
/// </summary>
public partial class CustomerViewModel : CrudViewModelBase<CustomerDto, CreateCustomerRequest, UpdateCustomerRequest>
{
    public CustomerViewModel(
        ICrudAppService<CustomerDto, CreateCustomerRequest, UpdateCustomerRequest> service,
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

    protected override CreateCustomerRequest BuildCreateRequest()
    {
        return new CreateCustomerRequest
        {
            Code = Code,
            Name = Name,
            ContactPerson = ContactPerson,
            Phone = Phone,
            Address = Address
        };
    }

    protected override UpdateCustomerRequest BuildUpdateRequest()
    {
        return new UpdateCustomerRequest
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

    protected override void LoadFromItem(CustomerDto item)
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

    protected override int GetId(CustomerDto item)
    {
        return item.Id;
    }
}