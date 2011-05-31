using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper.IMDB;
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
            var api = new IMDB("fr_FR");
            var results = api.Find("matrix 2003 ");
            Assert.Greater(results.Count, 0);
            var first = results.First();
            Assert.That(first.title.ToLower().Contains("matrix"));
            Assert.AreEqual(2003, first.GetYear());
        }
    }
}
