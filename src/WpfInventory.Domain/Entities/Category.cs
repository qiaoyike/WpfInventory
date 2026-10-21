namespace WpfInventory.Domain.Entities;

/// <summary>
/// 商品分类实体。
/// 一个分类下可以有多个商品，因此是“一对多”关系中的“一”方。
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// 分类名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分类描述。
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否启用。
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 该分类下的商品集合。
    /// 这是导航属性，表示“一对多”中的“多”方。
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
