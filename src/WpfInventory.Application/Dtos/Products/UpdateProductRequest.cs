namespace WpfInventory.Application.Dtos.Products;

/// <summary>
/// 更新商品的请求 DTO。
/// 比创建请求多一个 Id，用来定位要更新的记录。
/// </summary>
public class UpdateProductRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }
}
