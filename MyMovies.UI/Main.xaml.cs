using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Monstro.Util;
using MyMovies.Core;
using Helper;
using Helper.IMDB;
using System.Windows.Threading;
using MyMovies.UI;

namespace MyMovies
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        DM DM = DM.Instance;

        public Window1()
        {
            InitializeComponent();
            WebServer.Start(8080);
            UpdateTitle();
            Log.Listner = (cat, message, level) => {
                if(level < Log.Level.Info)
                    return;
                tbLog.Text += cat + ": " + message + "\n";
                tbLog.ScrollToEnd();                 
            };
            btWeb.Content = WebServer.GetHomeUrl();
        }

        private void BtScan_Click(object sender, RoutedEventArgs e)
        {
            new StartScan().Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            WebServer.Stop();
            DM.Save();
            base.OnClosed(e);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var scanLog = new Log("scan");
            var directories = tbDirectories.Text.Split('\n')
                .ConvertAll(s => s.TrimOrNull())
                .Where(s => s != null)
                .ToArray();
            if(directories.Length < 1)
            {
                scanLog.Warn("No directory to scan.");
                return;
            }
                
            var w = Reactor.Run(
                directories,
                dirs =>
                    {
                    var wc = new WebClient();
                    var imdb = new IMDB();
                    foreach (String dir in dirs)
                    {
                        scanLog.Info("Scanning " + dir);
                        var files = Scanner.ScanDir(dir);
                        scanLog.Info(files.Count + " video files found");
                        foreach (var f in files)
                        {
                            if(DM.GetMovieByFile(f.Path) != null)
                            {
                                scanLog.Debug("File already present in collection");
                                continue;
                            }

                            try
                            {
                                scanLog.Info("Processing " + f.Path);

                                String q = f.Title + " " + f.Year;
                                scanLog.Info("IMDB: searching for '{0}'...", q);
                                var m = imdb.Find(q).FirstOrDefault();
                                if (m == null)
                                {
                                    scanLog.Info("IMDB: no result");
                                    continue;
                                }

                                String cover = null;
                                if(m.image != null && !m.image.url.IsNullOrEmpty())
                                {
                                    scanLog.Info("IMDB: downloading cover");
                                    cover = Util.CleanFileName(m.image.url);
                                    var coverDir = System.IO.Path.Combine(WebServer.RootDir, "covers");
                                    if(!Directory.Exists(coverDir))
                                        Directory.CreateDirectory(coverDir);
                                    wc.DownloadFile(m.image.url, System.IO.Path.Combine(coverDir, cover));
                                }

                                scanLog.Info("IMDB: found {0} - {1}", m.title, m.year);
                                scanLog.Info("IMDB: getting movie details...");
                                var detail = imdb.GetDetails(m.tconst);

                                var movie = new Movie();
                                movie.Files.AddRange(f.Duplicated.ConvertAll(d => d.Path).Prepend(f.Path));
                                movie.UpdateInfos(m);
                                movie.UpdateInfos(detail);
                                movie.Cover = cover;
                                DM.AddMovie(movie);

                                Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(delegate
                                {
                                    UpdateTitle();
                                    return null;
                                }), null);
                            }
                            catch(Exception ex)
                            {
                                scanLog.Error(ex.Message);
                            }
                        }
                    }
                    return "";
                },
                null, null, (s, ex) => scanLog.Info("Scan finished"));
        }

        void UpdateTitle()
        {
            Title = "My movies - " + DM.CountMovies();
        }

        private void btWeb_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(WebServer.GetHomeUrl());
        }
    }
}
