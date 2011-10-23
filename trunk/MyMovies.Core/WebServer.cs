using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Helper;
using Helper.IMDB;
using JsonExSerializer;
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
        public static String WebDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "www");
        public static String ScaledDir = Path.Combine(DM.CoverDir, "scaled");
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
            var scheduler = KayakScheduler.Factory.Create(new SchedulerDelegate());
            scheduler.Post(() => KayakServer.Factory
                .CreateHttp(new RequestDelegate(), scheduler)
                .Listen(new IPEndPoint(IPAddress.Any, port))
            );
            thread = new Thread(scheduler.Start);
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
                e.DebugStackTrace();
            }

            public void OnStop(IScheduler scheduler)
            {

            }
        }

        class RequestDelegate : IHttpRequestDelegate
        {
            private static readonly Regex RegScaleFormat = new Regex(@"^(\d*)x(\d*)([cs]*)$", RegexOptions.Compiled);

            private static String QueryPathToFile(String queryPath)
            {
                queryPath = (queryPath ?? "").Replace('/', Path.DirectorySeparatorChar);
                if(queryPath.StartsWith(Path.DirectorySeparatorChar.ToString()))
                    queryPath = queryPath.Substring(1);
                return Path.Combine(WebDir, queryPath);
            }

            public void OnRequest(HttpRequestHead request, IDataProducer requestBody,
                IHttpResponseDelegate response)
            {
                try
                {
                    log.Info(new[] { request.Method, request.Uri}.Join("\t"));

                    String url = request.Uri;
                    var parts = url.Split(new[]{'?'}, 2);
                    String path = HttpUtility.UrlDecode(parts[0]);

                    if (path.EndsWith("/"))
                        path += "index.htm";

                    if (ReplyFile(response, QueryPathToFile(path)))
                        return;

                    var o = HttpUtility.ParseQueryString(parts.GetOrDefault(1) ?? "");
                    if(path == "/*movies")
                    {
                        ReplyJson(response, o, DM.Instance.GetJson());
                        return;
                    }

                    if (path == "/*play")
                    {
                        if(DM.Instance.PlayFile(o["f"]))
                        {
                            ReplyJson(response, o, "{success:true}");
                            return;
                        }
                    }

                    if (path == "/*searchImdb")
                    {
                        var q = o["q"];
                        if (q.IsNullOrEmpty())
                        {
                            var g = Scanner.ParseMovieName(o["f"]);
                            q = g.GuessedTitle + " " + g.GuessedYear;
                        }

                        var results = new IMDBClient().Find(q);
                        ReplyJson(response, o, new Serializer(typeof(SearchImdb)).Serialize(new SearchImdb(q, results)));
                        return;
                    }

                    if (path == "/*setMatch")
                    {
                        var id = o["id"];
                        var file = o["f"];
                        if (!id.IsNullOrEmpty() || !file.IsNullOrEmpty())
                        {
                            if (id.IsNullOrEmpty())
                                DM.Instance.AddUnmatched(file);
                            else if (file.IsNullOrEmpty())
                                DM.Instance.UnmatchMovie(id);
                            else
                                DM.Instance.AddMovie(Scanner.FetchMovie(file, id));
                        }

                        ReplyJson(response, o, DM.Instance.GetJson());
                        return;
                    }

                    if (path == "/*setTag")
                    {
                        var id = o["id"];
                        var tag = o["tag"];
                        bool del = o["del"] == "1";

                        DM.Instance.SetTag(tag, id, del);

                        ReplyJson(response, o, DM.Instance.GetJson());
                        return;
                    }

                    //TODO: resize image in a thread
                    if (path.StartsWith("/*cover/"))
                    {
                        var p = path.Split('/', 4);
                        if(p.Length != 4)
                            throw new Exception("invalid scale request");

                        var format = RegScaleFormat.Match(p[2]);
                        if (!format.Success)
                            throw new Exception("Invalid format");

                        String img = Path.Combine(DM.CoverDir, p[3]);
                        if (!File.Exists(img))
                            img = QueryPathToFile("img/nocover.jpg");
                        if(!File.Exists(img))
                            throw new Exception("Image not found " + img);

                        String dir = Path.Combine(ScaledDir, format.Groups[0].Value);
                        Directory.CreateDirectory(dir);
                        String scaledPath = Path.Combine(dir, Path.GetFileName(img));

                        if(!File.Exists(scaledPath))
                        {
                            using(var b = new Bitmap(img))
                            {
                                String w = format.Groups[1].Value;
                                String h = format.Groups[2].Value;
                                String flags = format.Groups[3].Value;
                                using(var  scaled = ImgUtil.Scale(b,
                                    w.IsNullOrEmpty() ? (int?)null : int.Parse(w),
                                    h.IsNullOrEmpty() ? (int?)null : int.Parse(h),
                                    flags.Contains('c'),
                                    flags.Contains('s')))
                                {
                                    ImgUtil.Compress(scaled, scaledPath, ImageFormat.Jpeg, 90);
                                }
                            }
                        }
                        if (ReplyFile(response, scaledPath))
                            return;
                    }

                    ReplyText(response, "The resource you requested ('" + path + "') could not be found.", false, "404 Not Found");
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    ReplyText(response, e.Message + "\n\n" + e.StackTrace, false, "500 Internal Server Error");
                }
            }

            /// <summary>
            /// Try to return a file to client. Return false if file is not found
            /// </summary>
            /// <returns></returns>
            private static bool ReplyFile(IHttpResponseDelegate response, String filePath)
            {
                if (!File.Exists(filePath))
                    return false;

                var ext = (Path.GetExtension(filePath).TrimOrNull() ?? ".").Substring(1).ToLower();
                String contentType = ContentTypes.GetOrDefault(ext, "text/plain");

                Reply(response, contentType, File.ReadAllBytes(filePath), contentType.StartsWith("image/"), null);
                return true;
            }

            private static void ReplyJson(IHttpResponseDelegate response, NameValueCollection o, String json)
            {
                var cb = o["jsonp"];
                var data = cb.IsNullOrEmpty()
                    ? json
                    : String.Format("{0}({1})", cb, json);
                Reply(response, "application/x-javascript; charset=utf-8", Encoding.UTF8.GetBytes(data), false, null);
            }

            private static void Reply(IHttpResponseDelegate response, String contentType, byte[] data, bool allowCache, String status)
            {
                var body = new BufferedBody(data);
                var headers = new Dictionary<string, string>{
                    { "Content-Type", contentType },
                    { "Content-Length", body.Length.ToString() },
                };
                if (allowCache)
                    headers["Cache-Control"] = "max-age=31556926";
                response.OnResponse(new HttpResponseHead
                {
                    Status = status ?? "200 OK",
                    Headers = headers
                }, body);
            }

            private static void ReplyText(IHttpResponseDelegate response, String data, bool allowCache, String status)
            {
                Reply(response, "text/plain; charset=utf-8", Encoding.UTF8.GetBytes(data), allowCache, status);
            }
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

        public class SearchImdb
        {
            public List<Result> Results;
            public String Q;

            public SearchImdb(String q, List<JsonFind.List> results)
            {
                Q = q;
                Results = results.ConvertAll(r => new Result(r)).ToList();
            }
            public class Result
            {
                public String ImdbId;
                public String Title;
                public String Infos;
                public Result(JsonFind.List r)
                {
                    Title = (r.title + " " + r.year).Trim();
                    Infos = r.principals.NoNull().Where(p => p != null).Select(p => p.name).Join(", ");
                    ImdbId = r.tconst;
                }
            }
        }
    }
}
