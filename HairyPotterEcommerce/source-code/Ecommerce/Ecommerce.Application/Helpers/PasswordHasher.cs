using System.Security.Cryptography;

namespace Ecommerce.Application.Helpers;

public class PasswordHasher
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32; // 256 bit
    private const int Iterations = 100000; // Number of iterations

    // Method to hash the password using PBKDF2
    public string HashPassword(string password, out string salt)
    {
        // Generate a cryptographically secure salt
        var saltBytes = new byte[SaltSize];
        RandomNumberGenerator.Fill(saltBytes);
        salt = Convert.ToBase64String(saltBytes);

        // Use PBKDF2 to hash the password with the salt
        var hash = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA512);
        var hashBytes = hash.GetBytes(KeySize);

        // Return the hashed password as a Base64 string
        return Convert.ToBase64String(hashBytes);
    }

    // Method to verify the password using PBKDF2
    public bool VerifyPassword(string password, string storedHash, string salt)
    {
        // Convert the stored salt back to byte array
        var saltBytes = Convert.FromBase64String(salt);

        // Use PBKDF2 to hash the provided password with the stored salt
        var hash = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA512);
        var hashBytes = hash.GetBytes(KeySize);

        // Compare the newly hashed password with the stored hash
        return Convert.ToBase64String(hashBytes) == storedHash;
    }
}