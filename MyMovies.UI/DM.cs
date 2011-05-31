using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Web.Script.Serialization;
using MyMovies.Core;

namespace MyMovies.UI
{
    public class DM
    {
        private List<Movie> _movies;
        private readonly String _dataFile = GetLocalFilePath("data.json");

        public static String GetLocalFilePath(String filename)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);
        }

        public DM()
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
                File.WriteAllText(_dataFile, new JavaScriptSerializer().Serialize(_movies), Encoding.UTF8);
            }
        }

        public void AddMovie(Movie movie)
        {
            lock (_movies)
            {
                if (_movies.Any(m => m.Files.Intersect(movie.Files).Any()))
                    throw new Exception("This file is already in collection");
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
