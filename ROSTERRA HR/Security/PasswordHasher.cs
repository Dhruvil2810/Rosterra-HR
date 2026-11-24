using System;
using System.Security.Cryptography;

namespace ROSTERRA_HR.Security
{
    /// <summary>
    /// Provides cryptographically secure password hashing and verification using PBKDF2 (HMAC-SHA256).
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSize = 16;       // 128-bit salt
        private const int HashSize = 32;       // 256-bit subkey
        private const int Iterations = 100000; // PBKDF2 iteration count

        /// <summary>
        /// Generates a salt and hashes the plain-text password.
        /// </summary>
        public static void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
            }

            byte[] saltBytes = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(HashSize);
                passwordHash = Convert.ToBase64String(hashBytes);
                passwordSalt = Convert.ToBase64String(saltBytes);
            }
        }

        /// <summary>
        /// Verifies a candidate plain-text password against the stored PBKDF2 hash and salt.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
            {
                return false;
            }

            try
            {
                byte[] saltBytes = Convert.FromBase64String(storedSalt);
                byte[] storedHashBytes = Convert.FromBase64String(storedHash);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
                {
                    byte[] computedHashBytes = pbkdf2.GetBytes(HashSize);
                    return SlowEquals(storedHashBytes, computedHashBytes);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Constant-time byte array comparison to protect against timing attacks.
        /// </summary>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}
