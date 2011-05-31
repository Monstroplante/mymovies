using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                tbLog.Text += cat + ": " + message + "\n";
                tbLog.ScrollToEnd();                 
            };
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
                    var imdb = new IMDB();
                    foreach (String dir in dirs)
                    {
                        scanLog.Info("Scanning " + dir);
                        var files = Scanner.ScanDir(dir);
                        scanLog.Info(files.Count + " video files found");
                        foreach (var f in files)
                        {
                            scanLog.Info("Processing " + f.Path);
                            if(DM.GetMovieByFile(f.Path) != null)
                            {
                                scanLog.Info("File already present in collection");
                                continue;
                            }

                            String q = f.Title + " " + f.Year;
                            scanLog.Info("IMDB: searching for '{0}'...", q);
                            var m = imdb.Find(q).FirstOrDefault();
                            if (m == null)
                            {
                                scanLog.Info("IMDB: no result");
                                continue;
                            }
                            scanLog.Info("IMDB: found {0} - {1}", m.title, m.year);
                            scanLog.Info("IMDB: getting movie details...");
                            var detail = imdb.GetDetails(m.tconst);

                            var movie = new Movie();
                            movie.Files.Add(f.Path);
                            movie.Files.AddRange(f.Duplicated.ConvertAll(d => d.Path));
                            movie.UpdateInfos(m);
                            movie.UpdateInfos(detail);
                            DM.AddMovie(movie);

                            Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(delegate{
                                UpdateTitle();
                                return null;
                            }), null);
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
    }
}
