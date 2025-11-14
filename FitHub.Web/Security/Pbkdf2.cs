using System.Security.Cryptography;

namespace FitHub.Web.Security
{
    public static class Pbkdf2
    {
        public static (byte[] salt, byte[] hash) Hash(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);
            return (salt, Derive(password, salt));
        }

        public static bool Verify(string password, byte[] salt, byte[] expected)
        {
            var test = Derive(password, salt);
            return CryptographicOperations.FixedTimeEquals(test, expected);
        }

        private static byte[] Derive(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
    }
}
