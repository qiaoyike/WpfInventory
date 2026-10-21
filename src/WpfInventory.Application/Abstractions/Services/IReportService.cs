using WpfInventory.Application.Dtos.Reports;

namespace WpfInventory.Application.Abstractions.Services;

public interface IReportService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductStockReportDto>> GetProductStockReportAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockTransactionReportDto>> GetStockTransactionReportAsync(CancellationToken cancellationToken = default);
}
