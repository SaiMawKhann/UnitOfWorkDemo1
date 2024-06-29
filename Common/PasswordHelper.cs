using System.Security.Cryptography;

namespace UnitOfWorkDemo1.Common
{
    public class PasswordHelper
    {
        public class PBKDF2Result
        {
            public string Salt { get; set; }

            public string Hash { get; set; }
        }

        public const int SaltByteSize = 24;

        public const int HashByteSize = 20;

        public const int Pbkdf2Iterations = 1000;

        public const int IterationIndex = 0;

        public const int SaltIndex = 1;

        public const int Pbkdf2Index = 2;

        public static PBKDF2Result HashPassword(string password)
        {
            RandomNumberGenerator rNGCryptoServiceProvider = RandomNumberGenerator.Create();
            byte[] array = new byte[24];
            rNGCryptoServiceProvider.GetBytes(array);
            byte[] pbkdf2Bytes = GetPbkdf2Bytes(password, array, 1000, array.Length);
            return new PBKDF2Result
            {
                Salt = Convert.ToBase64String(array),
                Hash = Convert.ToBase64String(pbkdf2Bytes)
            };
        }

        private static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            return new Rfc2898DeriveBytes(password, salt)
            {
                IterationCount = iterations
            }.GetBytes(outputBytes);
        }

        public static bool ValidatePassword(string password, PBKDF2Result correctHash)
        {
            byte[] salt = Convert.FromBase64String(correctHash.Salt);
            byte[] array = Convert.FromBase64String(correctHash.Hash);
            byte[] pbkdf2Bytes = GetPbkdf2Bytes(password, salt, 1000, array.Length);
            return SlowEquals(array, pbkdf2Bytes);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint num = (uint)(a.Length ^ b.Length);
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                num |= (uint)(a[i] ^ b[i]);
            }

            return num == 0;
        }
    }
}
