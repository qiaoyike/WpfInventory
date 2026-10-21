namespace WpfInventory.Application.Abstractions.Services;

public interface IExportService
{
    Task<string> ExportProductStockAsync(string filePath, CancellationToken cancellationToken = default);
    Task<string> ExportStockTransactionsAsync(string filePath, CancellationToken cancellationToken = default);
}
