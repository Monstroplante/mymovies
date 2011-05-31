using System;
using System.Collections.Generic;
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
        DM DM = new DM();

        public Window1()
        {
            InitializeComponent();
            WebServer.Start(8080);
            UpdateTitle();
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
            var directories = tbDirectories.Text.Split('\n')
                .ConvertAll(s => s.TrimOrNull())
                .Where(s => s != null)
                .ToArray();
            if(directories.Length < 1)
                return;
            var w = Reactor.Run(
                directories,
                dirs =>
                {
                    var imdb = new IMDB();
                    foreach (String dir in dirs)
                    {
                        var files = Scanner.ScanDir(dir);
                        foreach (var f in files)
                        {
                            if(DM.GetMovieByFile(f.Path) != null)
                                continue;
                            var m = imdb.Find(f.Title + " " + f.Year).FirstOrDefault();
                            if (m == null)
                                continue;
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
                null, null, null);
        }

        void UpdateTitle()
        {
            Title = "My movies - " + DM.CountMovies();
        }
    }
}
