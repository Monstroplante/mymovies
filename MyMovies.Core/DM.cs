using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Web.Script.Serialization;
using JsonExSerializer;

namespace MyMovies.Core
{
    public class DM
    {
        private static DM instance;
        private readonly DataBase _data;
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
                using (Stream s = File.OpenRead(@"C:\Users\Tom\projects\MyMovies\trunk\MyMovies.UI\bin\Debug\data.json"))
                {
                    _data = (DataBase)new Serializer(typeof(DataBase)).Deserialize(s);
                }
            }
            else
            {
                _data = new DataBase();
            }
        }

        public void Save()
        {
            lock (_data)
            {
                File.WriteAllText(_dataFile, GetJson(), Encoding.UTF8);
            }
        }

        public String GetJson()
        {
            lock (_data)
            {
                return new Serializer(typeof (DataBase)).Serialize(_data);
            }
        }

        public void AddMovie(Movie movie)
        {
            lock (_data)
            {
                foreach (var file in movie.Files)
                {
                    _data.Unmatched.Remove(file);
                    _data.Ignored.Remove(file);
                    _data.Skipped.Remove(file);
                }
                if (_data.Movies.Any(o => o.Files.Intersect(movie.Files).Any()))
                    throw new Exception("This file is already in collection");
                var m = movie.ImdbId.IsNullOrEmpty() ? null : _data.Movies.FirstOrDefault(o => o.ImdbId == movie.ImdbId);
                if(m != null)
                    m.Files = m.Files.Concat(movie.Files).Distinct().OrderBy(s => s).ToList();
                else
                    _data.Movies.Add(movie);
            }
        }

        public int CountMovies()
        {
            lock (_data)
            {
                return _data.Movies.Count;
            }
        }

        public bool PlayFile(string f)
        {
            if (!HasFile(f))
                return false;
            Process.Start(f);
            return true;
        }

        public bool HasFile(string f)
        {
            lock (_data)
            {
                return _data.Movies
                    .SelectMany(m => m.Files)
                    .Concat(_data.Unmatched)
                    .Concat(_data.Ignored)
                    .Concat(_data.Skipped)
                    .Contains(f);
            }
        }

        public void AddUnmatched(string path)
        {
            lock (_data)
            {
                _data.Unmatched.Add(path);
            }
        }

        public void AddSkipped(string path)
        {
            lock (_data)
            {
                _data.Skipped.Add(path);
            }
        }
    }

    public class DataBase
    {
        public List<Movie> Movies = new List<Movie>();
        public HashSet<String> Unmatched = new HashSet<String>();
        public HashSet<String> Ignored = new HashSet<String>();
        public HashSet<String> Skipped = new HashSet<String>();
    }
}
