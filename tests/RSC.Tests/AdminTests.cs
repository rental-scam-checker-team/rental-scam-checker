using RSC.Classes;

namespace RSC.Tests;

public class AdminTests
{
    private const string TestAdminHash =
        "100000.UlNDYWRtaW5TYWx0MjAyNg==.AaoNQRB53yDhxAjCBnbkb1TlwYFDhSTfCRlH8JSsBuQ=";

    [Fact]
    public void HashIsNotThePlainPassword()
    {
        string hash = Admin.HashPassword("Secret123!");
        Assert.DoesNotContain("Secret123!", hash);
    }

    [Fact]
    public void SamePasswordGivesDifferentHashes()
    {
        Assert.NotEqual(Admin.HashPassword("Secret123!"), Admin.HashPassword("Secret123!"));
    }

    [Fact]
    public void CorrectPasswordIsAccepted()
    {
        var admin = new Admin { PasswordHash = Admin.HashPassword("Secret123!") };
        Assert.True(admin.CheckPassword("Secret123!"));
    }

    [Fact]
    public void WrongPasswordIsRejected()
    {
        var admin = new Admin { PasswordHash = Admin.HashPassword("Secret123!") };
        Assert.False(admin.CheckPassword("Wrong123!"));
    }

    [Fact]
    public void PasswordIsCaseSensitive()
    {
        var admin = new Admin { PasswordHash = Admin.HashPassword("Secret123!") };
        Assert.False(admin.CheckPassword("secret123!"));
    }

    [Fact]
    public void BlankPasswordIsRejected()
    {
        var admin = new Admin { PasswordHash = Admin.HashPassword("Secret123!") };
        Assert.False(admin.CheckPassword(""));
    }

    [Fact]
    public void BrokenStoredHashIsRejected()
    {
        var admin = new Admin { PasswordHash = "not-a-real-hash" };
        Assert.False(admin.CheckPassword("Secret123!"));
    }

    [Fact]
    public void TestAdminFromSqlScriptAcceptsItsPassword()
    {
        var admin = new Admin { PasswordHash = TestAdminHash };
        Assert.True(admin.CheckPassword("Admin123!"));
    }

    [Fact]
    public void TestAdminFromSqlScriptRejectsWrongPassword()
    {
        var admin = new Admin { PasswordHash = TestAdminHash };
        Assert.False(admin.CheckPassword("admin123"));
    }
}
