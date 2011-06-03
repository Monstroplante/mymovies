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
        private readonly String DeviceId = Guid.NewGuid().ToString();
        private const String AppId = "android2";
        public const String Key = "eRnAYqbvj2JWXyPcu62yCA";
        private const String BaseUrl = "http://app.imdb.com/";

        public String Locale{get; private set;}
        static readonly Regex RegExtractTitleAndYear = new Regex(@"^(.+?)\b((?:1|2)[0-9o]{3})\s*$", RegexOptions.Compiled);
        
        public IMDB() : this(Thread.CurrentThread.CurrentCulture.Name){}

        public IMDB(String locale) : base(){
            Locale = (locale ?? "fr_FR").Replace("-", "_");
            SayHello();
        }

        private void SayHello()
        {
            Call<JsonHello>("hello",
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
                r = r.OrderBy(m => m.GetYear() == null ? int.MaxValue : Math.Abs((m.GetYear() - year) ?? 0));
            return r.ToList();
        }

        public JsonMainDetails.Data GetDetails(String id)
        {
            return Call<JsonMainDetails>("title/maindetails", KV("tconst", id))
                .data;
        }

        private DateTime _nextReqMinDate = DateTime.MinValue;
        private T Call<T>(String function, params KeyValuePair<String, String>[] args)
        {
            int delay = (int)(_nextReqMinDate - DateTime.Now).TotalMilliseconds;
            if (delay > 0)
                Thread.Sleep(delay);

            try
            {
                var o = base.Call<T>(SignUrl(new UrlBuilder(BaseUrl + function)
                    .PutAll(args)
                    .Put("appid", AppId)
                    .Put("device", DeviceId)
                    .Put("locale", Locale)
                    .Put("timestamp", Util.GetTimestamp())
                    .ToString()));

                //Wait minimum random time betweed requests
                _nextReqMinDate = DateTime.Now.AddSeconds(new Random().NextDouble() * 2);
                return o;
            }
            catch(WebException ex)
            {
                //Wait minimum 30s after an error
                _nextReqMinDate = DateTime.Now.AddSeconds(30);
                throw;
            }
        }

        public static String SignUrl(String url)
        {
            url += "&sig=and2";
            return url + "-" + Crypto.HMACSHA1(url, Key);
        }
    }
}