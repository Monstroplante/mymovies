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
                var bytes = _cache.Get(cacheKey);
                if(bytes != null)
                    return BytesToObject<T>(bytes);
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
                    using (var s = r.GetResponseStream())
                    {
                        byte[] bytes;
                        if (r.ContentEncoding == "gzip")
                        {
                            using (var gs = new GZipStream(s, CompressionMode.Decompress))
                            {
                                bytes = Util.StreamToBytes(gs);
                            }
                        }
                        else
                        {
                            bytes = Util.StreamToBytes(s);
                        }
                        if (useCache)
                            _cache.Add(cacheKey, bytes);
                        return BytesToObject<T>(bytes);
                    }
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

        private static T BytesToObject<T>(byte[] bytes)
        {
            return new JavaScriptSerializer().Deserialize<T>(Encoding.UTF8.GetString(bytes));
        }
    }
}
