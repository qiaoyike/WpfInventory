namespace WpfInventory.Application.Abstractions.Identity;

/// <summary>
/// 密码哈希器接口。
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
