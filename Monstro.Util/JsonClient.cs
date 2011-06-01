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
        private const String UserAgent = "Personal use, thank you :)";

        protected T Request<T>(UrlBuilder url)
        {
            return Request<T>(url.ToString());
        }

        protected T Request<T>(String url)
        {
            var q = (HttpWebRequest)WebRequest.Create(url);
            q.KeepAlive = false;
            q.UserAgent = UserAgent;
            q.Headers["Accept-Encoding"] = "gzip";

            var r = (HttpWebResponse)q.GetResponse();
            var s = r.ContentEncoding == "gzip"
                ? new GZipStream(r.GetResponseStream(), CompressionMode.Decompress)
                : r.GetResponseStream();
            var content = new StreamReader(s, Encoding.UTF8).ReadToEnd();
            return new JavaScriptSerializer().Deserialize<T>(content);
        }
    }
}
