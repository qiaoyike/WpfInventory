using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Infrastructure.Data.Configurations;

/// <summary>
/// 仓库映射配置。
/// </summary>
public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Address).HasMaxLength(200);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}

/// <summary>
/// 供应商映射配置。
/// </summary>
public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ContactPerson).HasMaxLength(50);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Address).HasMaxLength(200);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}

/// <summary>
/// 客户映射配置。
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ContactPerson).HasMaxLength(50);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Address).HasMaxLength(200);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}
