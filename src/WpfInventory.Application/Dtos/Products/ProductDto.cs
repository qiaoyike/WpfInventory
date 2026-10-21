namespace WpfInventory.Application.Dtos.Products;

/// <summary>
/// 商品查询结果 DTO。
/// 它只暴露给表现层，避免把数据库实体直接传到界面。
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}
