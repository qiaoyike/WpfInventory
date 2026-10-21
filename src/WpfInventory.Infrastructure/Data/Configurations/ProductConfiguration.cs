using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Infrastructure.Data.Configurations;

/// <summary>
/// Product 实体的 EF Core 映射配置。
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(50);

        // 商品编码通常需要唯一，避免重复 SKU。
        builder.HasIndex(x => x.Sku).IsUnique();

        // 金额字段统一保留两位小数，避免浮点误差。
        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
