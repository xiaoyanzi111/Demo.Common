using System;
using System.Security.Cryptography;

namespace Demo.Common.Utilities
{
    /// <summary>
    /// 密码工具：加盐哈希 + 校验
    /// </summary>
    public static class PasswordUtil
    {
        /// <summary>
        /// 生成随机盐值
        /// </summary>
        public static string CreateSalt(int size = 16)
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(size);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 使用 PBKDF2 生成密码哈希
        /// </summary>
        public static string HashPassword(string password, string salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, System.Text.Encoding.UTF8.GetBytes(salt), 10000, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(pbkdf2.GetBytes(32));
        }

        /// <summary>
        /// 校验密码
        /// </summary>
        public static bool VerifyPassword(string password, string salt, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(salt) || string.IsNullOrEmpty(hash)) return false;
            return HashPassword(password, salt) == hash;
        }
    }
}
