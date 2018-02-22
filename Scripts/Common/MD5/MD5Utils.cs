using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HFramework
{
    public class MD5Utils
    {
        public static readonly string MD5FileName = "cryptography.txt";

        public static string GetFileHash(string filePath, out string error)
        {
            string fileMD5 = string.Empty;
            error = string.Empty;
            if (File.Exists(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                var result = md5.ComputeHash(fs);
                fs.Close();
                fs.Dispose();

                for (int i = 0; i < result.Length; i++)
                    fileMD5 += Convert.ToString(result[i], 16);
            }
            else
                error = string.Format("Didnt find file:{0}", filePath);
            return fileMD5;
        }
    }
}
