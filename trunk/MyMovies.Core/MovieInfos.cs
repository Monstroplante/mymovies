using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class MovieInfos
    {
        public String GuessedTitle { get; private set; }
        public int? GuessedYear { get; private set; }
        public String Path{ get; private set;}
        public bool SeamsDuplicated { get; private set; }
        private bool _ignore;

        public bool ShouldBeIgnored
        {
            get
            {
                return _ignore || GuessedTitle.IsNullOrEmpty();
            }
        }

        public MovieInfos(String title, int? year, String path, bool seamsDuplicated, bool ignore)
        {
            GuessedTitle = title;
            GuessedYear = year;
            Path = path;
            SeamsDuplicated = seamsDuplicated;
            _ignore = ignore;
        }

        public override string ToString()
        {
            return Path + "\n>> " + new[]{
                ShouldBeIgnored ? "IGNORE" : null,
                GuessedTitle,
                GuessedYear == null ? null : GuessedYear.ToString(),
                SeamsDuplicated ? "SeamsDuplicated" : null
            }.Where(s => s != null)
            .Join(" - ");
        }
    }
}