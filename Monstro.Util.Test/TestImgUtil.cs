using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Monstro.Util.Test
{
    [TestFixture]
    class TestImgUtil
    {
        public const String ImagesDir = @"..\..\images\";
        public const String ResultDir = ImagesDir + @"result\";

        [SetUp]
        public void Setup()
        {
            if(Directory.Exists(ResultDir))
                Directory.Delete(ResultDir, true);
            Directory.CreateDirectory(ResultDir);
        }

        [Test]
        public void TestScale()
        {
            var b = new Bitmap(Path.Combine(ImagesDir, "test.jpg"));
            Assert.AreEqual(500, b.Width);
            Assert.AreEqual(740, b.Height);

            var expected = new[]{
                new{maxW = (int?)500, maxH = (int?)740, crop = false, strech = false, w = 500, h = 740},
                new{maxW = (int?)600, maxH = (int?)null, crop = false, strech = false, w = 500, h = 740},
                new{maxW = (int?)600, maxH = (int?)null, crop = false, strech = true, w = 600, h = 888},
                new{maxW = (int?)100, maxH = (int?)100, crop = true, strech = false, w = 100, h = 100},
                new{maxW = (int?)100, maxH = (int?)100, crop = false, strech = false, w = 68, h = 100},
                new{maxW = (int?)1000, maxH = (int?)500, crop = false, strech = true, w = 338, h = 500},
                new{maxW = (int?)500, maxH = (int?)1000, crop = true, strech = true, w = 500, h = 1000},
            };

            foreach(var o in expected)
            {
                String format = new object[] { o.maxW, 'x', o.maxH, o.crop ? "crop" : null, o.strech ? "stretch" : null }
                    .ConvertAll(p => p == null ? "" : p.ToString())
                    .Join("");

                var result = ImgUtil.Scale(b, o.maxW, o.maxH, o.crop, o.strech);
                Assert.AreEqual(o.w, result.Width, format);
                Assert.AreEqual(o.h, result.Height, format);

                //Save result for a visual check
                ImgUtil.Compress(result, Path.Combine(ResultDir, format + ".png"), ImageFormat.Png, 100);
            }
        }
    }
}
