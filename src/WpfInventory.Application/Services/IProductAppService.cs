using WpfInventory.Application.Dtos.Products;

namespace WpfInventory.Application.Services;

/// <summary>
/// 商品应用服务接口。
/// 它描述业务用例，是表现层唯一需要依赖的应用层入口。
/// </summary>
public interface IProductAppService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
