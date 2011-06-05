using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;

namespace Monstro.Util
{
    public class JsonClient
    {
        private readonly String _userAgent;
        private readonly CookieContainer _cookies = new CookieContainer();
        private readonly DiskCache _cache;
        private DateTime _nextReqMinDate = DateTime.MinValue;
        private readonly int _minDelay;
        private readonly int _maxDelay;
        private readonly int _delayAfterError;
        
        protected JsonClient(DiskCache cache, String userAgent, int minDelay, int maxDelay, int delayAfterError)
        {
            _cache = cache;
            _userAgent = userAgent;
            _minDelay = minDelay;
            _maxDelay = maxDelay;
            _delayAfterError = delayAfterError;
        }

        protected virtual String GetCachedKey(String url)
        {
            return url;
        }

        protected virtual T Call<T>(String url, bool useCache)
        {
            useCache = useCache && _cache != null;
            String cacheKey = useCache ? GetCachedKey(url) : null;

            if(useCache)
            {
                using (var s = _cache.Get(cacheKey))
                {
                    if (s != null)
                        return StreamToObject<T>(s);
                }
            }
            
            try
            {
                int delay = (int)(_nextReqMinDate - DateTime.Now).TotalMilliseconds;
                if (delay > 0)
                    Thread.Sleep(delay);
                var q = (HttpWebRequest)WebRequest.Create(url);
                q.Timeout = 10000;
                q.KeepAlive = true;
                q.UserAgent = _userAgent;
                q.Accept = "gzip";
                q.CookieContainer = _cookies;

                using (var r = (HttpWebResponse)q.GetResponse())
                {
                    var s = r.GetResponseStream();
                    if (r.ContentEncoding == "gzip")
                        s = new GZipStream(s, CompressionMode.Decompress);
                    if (!useCache)
                        return StreamToObject<T>(s);
                    _cache.Add(cacheKey, s);
                    return StreamToObject<T>(_cache.Get(cacheKey));
                }
            }
            catch
            {
                _nextReqMinDate = DateTime.Now.AddSeconds(_delayAfterError);
                throw;
            }
            finally
            {
                //Wait minimum random time betweed requests
                if(_nextReqMinDate < DateTime.Now)
                    _nextReqMinDate = DateTime.Now.AddMilliseconds(_minDelay + new Random().NextDouble() * (_maxDelay - _minDelay));
            }
        }

        private static T StreamToObject<T>(Stream s)
        {
            var content = new StreamReader(s, Encoding.UTF8).ReadToEnd();
            return new JavaScriptSerializer().Deserialize<T>(content);
        }
    }
}
