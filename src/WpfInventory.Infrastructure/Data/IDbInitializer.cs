namespace WpfInventory.Infrastructure.Data;

/// <summary>
/// 数据库初始化器接口。
/// </summary>
public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
