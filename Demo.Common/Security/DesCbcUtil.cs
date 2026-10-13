using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Demo.Common.Security
{
    /// <summary>
    /// DES(CBC) 加解密：常用于配置文件中连接串的加密存储
    /// </summary>
    public static class DesCbcUtil
    {
        // 密钥（8 位）
        private const string key = "@#$%^6a7";
        // 偏移量（8 位）
        private const string iv = "@#$%^6a7";

        /// <summary>
        /// 加密：明文 -> 十六进制密文
        /// </summary>
        public static string Encode(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText)) return plainText;

            using var des = DES.Create();
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            des.Key = Encoding.UTF8.GetBytes(key);
            des.IV = Encoding.UTF8.GetBytes(iv);

            byte[] data = Encoding.UTF8.GetBytes(plainText);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
            }
            return BytesToString(ms.ToArray());
        }

        /// <summary>
        /// 解密：十六进制密文 -> 明文（失败时原样返回）
        /// </summary>
        public static string Decrypt(string cipherText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText)) return cipherText;

                byte[] input = StringToBytes(cipherText);
                using var des = DES.Create();
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = Encoding.UTF8.GetBytes(iv);

                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                    cs.FlushFinalBlock();
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch
            {
                return cipherText;
            }
        }

        private static string BytesToString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                string hex = (0xff & b).ToString("x");
                if (hex.Length < 2) hex = "0" + hex;
                sb.Append(hex);
            }
            return sb.ToString();
        }

        private static byte[] StringToBytes(string hex)
        {
            byte[] digest = new byte[hex.Length / 2];
            for (int i = 0; i < digest.Length; i++)
            {
                digest[i] = Convert.ToByte(hex.Substring(2 * i, 2), 16);
            }
            return digest;
        }
    }
}
