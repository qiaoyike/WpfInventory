namespace WpfInventory.Application.Abstractions.Services;

/// <summary>
/// 通用 CRUD 应用服务接口。
/// 适用于标准的主数据管理模块。
/// </summary>
public interface ICrudAppService<TDto, in TCreateRequest, in TUpdateRequest>
{
    Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TDto> CreateAsync(TCreateRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(TUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
