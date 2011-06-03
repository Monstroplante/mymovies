using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper.IMDB;
using MyMovies.Core.IMDB;

namespace MyMovies.Core
{
    public class Movie
    {
        public List<String> Files = new List<String>();
        public List<String> Principals = new List<String>();
        public List<String> Genres = new List<String>();
        public String Title;
        public int? Year;
        public String ImdbId;
        public double? ImdbRating;
        public String Cover;
        public DateTime DateAdded;
        public int? Duration;

        public Movie()
        {
            DateAdded = DateTime.Now;
        }

        public void UpdateInfos(JsonFind.List infos)
        {
            Title = infos.title;
            try
            {
                Year = int.Parse(infos.year);
            }
            catch { }
            ImdbId = infos.tconst;
            Principals = infos.principals.NoNull().ConvertAll(p => p.name).ToList();
        }

        public void UpdateInfos(JsonMainDetails.Data infos)
        {
            Genres = infos.genres.ToList();
            ImdbRating = infos.rating;
            if (infos.runtime != null)
                Duration = infos.runtime.time;
        }
    }
}
