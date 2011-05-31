using System;
using System.Collections.Generic;
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
            var rrq = (HttpWebRequest)WebRequest.Create(url);
            rrq.KeepAlive = false;
            rrq.UserAgent = UserAgent;

            var rrs = (HttpWebResponse)rrq.GetResponse();
            var content = new StreamReader(rrs.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            return new JavaScriptSerializer().Deserialize<T>(content);
        }
    }
}
