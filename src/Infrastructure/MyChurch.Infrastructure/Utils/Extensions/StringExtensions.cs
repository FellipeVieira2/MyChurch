using System.Security.Cryptography;
using System.Text;

namespace MyChurch.Infrastructure.Utils.Extensions
{
    public static class StringExtensions
    {

        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("3f2504e0-4f89-11d3-9a0c-0305e82c3301");

        public static string Encrypt(this string plainText, string key)
        {
            using var aes = Aes.Create();
            using var keyDerivationFunction = new Rfc2898DeriveBytes(key, Salt, 10000);
            aes.Key = keyDerivationFunction.GetBytes(32);
            aes.IV = keyDerivationFunction.GetBytes(16);

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string Decrypt(this string cipherText, string key)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            using var keyDerivationFunction = new Rfc2898DeriveBytes(key, Salt, 10000);
            aes.Key = keyDerivationFunction.GetBytes(32);
            aes.IV = keyDerivationFunction.GetBytes(16);

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var msDecrypt = new MemoryStream(cipherBytes);
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
}
