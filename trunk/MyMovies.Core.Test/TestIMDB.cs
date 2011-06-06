using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper.IMDB;
using Monstro.Util;
using NUnit.Framework;
using System.Threading;

namespace Test
{
    [TestFixture]
    public class TestIMDB
    {
        [Test]
        public void TestFind()
        {
            var api = new IMDBClient("fr_FR");
            var results = api.Find("matrix 2003 ");
            Assert.Greater(results.Count, 0);
            var first = results.First();
            Assert.That(first.title.ToLower().Contains("matrix"));
            Assert.AreEqual(2003, first.GetYear());

            var detail = api.GetDetails(first.tconst);
            Assert.AreEqual(first.year, detail.year);
            Assert.AreEqual(first.title, detail.title);
            Assert.AreEqual(first.tconst, detail.tconst);
        }

        [Test]
        public void TestSignUrl()
        {
            var sample = new[]{
                new{
                    usigned = "http://app.imdb.com/find?q=matrix&appid=android2&device=406642b5-8ee2-4a3b-91ac-5229dc474faf&locale=fr_FR",
                    timestamp = 1307058440,
                    signed = "http://app.imdb.com/find?q=matrix&appid=android2&device=406642b5-8ee2-4a3b-91ac-5229dc474faf&locale=fr_FR&timestamp=1307058440&sig=and2-11fd0275e635e91b9a874c61ec7b6a95c455676b"},
                new{
                    usigned = "http://app.imdb.com/title/maindetails?videoformats=H.264%2CCBP&tconst=tt0133093&appid=android2&device=406642b5-8ee2-4a3b-91ac-5229dc474faf&locale=fr_FR",
                    timestamp = 1307058447,
                    signed = "http://app.imdb.com/title/maindetails?videoformats=H.264%2CCBP&tconst=tt0133093&appid=android2&device=406642b5-8ee2-4a3b-91ac-5229dc474faf&locale=fr_FR&timestamp=1307058447&sig=and2-d86c575a2cd3fd0cef28697a4818f2deb23029db"},
                new{
                    usigned = "http://app.imdb.com/title/plot?tconst=tt0133093&appid=android2&device=406642b5-8ee2-4a3b-91ac-5229dc474faf&locale=fr_FR",
                    timestamp = 1307058465,
                    signed = "http://app.imdb.com/title/plot?tconst=tt0133093&appid=android2&device=406642b5-8ee2-4a3b-91ac-5229dc474faf&locale=fr_FR&timestamp=1307058465&sig=and2-701c6103545a2ba91fa6733fc910c1772ef25ffc"},
            };
            foreach (var o in sample)
            {
                Assert.AreEqual(o.signed, IMDBClient.SignUrl(new UrlBuilder(o.usigned), o.timestamp));
            }
        }
    }
}
