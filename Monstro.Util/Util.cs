using System;
using System.Collections.Generic;
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
    }
}
