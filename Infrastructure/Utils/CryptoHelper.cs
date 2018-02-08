using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Utils
{
    public class CryptoHelper
    {
        public static string MD5_Encrypt(string source, string addKey, Encoding encoding)
        {
            var md5 = MD5.Create();
            var bytes = md5.ComputeHash(encoding.GetBytes(source + addKey));

            var byte2String = new StringBuilder();
            //字节类型的数组转换为字符串
            for (int i = 0; i < bytes.Length; i++)
            {
                //16进制转换
                var thisByte = bytes[i].ToString("x");
                if (thisByte.Length == 1)
                    thisByte = "0" + thisByte;
                byte2String.Append(thisByte);
            }

            return byte2String.ToString();
        }

        public static string MD5_Encrypt(string source, string addKey = "")
        {
            return MD5_Encrypt(source, addKey, Encoding.Default);
        }
    }
}
