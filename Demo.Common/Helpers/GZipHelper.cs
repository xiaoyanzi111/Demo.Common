using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Demo.Common.Helpers
{
    /// <summary>
    /// GZip 压缩/解压助手
    /// </summary>
    public static class GZipHelper
    {
        /// <summary>
        /// 压缩字符串
        /// </summary>
        public static string Compress(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            byte[] raw = Encoding.UTF8.GetBytes(input);
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(raw, 0, raw.Length);
            }
            return Convert.ToBase64String(output.ToArray());
        }

        /// <summary>
        /// 解压字符串
        /// </summary>
        public static string Decompress(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            byte[] raw = Convert.FromBase64String(input);
            using var inputStream = new MemoryStream(raw);
            using var gzip = new GZipStream(inputStream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
