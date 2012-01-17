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
using MyMovies.Core;
using MyMovies.Core.IMDB;
using System.Threading;

namespace Helper.IMDB
{
    public class IMDBClient : JsonClient
    {
        private readonly String DeviceId;
        private const String AppId = "android2";
        public const String Key = "eRnAYqbvj2JWXyPcu62yCA";
        private const String BaseUrl = "http://app.imdb.com/";

        public String Locale{get; private set;}
        static readonly Regex RegExtractTitleAndYear = new Regex(@"^(.+?)\b((?:1|2)[0-9o]{3})\s*$", RegexOptions.Compiled);
        
        public IMDBClient() : this(Thread.CurrentThread.CurrentCulture.Name){}

        public IMDBClient(String locale) : base(new DiskCache(DM.GetLocalFilePath("httpcache")), null, 0, 0, 10000){
            Locale = (locale ?? "fr_FR").Replace("-", "_");
            if(DeviceId == null)
            {
                DeviceId = Guid.NewGuid().ToString();
                SayHello();
            }
        }

        private void SayHello()
        {
            Call<JsonHello>("hello", false,
                KV("app_version", "1.5"),
                KV("device_model", "Nexus One"),
                KV("count", "0"),
                KV("system_version", "10"),
                KV("system_name", "google"));
        }

        private KeyValuePair<String, String> KV(String k, String v)
        {
            return new KeyValuePair<String, String>(k,v);
        }

        public List<JsonFind.List> Find(String q)
        {
            q = (q ?? "").Trim();
            //Try to extract year from query
            int? year = null;
            var match = RegExtractTitleAndYear.Match(q);
            if (match.Success)
            {
                q = match.Groups[1].Value;
                year = int.Parse(match.Groups[2].Value.ToLower().Replace('o', '0'));
            }
            var r = Call<JsonFind>("find", KV("q", q))
                .data.results.NoNull().SelectMany(e => e.list)
                .Where(m => !m.title.IsNullOrEmpty() && !m.tconst.IsNullOrEmpty());
            if (year != null)
                r = r.OrderBy(m => m.GetYear() == null ? int.MaxValue : Math.Max(1, Math.Abs((m.GetYear() - year) ?? 0)));
            return r.ToList();
        }

        public JsonMainDetails.Data GetDetails(String id)
        {
            return Call<JsonMainDetails>("title/maindetails", KV("tconst", id))
                .data;
        }

        private T Call<T>(String function, bool useCache, params KeyValuePair<String, String>[] args)
        {
            return base.Call<T>(SignUrl(new UrlBuilder(BaseUrl + function)
                .PutAll(args)
                .Put("appid", AppId)
                .Put("device", DeviceId)
                .Put("locale", Locale)),
                useCache);
        }

        private T Call<T>(String function, params KeyValuePair<String, String>[] args)
        {
            return Call<T>(function, true, args);
        }

        protected static String SignUrl(UrlBuilder b)
        {
            return SignUrl(b, Util.GetTimestamp());
        }

        /// <param name="timestamp">Usefull for tests</param>
        public static String SignUrl(UrlBuilder b, double timestamp)
        {
            b.Put("timestamp", timestamp);
            b.Put("sig", "and2");
            String url = b.ToString();
            return url + "-" + Crypto.HMACSHA1(url, Key);
        }

        protected override string GetCachedKey(string url)
        {
            var ignoredParams = new[] {"appid", "device", "timestamp", "sig"};
            var parts = url.Split(new[] {'?', '&'});
            return parts[0] + '?' + parts.Skip(1)
                .Where(s => !s.Split('=')[0].In(ignoredParams))
                .Join("&");
        }
    }
}