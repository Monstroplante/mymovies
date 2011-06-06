using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Helper;
using Helper.IMDB;
using System.Web.Script.Serialization;
using Monstro.Util;

namespace MyMovies
{
    public partial class Scan : Window
    {
        private int _pos = -1;
        private List<MovieInfos> _movies;
        private BackgroundWorker _imdbWorker;

        private MovieInfos Current
        {
            get
            {
                return _movies.GetOrDefault(_pos);
            }
        }

        public Scan(List<MovieInfos> movies)
        {
            if(!movies.NoNull().Any())
                throw new Exception("movies is empty");
            _movies = movies;
            InitializeComponent();
            Next();
        }

        private void Next()
        {
            _pos++;
            var m = Current;
            if (m == null)
            {
                Close();
                return;
            }
            Title = String.Format("Scan {0}/{1}", _pos + 1, _movies.Count);
            TbFile.Text = m.Path;
            TbQuery.Text = (m.GuessedTitle + " " + m.GuessedYear).Trim();

            Search();
        }

        private void Search()
        {
            LvMovies.ItemsSource = null;
            if (_imdbWorker != null)
                _imdbWorker.CancelAsync();
            _imdbWorker = Reactor.Run(
                TbQuery.Text,
                q => new IMDB().Find(q),
                r => LvMovies.ItemsSource = r,
                e => { if (!(e is CancelledException)) MessageBox.Show("Unable to get movies from IMDB"); },
                (r, e) => RefreshImdbSpinner());
            RefreshImdbSpinner();
        }

        private void RefreshImdbSpinner()
        {
            spinImdb.Visibility = _imdbWorker != null && _imdbWorker.IsBusy
                ? Visibility.Visible : Visibility.Hidden;
        }

        private void BtSkip_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void BtIgnore_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void BtPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Current.Path);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void BtSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }
    }

    public class TestMovieProvider : List<JsonFind.List>
    {
        public TestMovieProvider()
        {
            const String json = "{\"data\":{\"fields\":[\"title\",\"name\"],\"q\":\"la vie est un long fleuve tranquile\",\"results\":[{\"label\":\"Titres (RÃ©sultats Approximatif)\",\"list\":[{\"tconst\":\"tt0096386\",\"title\":\"La vie est un long fleuve tranquille\",\"type\":\"feature\",\"year\":\"1988\",\"image\":{\"width\":77,\"url\":\"http://ia.media-imdb.com/images/M/MV5BMTcxMzA1MjI3NF5BMl5BanBnXkFtZTcwMDU4MjUxMQ@@._V1_.jpg\",\"height\":140},\"principals\":[{\"nconst\":\"nm0536095\",\"name\":\"BenoÃ®t Magimel\"},{\"nconst\":\"nm1663900\",\"name\":\"ValÃ©rie Lalande\"},{\"nconst\":\"nm0739279\",\"name\":\"Tara RÃ¶mer\"},{\"nconst\":\"nm0282467\",\"name\":\"JÃ©rÃ´me Floch\"}]},{\"tconst\":\"tt0095253\",\"title\":\"The Great Outdoors\",\"type\":\"feature\",\"year\":\"1988\",\"image\":{\"width\":335,\"url\":\"http://ia.media-imdb.com/images/M/MV5BMjE0NDQ2NjU0Ml5BMl5BanBnXkFtZTcwNzIyODQyMQ@@._V1_.jpg\",\"height\":475},\"principals\":[{\"nconst\":\"nm0000101\",\"name\":\"Dan Aykroyd\"},{\"nconst\":\"nm0001006\",\"name\":\"John Candy\"},{\"nconst\":\"nm0266995\",\"name\":\"Stephanie Faracy\"},{\"nconst\":\"nm0000906\",\"name\":\"Annette Bening\"}]}]}]},\"@type\":\"mobile.find.results\",\"copyright\":\"For use only by clients authorized in writing by IMDb.  Authors and users of unauthorized clients accept full legal exposure/liability for their actions.\"}";
            AddRange(new JavaScriptSerializer().Deserialize<JsonFind>(json).data.results.NoNull().SelectMany(e => e.list));
        }
    }
}
