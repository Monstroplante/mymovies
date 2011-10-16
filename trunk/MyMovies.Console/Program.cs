using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Helper;
using Helper.IMDB;
using Monstro.Util;
using Test;

namespace FileScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            var curdir = Directory.GetCurrentDirectory();
            var movies = Scanner.GetFiles(@"C:\torrent\films").Select(Scanner.ParseMovieName);

            movies = movies
                //.Where(m => !TestScanner.Good.Any(g => g.File == m.Path))
                .ToList();

            foreach (var m in movies)
            {
                Console.WriteLine(m.GuessedTitle + " " + m.GuessedYear);
                var imdb = new IMDBClient("fr_FR").Find(m.GuessedTitle + " " + m.GuessedYear);
                foreach(var r in imdb.Take(3))
                    Console.WriteLine(new[]{"imdb:", r.title, r.year, r.image == null ? null : r.image.url}.Where(s =>!s.IsNullOrEmpty()).Join(" "));
                Console.ReadLine();
            }

            Console.Read();
        }

        static void WriteResult(MovieInfos m, bool good)
        {
            using (var w = new StreamWriter("result.txt", true))
            {
                //MovieInfos(String title, int? year, String path, bool seamsDuplicated, bool ignore)
                w.WriteLine("new TestResult({5}, new MovieInfos(\"{0}\", {1}, @\"{2}\", {3}, {4})),",
                    m.GuessedTitle,
                    m.GuessedYear == null ? "null" : m.GuessedYear.ToString(),
                    m.Path,
                    m.SeamsDuplicated ? "true" : "false",
                    m.ShouldBeIgnored ? "true" : "false",
                    good ? "true" : "false");
            }
        }

        static bool ReadBool(String question)
        {
            while (true)
            {
                Console.Write(question + " - y/n: ");
                var r = (Console.ReadLine() ?? "").ToLower();
                if (r == "y")
                    return true;
                if (r == "n")
                    return false;
            }
        }
    }
}
