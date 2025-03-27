using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CSWWeb.Lib.Utils
{
    public class AesEncryptionHelper
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        // DES 固定的 Key 與 IV (需符合 DES 的 8 字節要求)
        private string _DesKey = "WSau4a83";
        private string _DesIV = "WSau4a83";

        public AesEncryptionHelper(IConfiguration configuration)
        {
            var keyString = configuration["AES:Key"];
            var ivString = configuration["AES:IV"];

            if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(ivString))
                throw new ArgumentNullException("AES Key or IV is missing in appsettings.json");

            _key = Encoding.UTF8.GetBytes(keyString);
            _iv = Encoding.UTF8.GetBytes(ivString);
            _DesKey = configuration["DES:Key"];
            _DesIV = configuration["DES:Key"];
            if (_key.Length != 32 || _iv.Length != 16)
                throw new ArgumentException("Invalid AES Key or IV length");
        }

        /// <summary>
        /// 使用 AES 加密明文字串，回傳 Base64 格式的密文。
        /// </summary>
        public string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = ExecuteCryptoTransform(encryptor, plainBytes);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        /// <summary>
        /// 使用 AES 解密 Base64 格式的密文，回傳明文字串。
        /// </summary>
        public string DecryptString(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentNullException(nameof(encryptedText));

            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                    byte[] decryptedBytes = ExecuteCryptoTransform(decryptor, encryptedBytes);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        /// <summary>
        /// 使用 DES 加密明文字串，回傳 Base64 格式的密文。
        /// </summary>
        public string DESEncryptString(string encryptText)
        {
            if (string.IsNullOrEmpty(encryptText))
                throw new ArgumentNullException(nameof(encryptText));

            using (var des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(_DesKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(_DesIV);

                using (var encryptor = des.CreateEncryptor())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(encryptText);
                    byte[] encryptedBytes = ExecuteCryptoTransform(encryptor, inputBytes);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        /// <summary>
        /// 使用 DES 解密 Base64 格式的密文，回傳明文字串。
        /// </summary>
        public string DESDecryptString(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentNullException(nameof(encryptedText));

            using (var des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(_DesKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(_DesIV);

                using (var decryptor = des.CreateDecryptor())
                {
                    byte[] inputBytes = Convert.FromBase64String(encryptedText);
                    byte[] decryptedBytes = ExecuteCryptoTransform(decryptor, inputBytes);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        /// <summary>
        /// 透過指定的 ICryptoTransform 執行加解密作業，並回傳轉換後的位元組陣列。
        /// </summary>
        private byte[] ExecuteCryptoTransform(ICryptoTransform cryptoTransform, byte[] input)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }
    }
}
