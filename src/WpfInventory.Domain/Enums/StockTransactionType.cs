namespace WpfInventory.Domain.Enums;

/// <summary>
/// 库存流水类型。
/// </summary>
public enum StockTransactionType
{
    /// <summary>
    /// 入库。
    /// </summary>
    Inbound = 1,

    /// <summary>
    /// 出库。
    /// </summary>
    Outbound = 2,

    /// <summary>
    /// 库存调整。
    /// </summary>
    Adjustment = 3
}
