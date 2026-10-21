using WpfInventory.Application.Dtos.Stock;

namespace WpfInventory.Application.Abstractions.Services;

public interface IStockTransactionService
{
    Task<IReadOnlyList<StockTransactionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockTransactionDto>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<StockTransactionDto> CreateAsync(CreateStockTransactionRequest request, CancellationToken cancellationToken = default);
}
