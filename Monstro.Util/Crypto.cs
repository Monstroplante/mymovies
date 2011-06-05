using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Monstro.Util
{
    public static class Crypto
    {
        public static string HMACSHA1(String input, String key)
        {
            var hash = new HMACSHA1(Encoding.UTF8.GetBytes(key)).ComputeHash(Encoding.UTF8.GetBytes(input));
            return Hex(hash);
        }

        public static String Hex(byte[] data)
        {
            return Hex(data, 0, data.Length);
        }

        private static String Hex(byte[] data, int offset, int length)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i != length; ++i)
            {
                s.Append(String.Format("{0:x2}", data[i + offset]));
            }
            return s.ToString();
        }

        public static String MD5Hex(String s)
        {
            byte[] input = Encoding.UTF8.GetBytes(s);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(input);
            return Hex(output);
        }

        public static String MD5Hex(Stream s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(s);
            return Hex(output);
        }
    }
}
