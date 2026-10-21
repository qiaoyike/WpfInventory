using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Infrastructure.Data.Configurations;

/// <summary>
/// 审计日志映射配置。
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Username).HasMaxLength(50);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(50);
        builder.Property(x => x.EntityName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Message).HasMaxLength(1000);
    }
}
