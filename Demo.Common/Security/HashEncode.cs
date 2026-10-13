using System;
using System.Security.Cryptography;
using System.Text;

namespace Demo.Common.Security
{
    /// <summary>
    /// 哈希工具（SHA256 / HMACSHA256）
    /// </summary>
    public static class HashEncode
    {
        /// <summary>
        /// SHA256 哈希（十六进制大写）
        /// </summary>
        public static string Sha256(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash);
        }

        /// <summary>
        /// HMACSHA256 签名
        /// </summary>
        public static string HmacSha256(string input, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash);
        }
    }
}
