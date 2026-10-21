using AutoMapper;
using FluentValidation;
using WpfInventory.Application.Abstractions.Persistence;
using WpfInventory.Domain.Repositories;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Stock;
using WpfInventory.Domain.Entities;
using WpfInventory.Domain.Enums;

namespace WpfInventory.Application.Services;

/// <summary>
/// 库存流水服务。
/// 入库、出库和调整都会在这里统一处理，并同步更新商品库存。
/// </summary>
public class StockTransactionService : IStockTransactionService
{
    private readonly IProductRepository _productRepository;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IRepository<Warehouse> _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateStockTransactionRequest> _createValidator;
    private readonly IAuditService _auditService;

    public StockTransactionService(
        IProductRepository productRepository,
        IStockTransactionRepository stockTransactionRepository,
        IRepository<Warehouse> warehouseRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateStockTransactionRequest> createValidator,
        IAuditService auditService)
    {
        _productRepository = productRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _auditService = auditService;
    }

    public async Task<IReadOnlyList<StockTransactionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var transactions = await _stockTransactionRepository.GetAllWithDetailsAsync(cancellationToken);
        return _mapper.Map<List<StockTransactionDto>>(transactions);
    }

    public async Task<IReadOnlyList<StockTransactionDto>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var transactions = await _stockTransactionRepository.GetByProductIdAsync(productId, cancellationToken);
        return _mapper.Map<List<StockTransactionDto>>(transactions);
    }

    public async Task<StockTransactionDto> CreateAsync(CreateStockTransactionRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new KeyNotFoundException($"未找到 Id 为 {request.ProductId} 的商品");

        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken)
            ?? throw new KeyNotFoundException($"未找到 Id 为 {request.WarehouseId} 的仓库");

        StockTransaction? transaction = null;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            switch (request.TransactionType)
            {
                case StockTransactionType.Inbound:
                    product.StockQuantity += request.Quantity;
                    break;
                case StockTransactionType.Outbound:
                    if (product.StockQuantity < request.Quantity)
                    {
                        throw new InvalidOperationException("库存不足，无法出库");
                    }
                    product.StockQuantity -= request.Quantity;
                    break;
                case StockTransactionType.Adjustment:
                    product.StockQuantity = request.Quantity;
                    break;
            }

            product.UpdatedAt = DateTime.UtcNow;
            _productRepository.Update(product);

            transaction = new StockTransaction
            {
                ProductId = product.Id,
                WarehouseId = warehouse.Id,
                TransactionType = request.TransactionType,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                ReferenceNo = request.ReferenceNo,
                Remark = request.Remark,
                CreatedAt = DateTime.UtcNow
            };

            await _stockTransactionRepository.AddAsync(transaction, cancellationToken);
        }, cancellationToken);

        await _auditService.RecordAsync("创建库存流水", "StockTransaction", transaction!.Id, request.TransactionType.ToString(), cancellationToken);

        return _mapper.Map<StockTransactionDto>(transaction);
    }
}
