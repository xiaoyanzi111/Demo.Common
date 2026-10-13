using System.Security.Cryptography;
using System.Text;

namespace Demo.Common.Security
{
    /// <summary>
    /// RSA 加解密与签名工具（默认使用 PKCS#8 / PEM）
    /// </summary>
    public static class RSAEncode
    {
        /// <summary>
        /// 生成 RSA 密钥对（返回 PEM 格式的 public/private）
        /// </summary>
        public static (string PublicKey, string PrivateKey) GenerateKeyPair(int keySize = 2048)
        {
            using var rsa = RSA.Create(keySize);
            return (rsa.ExportRSAPublicKeyPem(), rsa.ExportRSAPrivateKeyPem());
        }

        /// <summary>
        /// 公钥加密（Base64 输出）
        /// </summary>
        public static string Encrypt(string plainText, string publicKeyPem)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);
            byte[] data = rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.OaepSHA256);
            return System.Convert.ToBase64String(data);
        }

        /// <summary>
        /// 私钥解密
        /// </summary>
        public static string Decrypt(string cipherText, string privateKeyPem)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);
            byte[] data = rsa.Decrypt(System.Convert.FromBase64String(cipherText), RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// 私钥签名（Base64 输出）
        /// </summary>
        public static string Sign(string content, string privateKeyPem)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);
            byte[] signature = rsa.SignData(Encoding.UTF8.GetBytes(content), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return System.Convert.ToBase64String(signature);
        }

        /// <summary>
        /// 公钥验签
        /// </summary>
        public static bool Verify(string content, string signature, string publicKeyPem)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);
            return rsa.VerifyData(Encoding.UTF8.GetBytes(content), System.Convert.FromBase64String(signature),
                HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
