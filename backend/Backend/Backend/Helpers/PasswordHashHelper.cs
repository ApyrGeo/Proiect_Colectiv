using Backend.Interfaces;
using System.Security.Cryptography;

namespace Backend.Helpers;

public class PasswordHashHelper : IPasswordHashHelper
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        // Use PBKDF2 for hashing
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[16];
        rng.GetBytes(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        // Combine salt and hash for storage
        var result = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);

        return Convert.ToBase64String(result);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        if (string.IsNullOrEmpty(hashedPassword))
            throw new ArgumentException("Hashed password cannot be null or empty.", nameof(hashedPassword));

        var decodedHash = Convert.FromBase64String(hashedPassword);
        // Extract salt and hash from stored value
        var salt = new byte[16];
        var storedHash = new byte[32];
        Buffer.BlockCopy(decodedHash, 0, salt, 0, salt.Length);
        Buffer.BlockCopy(decodedHash, salt.Length, storedHash, 0, storedHash.Length);
        // Hash the input password with the same salt
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        // Compare the hashes
        return CryptographicOperations.FixedTimeEquals(hash, storedHash);
    }
}
