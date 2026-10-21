using WpfInventory.Domain.Enums;

namespace WpfInventory.Domain.Entities;

/// <summary>
/// 库存流水实体。
/// 记录每次入库、出库或库存调整，便于追溯。
/// </summary>
public class StockTransaction : BaseEntity
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    /// <summary>
    /// 流水类型：入库、出库或调整。
    /// </summary>
    public StockTransactionType TransactionType { get; set; }

    /// <summary>
    /// 本次数量。统一存正数，方向由 TransactionType 决定。
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 本次流水对应的单价快照。
    /// </summary>
    public decimal UnitPrice { get; set; }

    public string? ReferenceNo { get; set; }
    public string? Remark { get; set; }
}
