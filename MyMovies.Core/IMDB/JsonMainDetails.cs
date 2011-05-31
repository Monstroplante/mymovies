using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMovies.Core.IMDB
{
    public class JsonMainDetails
    {
        public int exp { get; set; }
        public Data data { get; set; }
        public string copyright { get; set; }

        public class Image
        {
            public int width { get; set; }
            public string url { get; set; }
            public int height { get; set; }
        }
 
        public class Photo
        {
            public Image image { get; set; }
        }
 
        public class Image2
        {
            public int width { get; set; }
            public string url { get; set; }
            public int height { get; set; }
        }
 
        public class Name
        {
            public string nconst { get; set; }
            public string name { get; set; }
            public Image2 image { get; set; }
        }
 
        public class DirectorsSummary
        {
            public Name name { get; set; }
            public string attr { get; set; }
        }
 
        public class UserComment
        {
            public int user_rating { get; set; }
            public string date { get; set; }
            public string status { get; set; }
            public string user_name { get; set; }
            public string summary { get; set; }
            public int user_score { get; set; }
            public string text { get; set; }
            public string user_location { get; set; }
            public int user_score_count { get; set; }
        }
 
        public class Certificate
        {
            public string certificate { get; set; }
        }
 
        public class Image3
        {
            public int width { get; set; }
            public string url { get; set; }
            public int height { get; set; }
        }
 
        public class Name2
        {
            public string nconst { get; set; }
            public string name { get; set; }
            public Image3 image { get; set; }
        }
 
        public class WritersSummary
        {
            public Name2 name { get; set; }
            public string attr { get; set; }
        }
 
        public class H264480x270
        {
            public string format { get; set; }
            public string url { get; set; }
        }
 
        public class H264640x360
        {
            public string format { get; set; }
            public string url { get; set; }
        }
 
        public class Image4
        {
            public int width { get; set; }
            public string url { get; set; }
            public int height { get; set; }
        }
 
        public class RelatedTitle
        {
            public string type { get; set; }
            public string title { get; set; }
            public Image4 image { get; set; }
            public string year { get; set; }
            public string title_id { get; set; }
        }
 
        public class Slate
        {
            public int width { get; set; }
            public string url { get; set; }
            public int height { get; set; }
        }
 
        public class Image5
        {
            public int width { get; set; }
            public string url { get; set; }
            public int height { get; set; }
        }
 
        public class RelatedName
        {
            public string nconst { get; set; }
            public string name { get; set; }
            public Image5 image { get; set; }
        }
 
        public class Trailer
        {
            public RelatedTitle relatedTitle { get; set; }
            public string description { get; set; }
            public int duration_seconds { get; set; }
            public Slate[] slates { get; set; }
            public string content_type { get; set; }
            public RelatedName relatedName { get; set; }
            public string id { get; set; }
            public string title { get; set; }
            public string @type { get; set; }
        }
 
        public class ReleaseDate
        {
            public string normal { get; set; }
        }
 
        public class Image6
        {
            public int width { get; set; }
            public string url { get; set; }
            public int height { get; set; }
        }
 
        public class Runtime
        {
            public int time { get; set; }
        }
 
        public class Image7
        {
            public int width { get; set; }
            public string url { get; set; }
            public int height { get; set; }
        }
 
        public class Name3
        {
            public string nconst { get; set; }
            public string name { get; set; }
            public Image7 image { get; set; }
        }
 
        public class CastSummary
        {
            public Name3 name { get; set; }
            public string attr { get; set; }
        }
 
        public class Plot
        {
            public int more { get; set; }
            public string outline { get; set; }
        }
 
        public class Ns0000063
        {
            public string logo { get; set; }
            public string url { get; set; }
            public string label { get; set; }
        }
 
        public class Ns0000077
        {
            public string logo { get; set; }
            public string url { get; set; }
            public string label { get; set; }
        }
 
        public class Ns0011867
        {
            public string logo { get; set; }
            public string url { get; set; }
            public string label { get; set; }
        }
 
        public class Sources
        {
            public Ns0000063 ns0000063 { get; set; }
            public Ns0000077 ns0000077 { get; set; }
            public Ns0011867 ns0011867 { get; set; }
        }
 
        public class Item
        {
            public string source { get; set; }
            public string head { get; set; }
            public string id { get; set; }
            public string datetime { get; set; }
        }
 
        public class News
        {
            public string channel { get; set; }
            public int total { get; set; }
            public Sources sources { get; set; }
            public string markup { get; set; }
            public string label { get; set; }
            public int limit { get; set; }
            public Item[] items { get; set; }
            public string @type { get; set; }
            public int start { get; set; }
        }
 
        public class Data
        {
            public Photo[] photos { get; set; }
            public DirectorsSummary[] directors_summary { get; set; }
            public UserComment user_comment { get; set; }
            public Certificate certificate { get; set; }
            public string[] has { get; set; }
            public WritersSummary[] writers_summary { get; set; }
            public double? rating { get; set; }
            public int num_votes { get; set; }
            public string tconst { get; set; }
            public string[] genres { get; set; }
            public bool can_rate { get; set; }
            public Trailer trailer { get; set; }
            public ReleaseDate release_date { get; set; }
            public string trivium { get; set; }
            public string goof { get; set; }
            public Image6 image { get; set; }
            public string tagline { get; set; }
            public Runtime runtime { get; set; }
            public CastSummary[] cast_summary { get; set; }
            public Plot plot { get; set; }
            public News news { get; set; }
            public string type { get; set; }
            public string title { get; set; }
            public string year { get; set; }
        }
    }
}
