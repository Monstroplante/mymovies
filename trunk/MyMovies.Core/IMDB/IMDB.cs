using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Monstro.Util;
using MyMovies.Core.IMDB;
using System.Threading;

namespace Helper.IMDB
{
    public class IMDB : JsonClient
    {
        public String Locale{get; private set;}
        static readonly Regex RegExtractTitleAndYear = new Regex(@"^(.+?)\b((?:1|2)[0-9o]{3})\s*$", RegexOptions.Compiled);
        
        public IMDB() : this(Thread.CurrentThread.CurrentCulture.Name){

            }

        public IMDB(String locale) : base(){
            Locale = (locale ?? "fr_FR").Replace("-", "_");
        }

        public List<JsonFind.List> Find(String q)
        {
            //Try to extract year from query
            int? year = null;
            var match = RegExtractTitleAndYear.Match(q);
            if (match.Success)
            {
                q = match.Groups[1].Value;
                year = int.Parse(match.Groups[2].Value.ToLower().Replace('o', '0'));
            }
            var r = Request<JsonFind>(GetUrlBuilder("find").Put("q", q))
                .data.results.NoNull().SelectMany(e => e.list);
            if (year != null)
                r = r.OrderBy(m => m.GetYear() == null ? int.MaxValue : Math.Abs((m.GetYear() - year) ?? 0));
            return r.ToList();
        }

        public JsonMainDetails.Data GetDetails(String id)
        {
            return Request<JsonMainDetails>(GetUrlBuilder("title/maindetails").Put("tconst", id))
                .data;
        }

        private UrlBuilder GetUrlBuilder(String function)
        {
            return new UrlBuilder("http://app.imdb.com/" + function)
                .Put("api", "v1")
                .Put("appid", "iphone1")
                .Put("locale", Locale);
        }
    }
}