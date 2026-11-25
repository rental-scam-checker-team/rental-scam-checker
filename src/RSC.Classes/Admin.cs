using System.Security.Cryptography;

namespace RSC.Classes;

public class Admin
{
    public const int Iterations = 100000;
    public const int SaltSize = 16;
    public const int HashSize = 32;

    public int AdminId { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime DateCreated { get; set; } = DateTime.Today;

    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool CheckPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        string[] parts = PasswordHash.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out int iterations))
            return false;

        byte[] salt;
        byte[] expected;
        try
        {
            salt = Convert.FromBase64String(parts[1]);
            expected = Convert.FromBase64String(parts[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
