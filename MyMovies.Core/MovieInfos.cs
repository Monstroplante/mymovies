using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class MovieInfos
    {
        public String Title { get; private set; }
        public int? Year { get; private set; }
        public String Path{ get; private set;}
        public bool SeamsDuplicated { get; private set; }
        public List<MovieInfos> Duplicated { get; private set; }
        private bool _ignore;

        public bool ShouldBeIgnored
        {
            get
            {
                return _ignore || Title.IsNullOrEmpty();
            }
        }

        public MovieInfos(String title, int? year, String path, bool seamsDuplicated, bool ignore)
        {
            Title = title;
            Year = year;
            Path = path;
            SeamsDuplicated = seamsDuplicated;
            _ignore = ignore;
            Duplicated = new List<MovieInfos>();
        }

        public override string ToString()
        {
            return Path + "\n>> " + new[]{
                ShouldBeIgnored ? "IGNORE" : null,
                Title,
                Year == null ? null : Year.ToString(),
                SeamsDuplicated ? "SeamsDuplicated" : null
            }.Where(s => s != null)
            .Join(" - ");
        }

        internal void Ignore()
        {
            _ignore = true;
        }
    }
}