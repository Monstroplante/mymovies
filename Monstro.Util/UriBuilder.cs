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
        readonly List<KeyValuePair<String, String>> _params = new List<KeyValuePair<String, String>>();
        private readonly String _url;

        public UrlBuilder(String url)
        {
            _url = url;
        }

        public UrlBuilder PutAll(params KeyValuePair<String, String>[] args)
        {
            _params.AddRange(args);
            return this;
        }

        public UrlBuilder Put(String name, String value)
        {
            if (String.IsNullOrEmpty(name))
                return this;
            _params.Add(new KeyValuePair<String, String>(name, value));;
            return this;
        }

        public UrlBuilder Put(String name, double? value)
        {
            return Put(name, value == null ? null : value.ToString());
        }

        public String ToString(bool ignoreEmptyParams)
        {
            var url = (_url ?? "").Trim();
            if (url.EndsWith("?"))
                url = url.Substring(0, url.Length - 1);
            return url + (url.Contains('?') ? '&' : '?') + GetQueryString(ignoreEmptyParams);
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public String GetQueryString(bool ignoreEmptyParams)
        {
            return _params.ConvertAll(kv =>
            {
                if (ignoreEmptyParams && kv.Value.IsNullOrEmpty())
                    return null;
                return kv.Key + "=" + UrlEncode(kv.Value);
            }).Where(s => !s.IsNullOrEmpty()).Join("&");
        }

        public String GetQueryString()
        {
            return GetQueryString(true);
        }

        public static String UrlEncode(String s)
        {
            if (String.IsNullOrEmpty(s))
                return "";
            return HttpUtility.UrlEncode(s).Replace("+", "%20");
        }
    }
}