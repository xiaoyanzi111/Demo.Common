using System;
using System.Security.Cryptography;
using System.Text;

namespace Demo.Common.Security
{
    /// <summary>
    /// MD5 加密
    /// </summary>
    public static class MD5Encode
    {
        public static string MD5Encrypt(string pwd)
        {
            if (string.IsNullOrEmpty(pwd)) return string.Empty;
            byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(pwd));
            return BitConverter.ToString(hash).Replace("-", "").ToUpper();
        }
    }
}
