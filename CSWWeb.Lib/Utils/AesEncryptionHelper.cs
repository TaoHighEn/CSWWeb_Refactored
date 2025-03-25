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

        public AesEncryptionHelper(IConfiguration configuration)
        {
            var keyString = configuration["AES:Key"];
            var ivString = configuration["AES:IV"];

            if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(ivString))
                throw new ArgumentNullException("AES Key or IV is missing in appsettings.json");

            _key = Encoding.UTF8.GetBytes(keyString);
            _iv = Encoding.UTF8.GetBytes(ivString);

            if (_key.Length != 32 || _iv.Length != 16)
                throw new ArgumentException("Invalid AES Key or IV length");
        }

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

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }
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

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        /// <summary>
        /// 現行系統的DES解密
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string DESDecryptString(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentNullException(nameof(encryptedText));
            using (var des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes("WSau4a83");
                des.IV = ASCIIEncoding.ASCII.GetBytes("WSau4a83");
                // 進行解碼
                var ms = new System.IO.MemoryStream();
                var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                var inputByteArray = Convert.FromBase64String(encryptedText);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cs.Close();
                // 將解碼的文字轉成 String.
                var str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }

        /// <summary>
        /// 現行系統的DES加密
        /// </summary>
        /// <param name="encryptText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string DESEncryptString(string encryptText)
        {
            if (string.IsNullOrEmpty(encryptText))
                throw new ArgumentNullException(nameof(encryptText));
            using (var des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes("WSau4a83");
                des.IV = ASCIIEncoding.ASCII.GetBytes("WSau4a83");
                // 進行加密
                var ms = new System.IO.MemoryStream();
                var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                var inputByteArray = Encoding.UTF8.GetBytes(encryptText);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cs.Close();
                // 轉出字串
                var str = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return str;
            }
        }
    }
}
