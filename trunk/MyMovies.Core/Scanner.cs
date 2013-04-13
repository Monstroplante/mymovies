using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Helper.IMDB;
using Monstro.Util;
using MyMovies.Core;

namespace Helper
{
    public static class Scanner
    {
        static readonly String[] ext = new[] { "avi", "mkv", "m4v", "mp4", "ifo", "divx" };
        private const int MinFileSize = 100 * 1024 * 1024;//100Mo

        private const RegexOptions CompiledIgnoreCase = RegexOptions.Compiled | RegexOptions.IgnoreCase;
        private const String Keywords = @"\b(divx\d?|mkv|xvid|dvdrip|dvd|french|vo|vf|cd\W?\d|720p|1080p|avi|BluRay|x264|H264|bdrip|brrip|aac|hd|2 Ch|fr|vost[a-z]{0,2}|rip)\b";
        static readonly Regex RegKeywords = new Regex(Keywords, CompiledIgnoreCase);
        static readonly Regex RegExtractTitleAndYear = new Regex(@"^(.+?)\b((?:19|2[0o])[0-9o]{2})\b", CompiledIgnoreCase);
        static readonly Regex RegSerial = new Regex(String.Format(@"^(.+?)\b(?:{0})\b", new[]{
            @"s(?<s>\d{1,2})e\d{1,3}",
            @"(?<s>\d{1,2})x\d{1,3}",
            @"s(?<s>\d{1,2})",
            @"(season|saison)\W?(?<s>\d{1,2})",
            @"\s*[-.]\s*(?<s>0?\d)\d\d\b",
            @"(?<s>\d)\d\d",
            @"(?<s>[01][0-8])\d\d",
        }.Join("|")), CompiledIgnoreCase);
        static readonly Regex RegExtractSerialFullPath = new Regex(@"(^|/)(?<n>[^/]{3,}?)\b(((season|saison)\W*(?<s>\d{1,2})|s(?<s>\d{1,2}))|Complete)\b", CompiledIgnoreCase);
        static readonly Regex RegExtractBeforeKeyword = new Regex(@"^(.+?)" + Keywords, CompiledIgnoreCase);
        static readonly Regex RegCleanup = new Regex("(" + new[]{
            @"\[.+?\]",
            @"\{.+?\}",
            @"\(.+?\)",
            @"^[.\-, ]+",
            @"[.\-, ]+$",
            @"[\.\[\]()_]",
            }.Join("|") + ")", CompiledIgnoreCase);
        static readonly Regex RegCleanup2 = new Regex(@"\s+", CompiledIgnoreCase);
        static readonly Regex RegTeams = new Regex(
            @"\b(aymo|ALLiANCE|CiNEFiLE|REFiNED|LiMiTED|^final|CiRCLE|^aaf|sample|SAMPLE|Sample|DiAMOND|LIMITED|UNRATED|UsaBit\.com)\b", RegexOptions.Compiled);
        static readonly Regex RegSeamsDuplicated = new Regex(@"\b(sample|cd\W?[2-9])\b", CompiledIgnoreCase);
        static readonly Regex RegBlackList = new Regex(@"(2 - Video à voir EN 2|\\sample\\|Sample)", CompiledIgnoreCase);

        public static IEnumerable<String> GetFiles(String path)
        {
            String[] dirs = null;
            String[] files = null;
            try
            {
                files = Directory.GetFiles(path, "*");
            }catch { }

            try
            {
                dirs = Directory.GetDirectories(path);
            }catch{ }

            foreach (var d in dirs.NoNull())
                foreach (var f in GetFiles(d))
                    yield return f;
            foreach (var f in files.NoNull()
                .Where(f => (f.Split('.').LastOrDefault() ?? "").ToLower().In(ext) && new FileInfo(f).Length > MinFileSize))
                yield return f;
            
        }

        public static MovieInfos ParseMovieName(String path)
        {
            var p = (path ?? "").Replace('\\', '/');
            bool seeamsDuplicated = RegSeamsDuplicated.IsMatch(p);
            String f = null;
            int? y = null;

            Match m = RegExtractSerialFullPath.Match(p);
            if(m.Success)
            {
                f = m.Groups["n"].Value;
            }
            else
            {
                var parts = p.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Reverse().ToArray();


                String file = RemoveFileExt(parts[0]);

                //Try to determine if parent isbetter candidate
                String parent = parts.GetOrDefault(1);
                if (parent == "VIDEO_TS")
                    f = parts.GetOrDefault(2);

                if (f.IsNullOrEmpty())
                {
                    int parentMatches = GetKwMathCount(parent);
                    f = parentMatches >= 2 && parentMatches > GetKwMathCount(file)
                        ? parent : file;
                }

                f = (f ?? "").Replace('_', ' ');

                m = RegSerial.Match(f);
                if (m.Success)
                    f = m.Groups[1].Value;

                m = RegExtractTitleAndYear.Match(f);
                if (m.Success)
                {
                    f = m.Groups[1].Value;
                    y = int.Parse(m.Groups[2].Value.ToLower().Replace('o', '0'));
                }
            }

            m = RegExtractBeforeKeyword.Match(f);
            if (m.Success)
                f = m.Groups[1].Value;

            return new MovieInfos(Cleanup(f), y, path, seeamsDuplicated, RegBlackList.IsMatch(p));
        }

        private static int GetKwMathCount(String s)
        {
            if (s.IsNullOrEmpty())
                return 0;
            return
                (RegExtractTitleAndYear.IsMatch(s) ? 1 : 0)
                + RegKeywords.Matches(s).Count
                + RegTeams.Matches(s).Count;
        }

        public static String Cleanup(String s)
        {
            s = RegTeams.Replace(s, "");
            s = RegKeywords.Replace(s, "");
            s = RegCleanup.Replace(s, " ");
            s = RegCleanup2.Replace(s, " ");

            //Replace strange accents
            s = s.Replace("è", "è");
            s = s.Replace("é", "é");
            return s.Trim();
        }

        public static String RemoveFileExt(String f)
        {
            if(f.IsNullOrEmpty())
                return f;
            int pos = f.LastIndexOf('.');
            return pos > 0
                ? f.Substring(0, pos) : f;
        }

        public class NoMatchFoundException : Exception{}

        public static Movie FetchMovie(String file)
        {
            var imdb = new IMDBClient();
            var f = ParseMovieName(file);

            if (f.ShouldBeIgnored || f.GuessedTitle.IsNullOrEmpty())
                return null;

            var m = imdb.Find(f.GuessedTitle + " " + f.GuessedYear).FirstOrDefault();
            if (m == null)
                throw new NoMatchFoundException();

            return FetchMovie(file, m.tconst, false);
        }

        public static Movie FetchMovie(String file, String imdbId, bool allowUnpopular)
        {
            var imdb = new IMDBClient();
            var m = imdb.GetDetails(imdbId);

            if (!allowUnpopular && m.num_votes < 50)
                throw new NoMatchFoundException();

            String cover = null;
            if (m.image != null && !m.image.url.IsNullOrEmpty())
            {
                cover = Util.CleanFileName(m.image.url);
                var coverPath = Path.Combine(DM.CoverDir, cover);
                if (!File.Exists(coverPath))
                {
                    if (!Directory.Exists(DM.CoverDir))
                        Directory.CreateDirectory(DM.CoverDir);
                    new WebClient().DownloadFile(m.image.url, coverPath);
                }
            }

            return new Movie(file, m, cover);
        }
    }
}
