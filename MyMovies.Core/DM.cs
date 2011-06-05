using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Web.Script.Serialization;

namespace MyMovies.Core
{
    public class DM
    {
        private static DM instance;
        private List<Movie> _movies;
        private readonly String _dataFile = GetLocalFilePath("data.json");

        public static String GetLocalFilePath(params String[] path)
        {
            return Path.Combine(path.Prepend(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).ToArray());
        }

        public static DM Instance
        {
            get { return instance ?? (instance = new DM()); }
        }

        private DM()
        {
            if (File.Exists(_dataFile))
            {
                _movies = new JavaScriptSerializer().Deserialize<List<Movie>>(
                    File.ReadAllText(_dataFile, Encoding.UTF8));
                
            }
            else
            {
                _movies = new List<Movie>();
            }
        }

        public void Save()
        {
            lock (_movies)
            {
                File.WriteAllText(_dataFile, GetJson(), Encoding.UTF8);
            }
        }

        public String GetJson()
        {
            lock (_movies)
            {
                return new JavaScriptSerializer().Serialize(_movies);
            }
        }

        public void AddMovie(Movie movie)
        {
            lock (_movies)
            {
                if (_movies.Any(o => o.Files.Intersect(movie.Files).Any()))
                    throw new Exception("This file is already in collection");
                var m = movie.ImdbId.IsNullOrEmpty() ? null : _movies.FirstOrDefault(o => o.ImdbId == movie.ImdbId);
                if(m != null)
                    m.Files.AddRange(movie.Files);
                else
                    _movies.Add(movie);
            }
        }

        public Movie GetMovieByFile(String file)
        {
            lock (_movies)
            {
                return _movies.FirstOrDefault(m => m.Files.Contains(file));   
            }
        }

        public int CountMovies()
        {
            lock (_movies)
            {
                return _movies.Count;
            }
        }

        public IEnumerable<Movie> GetMovies()
        {
            return new List<Movie>(_movies);
        }
    }
}
