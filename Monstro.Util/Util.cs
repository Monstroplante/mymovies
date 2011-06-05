using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Monstro.Util
{
    public static class Util
    {
        //Number of seconds elapsed since midnight Coordinated Universal Time (UTC) of January 1, 1970
        public static double GetTimestamp(DateTime d)
        {
            TimeSpan t = (d.ToUniversalTime() - new DateTime(1970, 1, 1).ToUniversalTime());
            return t.TotalSeconds;
        }

        public static double GetTimestamp()
        {
            return GetTimestamp(DateTime.Now);
        }

        public static String CleanFileName(String s)
        {
            String chars = @"\/:*?""<>|";
            foreach (var c in chars)
                s = s.Replace(c, '-');
            return s;
        }

        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public static byte[] StreamToBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }
    }
}
