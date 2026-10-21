using System.Text;
using WpfInventory.Application.Abstractions.Services;

namespace WpfInventory.Application.Services;

/// <summary>
/// CSV 导出服务。
/// 后续可以替换成 Excel、PDF 等实现。
/// </summary>
public class CsvExportService : IExportService
{
    private readonly IReportService _reportService;

    public CsvExportService(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<string> ExportProductStockAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var data = await _reportService.GetProductStockReportAsync(cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("Id,Sku,Name,CategoryName,UnitPrice,StockQuantity");

        foreach (var item in data)
        {
            sb.AppendLine($"{item.Id},{Escape(item.Sku)},{Escape(item.Name)},{Escape(item.CategoryName ?? "")},{item.UnitPrice},{item.StockQuantity}");
        }

        await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8, cancellationToken);
        return filePath;
    }

    public async Task<string> ExportStockTransactionsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var data = await _reportService.GetStockTransactionReportAsync(cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("Id,ProductName,WarehouseName,TransactionType,Quantity,UnitPrice,ReferenceNo,CreatedAt");

        foreach (var item in data)
        {
            sb.AppendLine($"{item.Id},{Escape(item.ProductName ?? "")},{Escape(item.WarehouseName ?? "")},{item.TransactionType},{item.Quantity},{item.UnitPrice},{Escape(item.ReferenceNo ?? "")},{item.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        }

        await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8, cancellationToken);
        return filePath;
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
