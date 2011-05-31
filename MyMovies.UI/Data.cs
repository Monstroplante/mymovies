using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Helper;
using Helper.IMDB;
using MyMovies.Core;

namespace MyMovies
{
    public static class Data
    {
        public static ObservableCollection<Movie> Movies = new ObservableCollection<Movie>();
    }
}
