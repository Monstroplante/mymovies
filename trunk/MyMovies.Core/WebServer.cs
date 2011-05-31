using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
                
                //System.Diagnostics.Processes.Start("Gobias Industries Business Plan.docx"

                String uri = request.Uri;

                if(uri.EndsWith("/"))
                    uri += "index.htm";
                    
                String file = Path.Combine(root, uri.Substring(1).Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(file))
                {
                    var ext = (Path.GetExtension(file) ?? ".").Substring(1).ToLower();
                    String contentType = ContentTypes.GetOrDefault(ext, "text/plain");
                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data = new byte[fs.Length];
                        fs.Read(data, 0, Convert.ToInt32(fs.Length));

                        response.OnResponse(new HttpResponseHead(){
                            Status = "200 OK",
                            Headers = new Dictionary<string, string>() {
                                { "Content-Type", contentType },
                                { "Content-Length", fs.Length.ToString() },
                            }
                        }, new BufferedBody(data));
                        return;
                    }
                }

                //404
                var responseBody = "The resource you requested ('" + request.Uri + "') could not be found.";
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
            ArraySegment<byte> data;

            public BufferedBody(string data) : this(data, Encoding.UTF8) { }
            public BufferedBody(string data, Encoding encoding) : this(encoding.GetBytes(data)) { }
            public BufferedBody(byte[] data) : this(new ArraySegment<byte>(data)) { }
            public BufferedBody(ArraySegment<byte> data)
            {
                this.data = data;
            }

            public IDisposable Connect(IDataConsumer channel)
            {
                // null continuation, consumer must swallow the data immediately.
                channel.OnData(data, null);
                channel.OnEnd();
                return null;
            }
        }
    }
}
