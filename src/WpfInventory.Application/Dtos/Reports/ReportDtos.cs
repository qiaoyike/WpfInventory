namespace WpfInventory.Application.Dtos.Reports;

public class ProductStockReportDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
}

public class StockTransactionReportDto
{
    public int Id { get; set; }
    public string? ProductName { get; set; }
    public string? WarehouseName { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? ReferenceNo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DashboardSummaryDto
{
    public int ProductCount { get; set; }
    public int CategoryCount { get; set; }
    public int CustomerCount { get; set; }
    public int SupplierCount { get; set; }
    public int WarehouseCount { get; set; }
    public int TodayInboundQuantity { get; set; }
    public int TodayOutboundQuantity { get; set; }
}
