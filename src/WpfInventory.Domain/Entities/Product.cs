namespace WpfInventory.Domain.Entities;

/// <summary>
/// 商品实体。
/// 这是库存管理中最核心的业务对象之一。
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// 商品名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品编码，通常用于唯一标识一个 SKU。
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// 商品描述。
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 单价。
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 当前库存数量。
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// 是否启用。
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 所属分类的主键，即外键。
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 所属分类导航属性。
    /// </summary>
    public Category? Category { get; set; }
}
