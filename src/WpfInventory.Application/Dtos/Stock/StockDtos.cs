using WpfInventory.Domain.Enums;

namespace WpfInventory.Application.Dtos.Stock;

public class StockTransactionDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public StockTransactionType TransactionType { get; set; }
    public string TransactionTypeName => TransactionType.ToString();
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateStockTransactionRequest
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public StockTransactionType TransactionType { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Remark { get; set; }
}
