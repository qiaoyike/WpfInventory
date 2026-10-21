using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfInventory.App.Services;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Audit;

namespace WpfInventory.App.ViewModels;

/// <summary>
/// 审计日志页面 ViewModel。
/// 审计日志只读，因此只提供加载，不提供增删改。
/// </summary>
public partial class AuditLogViewModel : ObservableObject
{
    private readonly IAuditLogService _auditLogService;
    private readonly IDialogService _dialogService;

    public AuditLogViewModel(IAuditLogService auditLogService, IDialogService dialogService)
    {
        _auditLogService = auditLogService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// 审计日志列表。
    /// </summary>
    public ObservableCollection<AuditLogDto> Items { get; } = new();

    /// <summary>
    /// 是否正在加载。
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// 加载审计日志。
    /// </summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            Items.Clear();
            foreach (var item in await _auditLogService.GetAllAsync())
            {
                Items.Add(item);
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
}