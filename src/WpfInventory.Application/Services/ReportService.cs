using AutoMapper;
using WpfInventory.Domain.Repositories;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Application.Dtos.Reports;
using WpfInventory.Domain.Entities;
using WpfInventory.Domain.Enums;

namespace WpfInventory.Application.Services;

/// <summary>
/// 报表统计服务。
/// </summary>
public class ReportService : IReportService
{
    private readonly IProductRepository _productRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Supplier> _supplierRepository;
    private readonly IRepository<Warehouse> _warehouseRepository;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IMapper _mapper;

    public ReportService(
        IProductRepository productRepository,
        IRepository<Category> categoryRepository,
        IRepository<Customer> customerRepository,
        IRepository<Supplier> supplierRepository,
        IRepository<Warehouse> warehouseRepository,
        IStockTransactionRepository stockTransactionRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _customerRepository = customerRepository;
        _supplierRepository = supplierRepository;
        _warehouseRepository = warehouseRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _mapper = mapper;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        var warehouses = await _warehouseRepository.GetAllAsync(cancellationToken);
        var transactions = await _stockTransactionRepository.GetAllWithDetailsAsync(cancellationToken);

        var today = DateTime.UtcNow.Date;

        return new DashboardSummaryDto
        {
            ProductCount = products.Count,
            CategoryCount = categories.Count,
            CustomerCount = customers.Count,
            SupplierCount = suppliers.Count,
            WarehouseCount = warehouses.Count,
            TodayInboundQuantity = transactions.Where(x => x.TransactionType == StockTransactionType.Inbound && x.CreatedAt >= today).Sum(x => x.Quantity),
            TodayOutboundQuantity = transactions.Where(x => x.TransactionType == StockTransactionType.Outbound && x.CreatedAt >= today).Sum(x => x.Quantity)
        };
    }

    public async Task<IReadOnlyList<ProductStockReportDto>> GetProductStockReportAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<ProductStockReportDto>>(products);
    }

    public async Task<IReadOnlyList<StockTransactionReportDto>> GetStockTransactionReportAsync(CancellationToken cancellationToken = default)
    {
        var transactions = await _stockTransactionRepository.GetAllWithDetailsAsync(cancellationToken);
        return _mapper.Map<List<StockTransactionReportDto>>(transactions);
    }
}
