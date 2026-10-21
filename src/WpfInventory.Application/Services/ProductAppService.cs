using AutoMapper;
using FluentValidation;
using WpfInventory.Application.Abstractions.Persistence;
using WpfInventory.Domain.Repositories;
using WpfInventory.Application.Dtos.Products;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Services;

/// <summary>
/// 商品应用服务实现。
/// 负责编排用例流程：校验 -> 映射 -> 仓储操作 -> 保存。
/// </summary>
public class ProductAppService : IProductAppService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductRequest> _createValidator;
    private readonly IValidator<UpdateProductRequest> _updateValidator;

    public ProductAppService(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        // 用例入口先做参数校验，保证无效数据不会进入数据库。
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var product = _mapper.Map<Product>(request);
        product.CreatedAt = DateTime.UtcNow;

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task UpdateAsync(UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"未找到 Id 为 {request.Id} 的商品");

        // 把请求字段复制到已跟踪的实体上，EF Core 会自动生成 UPDATE。
        _mapper.Map(request, product);
        product.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"未找到 Id 为 {id} 的商品");

        _productRepository.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
