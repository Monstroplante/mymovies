using System;
using System.Collections.Generic;
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

namespace MyMovies.Core
{
    public static class WebServer
    {
        static Thread thread;
        static String root = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "www");
        static Log log = new Log("www");

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
                //System.Diagnostics.Processes.Start("Gobias Industries Business Plan.docx"

                String url = request.Uri;
                var parts = url.Split(new[]{'?'}, 2);
                String path = parts[0];

                if (path.EndsWith("/"))
                    path += "index.htm";

                String file = Path.Combine(root, path.Substring(1).Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(file))
                {
                    var ext = (Path.GetExtension(file) ?? ".").Substring(1).ToLower();
                    String contentType = ContentTypes.GetOrDefault(ext, "text/plain");

                    var data = new BufferedBody(File.ReadAllBytes(file));

                    response.OnResponse(new HttpResponseHead(){
                        Status = "200 OK",
                        Headers = new Dictionary<string, string>() {
                            { "Content-Type", contentType },
                            { "Content-Length", data.Length.ToString() },
                        }
                    }, data);
                    return;
                }

                var o = HttpUtility.ParseQueryString(parts.GetOrDefault(1) ?? "");
                if(path == "/*movies")
                {
                    var json = DM.Instance.GetJson();
                    var jsonp = o["jsonp"];
                    if (!jsonp.IsNullOrEmpty())
                        json = String.Format("{0}({1})", jsonp, json);

                    var data = new BufferedBody(json);
                    response.OnResponse(new HttpResponseHead{
                        Status = "200 OK",
                        Headers = new Dictionary<string, string>{
                            { "Content-Type", "application/x-javascript; charset=utf-8" },
                            { "Content-Length", data.Length.ToString() },
                        }
                    }, data);
                    return;
                }

                //404
                var responseBody = "The resource you requested ('" + path + "') could not be found.";
                response.OnResponse(new HttpResponseHead()
                {
                    Status = "404 Not Found",
                    Headers = new Dictionary<string, string>(){
                            { "Content-Type", "text/plain" },
                            { "Content-Length", responseBody.Length.ToString() }
                        }
                }, new BufferedBody(responseBody));
            }
        }

        class BufferedBody : IDataProducer
        {
            byte[] data;
            public int Length { get { return data.Length; } }

            public BufferedBody(string data) : this(Encoding.UTF8.GetBytes(data)) { }
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
    }
}
