namespace WpfInventory.Domain.Entities;

/// <summary>
/// 所有实体的公共基类。
/// 统一放置主键和审计字段，后续每个业务实体都继承它。
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键，数据库自增。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 创建时间。
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最后更新时间。11
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
