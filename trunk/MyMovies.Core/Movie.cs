using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper;
using Helper.IMDB;
using MyMovies.Core.IMDB;

namespace MyMovies.Core
{
    public class Movie
    {
        public List<String> Files = new List<String>();
        public List<String> Principals = new List<String>();
        public List<String> Directors = new List<String>();
        public List<String> Writers = new List<String>();
        public List<String> Genres = new List<String>();
        public String Title;
        public int? Year;
        public String ImdbId;
        public double? ImdbRating;
        public String Cover;
        public DateTime DateAdded;
        public int? Duration;
        public String Plot;
        public String GuessedTitle;

        public Movie(MovieInfos file, JsonMainDetails.Data infos, String coverFileName)
        {
            DateAdded = DateTime.Now;

            Title = infos.title;
            try
            {
                Year = int.Parse(infos.year);
            }
            catch { }
            if (infos.plot != null)
                Plot = infos.plot.outline;
            ImdbId = infos.tconst;
            Principals = infos.cast_summary.NoNull().ConvertAll(p => p.name.name).ToList();
            Directors = infos.directors_summary.NoNull().ConvertAll(p => p.name.name).ToList();
            Writers = infos.writers_summary.NoNull().ConvertAll(p => p.name.name).ToList();
            Genres = infos.genres.ToList();
            ImdbRating = infos.rating;
            if (infos.runtime != null)
                Duration = infos.runtime.time;

            GuessedTitle = (file.GuessedTitle + " " + file.GuessedYear).Trim();
            Files.Add(file.Path);
            Cover = coverFileName;
        }

        /// <summary>
        /// Required for json deserialization
        /// </summary>
        public Movie(){}
    }
}
