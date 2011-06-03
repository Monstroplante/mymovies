using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMovies.Core.IMDB
{
    public class JsonHello
    {
        public Data data { get; set; }
        public string copyright { get; set; }

        public class Data
        {
            public int time { get; set; }
            public string status { get; set; }
            public string api_key { get; set; }
        }
    }
}
