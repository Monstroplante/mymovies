using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace Monstro.Util
{
    public class JsonClient
    {
        protected String UserAgent = null;
        private CookieContainer _cookies = new CookieContainer();

        protected virtual T Call<T>(UrlBuilder url)
        {
            return Call<T>(url.ToString());
        }

        protected virtual T Call<T>(String url)
        {
            var q = (HttpWebRequest)WebRequest.Create(url);
            q.Timeout = 10000;
            q.KeepAlive = true;
            q.UserAgent = UserAgent;
            q.Accept = "gzip";
            q.CookieContainer = _cookies;

            using (var r = (HttpWebResponse)q.GetResponse())
            {
                var s = r.ContentEncoding == "gzip"
	                ? new GZipStream(r.GetResponseStream(), CompressionMode.Decompress)
	                : r.GetResponseStream();
                var content = new StreamReader(s, Encoding.UTF8).ReadToEnd();
                return new JavaScriptSerializer().Deserialize<T>(content);
            }
        }
    }
}
