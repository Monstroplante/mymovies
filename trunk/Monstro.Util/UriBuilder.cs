using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Linq;

namespace Monstro.Util
{
    public class UrlBuilder
    {
        readonly Dictionary<String, String> _params = new Dictionary<String, String>();
        private readonly String _url;

        public UrlBuilder(String url)
        {
            _url = url;
        }

        public UrlBuilder Put(String name, String value)
        {
            if (String.IsNullOrEmpty(name))
                return this;
            _params[name] = value;
            return this;
        }

        public UrlBuilder Put(String name, int? value)
        {
            return Put(name, value == null ? null : value.ToString());
        }

        public String ToString(bool ignoreEmptyParams)
        {
            var url = (_url ?? "").Trim();
            if (url.EndsWith("?"))
                url = url.Substring(0, url.Length - 1);
            return url + (url.Contains('?') ? '&' : '?') + _params.ConvertAll(kv =>
            {
                if (ignoreEmptyParams && kv.Value.IsNullOrEmpty())
                    return null;
                return kv.Key + "=" + UrlEncode(kv.Value);
            }).Where(s => !s.IsNullOrEmpty()).Join("&");
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public static String UrlEncode(String s)
        {
            if (String.IsNullOrEmpty(s))
                return "";
            return HttpUtility.UrlEncode(s).Replace("+", "%20");
        }
    }
}