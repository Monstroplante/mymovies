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
        public List<String> Files { get; set; }
        public List<String> Principals { get; set; }
        public List<String> Directors { get; set; }
        public List<String> Writers { get; set; }
        public List<String> Genres { get; set; }
        public String Title { get; set; }
        public int? Year { get; set; }
        public String ImdbId { get; set; }
        public double? ImdbRating { get; set; }
        public String Cover { get; set; }
        public DateTime DateAdded { get; set; }
        public int? Duration { get; set; }
        public String Plot { get; set; }
        public String GuessedTitle { get; set; }
        public List<String> Tags { get; set; }

        public int Hash
        {
            get { return GetHashCode(); }

            //JsonExSerializer ignore properties with no setter
            set { }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Title != null ? Title.GetHashCode() : 0;
                result = (result * 397) ^ (Year.HasValue ? Year.Value : 0);
                result = (result * 397) ^ (ImdbId != null ? ImdbId.GetHashCode() : 0);
                result = (result * 397) ^ (ImdbRating.HasValue ? ImdbRating.Value.GetHashCode() : 0);
                result = (result * 397) ^ (Cover != null ? Cover.GetHashCode() : 0);
                result = (result * 397) ^ DateAdded.GetHashCode();
                result = (result * 397) ^ (Duration.HasValue ? Duration.Value : 0);
                result = (result * 397) ^ (Plot != null ? Plot.GetHashCode() : 0);
                result = (result * 397) ^ (GuessedTitle != null ? GuessedTitle.GetHashCode() : 0);

                foreach (var s in Files.Concat(Principals).Concat(Directors).Concat(Genres).Concat(Writers).Concat(Tags))
                    result = (result * 397) ^ (s != null ? s.GetHashCode() : 0);

                return result;
            }
        }

        public Movie(String file, JsonMainDetails.Data infos, String coverFileName) : this()
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
            Principals.AddRange(infos.cast_summary.NoNull().Select(p => p.name.name));
            Directors.AddRange(infos.directors_summary.NoNull().Select(p => p.name.name));
            Writers.AddRange(infos.writers_summary.NoNull().Select(p => p.name.name));
            Genres.AddRange(infos.genres.NoNull());
            ImdbRating = infos.rating;
            if (infos.runtime != null)
                Duration = infos.runtime.time;

            Files.Add(file);
            Cover = coverFileName;
        }

        /// <summary>
        /// Required for json deserialization
        /// </summary>
        public Movie()
        {
            Files = new List<String>();
            Principals = new List<String>();
            Directors = new List<String>();
            Writers = new List<String>();
            Genres = new List<String>();
            Tags = new List<String>();
        }
    }
}
