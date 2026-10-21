using WpfInventory.Application.Security;

namespace WpfInventory.Application.Tests;

/// <summary>
/// 密码哈希器测试。
/// </summary>
public class Pbkdf2PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ReturnsValueDifferentFromPlainPassword()
    {
        var hash = _hasher.Hash("Admin@123456");

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.NotEqual("Admin@123456", hash);
    }

    [Fact]
    public void Verify_ReturnsTrueForCorrectPassword()
    {
        var hash = _hasher.Hash("Secret123");

        var result = _hasher.Verify("Secret123", hash);

        Assert.True(result);
    }

    [Fact]
    public void Verify_ReturnsFalseForWrongPassword()
    {
        var hash = _hasher.Hash("Secret123");

        var result = _hasher.Verify("WrongPassword", hash);

        Assert.False(result);
    }

    [Fact]
    public void Verify_ReturnsFalseForMalformedHash()
    {
        var result = _hasher.Verify("Secret123", "not-a-valid-hash");

        Assert.False(result);
    }
}