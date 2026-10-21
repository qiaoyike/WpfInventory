namespace WpfInventory.Application.Dtos.Products;

/// <summary>
/// 创建商品的请求 DTO。
/// 不包含 Id、CreatedAt 这类由系统生成或数据库负责的字段。
/// </summary>
public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
}
