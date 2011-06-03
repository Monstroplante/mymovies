using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Kayak;
using Kayak.Http;
using System.Net;
using System.Threading;
using System.IO;
using System.Reflection;
using Monstro.Util;

namespace MyMovies.Core
{
    public static class WebServer
    {
        static Thread thread;
        public static String RootDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "www");
        static Log log = new Log("www");
        private static int Port;

        public static bool IsRunning
        {
            get { return thread != null; }
        }

        public static Dictionary<String, String> ContentTypes = new Dictionary<String, String>{
            {"css", "text/css"},
            {"htm", "text/html"},
            {"html", "text/html"},
            {"txt", "text/plain"},
            {"jpeg", "image/jpeg"},
            {"jpe", "image/jpeg"},
            {"jpg", "image/jpeg"},
            {"png", "image/png"},
            {"gif", "image/gif"},
            {"js", "application/javascript"}
        };

        static public void Start(int port)
        {
            Port = port;
            Stop();
            var scheduler = new KayakScheduler(new SchedulerDelegate());
            scheduler.Post(() =>
            {
                KayakServer.Factory
                    .CreateHttp(new RequestDelegate())
                    .Listen(new IPEndPoint(IPAddress.Any, port));
            });
            thread = new Thread(() => scheduler.Start());
            thread.Start();
        }

        static public void Stop()
        {
            if (thread != null)
                thread.Abort();
            thread = null;
        }

        class SchedulerDelegate : ISchedulerDelegate
        {
            public void OnException(IScheduler scheduler, Exception e)
            {
                Debug.WriteLine("Error on scheduler.");
                e.DebugStacktrace();
            }

            public void OnStop(IScheduler scheduler)
            {

            }
        }

        class RequestDelegate : IHttpRequestDelegate
        {
            public void OnRequest(HttpRequestHead request, IDataProducer requestBody,
                IHttpResponseDelegate response)
            {
                log.Info(new[] { request.Method, request.Uri}.Join("\t"));

                String url = request.Uri;
                var parts = url.Split(new[]{'?'}, 2);
                String path = HttpUtility.UrlDecode(parts[0]);

                if (path.EndsWith("/"))
                    path += "index.htm";

                String file = Path.Combine(RootDir, path.Substring(1).Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(file))
                {
                    var ext = (Path.GetExtension(file) ?? ".").Substring(1).ToLower();
                    String contentType = ContentTypes.GetOrDefault(ext, "text/plain");

                    DoResponse(request, response, contentType, File.ReadAllBytes(file));
                    return;
                }

                var o = HttpUtility.ParseQueryString(parts.GetOrDefault(1) ?? "");
                if(path == "/*movies")
                {
                    DoJsonpResponse(request, response, o, DM.Instance.GetJson());
                    return;
                }

                if(path == "/*play")
                {
                    var f = o["f"];
                    if (DM.Instance.GetMovieByFile(f) != null)
                    {
                        Process.Start(f);
                        DoJsonpResponse(request, response, o, "{success:true}");
                        return;
                    }
                }

                //404
                DoResponse(request, response, "text/plain",
                    "The resource you requested ('" + path + "') could not be found.");
            }
        }

        private static void DoJsonpResponse(HttpRequestHead request, IHttpResponseDelegate response, NameValueCollection o, String json)
        {
            var cb = o["jsonp"];
            DoResponse(request, response, "application/x-javascript; charset=utf-8", cb.IsNullOrEmpty()
                ? json
                : String.Format("{0}({1})", cb, json));
        }

        private static void DoResponse(HttpRequestHead request, IHttpResponseDelegate response, String contentType, byte[] data)
        {
            var body = new BufferedBody(data);
            var headers = new Dictionary<string, string>{
                { "Content-Type", contentType },
                { "Content-Length", body.Length.ToString() },
            };
            response.OnResponse(new HttpResponseHead
            {
                Status = "200 OK",
                Headers = headers
            }, body);
        }

        private static void DoResponse(HttpRequestHead request, IHttpResponseDelegate response, String contentType, String data)
        {
            DoResponse(request, response, contentType, Encoding.UTF8.GetBytes(data));
        }

        class BufferedBody : IDataProducer
        {
            byte[] data;
            public int Length { get { return data.Length; } }
            public BufferedBody(byte[] data)
            {
                this.data = data;
            }

            public IDisposable Connect(IDataConsumer channel)
            {
                // null continuation, consumer must swallow the data immediately.
                channel.OnData(new ArraySegment<byte>(data), null);
                channel.OnEnd();
                return null;
            }
        }

        public static String GetHomeUrl()
        {
            return String.Format("http://{0}:{1}", Network.GetLocalIp(), Port);
        }
    }
}
