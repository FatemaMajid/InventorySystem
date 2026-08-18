using System.Security.Cryptography;
using InventorySystem.Application.Common.Interfaces;

namespace InventorySystem.Infrastructure.Authentication;

public sealed class PasswordService : IPasswordService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return string.Join(
            '.',
            Iterations,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool VerifyPassword(
        string password,
        string passwordHash)
    {
        try
        {
            var parts = passwordHash.Split('.');

            if (parts.Length != 3)
                return false;

            var iterations = int.Parse(parts[0]);

            var salt =
                Convert.FromBase64String(parts[1]);

            var expectedHash =
                Convert.FromBase64String(parts[2]);

            var actualHash =
                Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(
                actualHash,
                expectedHash);
        }
        catch
        {
            return false;
        }
    }
}