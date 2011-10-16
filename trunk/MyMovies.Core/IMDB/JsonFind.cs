using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper.IMDB
{
    public class JsonFind
    {
        public Data data { get; set; }
        public string @type { get; set; }
        public string copyright { get; set; }

        public class Image
        {
            public int width { get; set; }
            public string url { get; set; }
            public int height { get; set; }
        }

        public class Principal
        {
            public string nconst { get; set; }
            public string name { get; set; }
        }

        public class List
        {
            public string tconst { get; set; }
            public string title { get; set; }
            public string type { get; set; }
            public string year { get; set; }
            public Image image { get; set; }
            public Principal[] principals { get; set; }
            public string nconst { get; set; }
            public string name { get; set; }
            public string known_for { get; set; }

            public String PrincipalsString
            {
                get
                {
                    if (principals == null || !principals.Any())
                        return null;
                    return "With: " + principals.Select(p => p.name).Join(" - ");
                }
            }

            public String TitleYear
            {
                get
                {
                    String s = title;
                    if (!year.IsNullOrEmpty())
                        s += " - " + year;
                    return s;
                }
            }

            public int? GetYear()
            {
                if (year.IsNullOrEmpty())
                    return null;
                try
                {
                    return int.Parse(year);
                }
                catch
                {
                    return null;
                }
            }
        }

        public class Result
        {
            public string label { get; set; }
            public List[] list { get; set; }
        }

        public class Data
        {
            public string[] fields { get; set; }
            public string q { get; set; }
            public Result[] results { get; set; }
        }
    }

}
