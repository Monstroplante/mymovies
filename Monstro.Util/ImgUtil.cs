using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Drawing;

namespace Monstro.Util
{
    public static class ImgUtil
    {

        public static void Compress(Bitmap b, Stream output, ImageFormat fmt, int? jpegQ)
        {
            if (fmt == ImageFormat.Jpeg)
            {
                var codec = ImageCodecInfo.GetImageEncoders()
                    .FirstOrDefault(e => e.MimeType == "image/jpeg");
                var ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(Encoder.Quality, jpegQ ?? 85);
                b.Save(output, codec, ep);
            }
            else
            {
                b.Save(output, fmt);
            }
        }

        public static void Compress(Bitmap b, String outputFile, ImageFormat fmt, int? jpegQ)
        {
            if (fmt == ImageFormat.Jpeg)
            {
                var codec = ImageCodecInfo.GetImageEncoders()
                    .FirstOrDefault(e => e.MimeType == "image/jpeg");
                var ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(Encoder.Quality, jpegQ ?? 85);
                b.Save(outputFile, codec, ep);
            }
            else
            {
                b.Save(outputFile, fmt);
            }
        }

        /// <summary>
        /// Resize a bitmap. Aspec ratio is always preserved
        /// </summary>
        /// <param name="src">input bitmap</param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="crop">If true and maxWidth and maxHeight are not null, crop image to have output image fit both max dimentions</param>
        /// <param name="stretch">If true allow src image to grow to fit max dimentions</param>
        /// <returns></returns>
        public static Bitmap Scale(Bitmap src, int? maxWidth, int? maxHeight, bool crop, bool stretch)
        {
            if (maxWidth == null || maxHeight == null)
                crop = false;
            int maxW = maxWidth ?? int.MaxValue;
            int maxH = maxHeight ?? int.MaxValue;

            if (maxW < 1 || maxH < 1)
                throw new ArgumentException("maxW < 1 || maxH < 1");

            double wratio = maxW / (double)src.Width;
            double hratio = maxH / (double)src.Height;

            double ratio = crop
                ? Math.Max(wratio, hratio)
                : Math.Min(wratio, hratio);

            if (!stretch)
                ratio = Math.Min(1, ratio);

            int destW = Math.Min((int)Math.Ceiling(src.Width * ratio), maxW);
            int destH = Math.Min((int)Math.Ceiling(src.Height * ratio), maxH);

            if (destW == src.Width && destH == src.Height) // No need to resize
                return (Bitmap)src.Clone();

            int viewportW = Math.Min(src.Width, (int)Math.Round(destW / ratio));
            int viewportH = Math.Min(src.Height, (int)Math.Round(destH / ratio));

            var result = new Bitmap(destW, destH, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(result))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Avoid artefacts at image edges
                // (see: http://www.codeproject.com/KB/GDI-plus/imgresizoutperfgdiplus.aspx)
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(src, new Rectangle(0, 0, result.Width, result.Height),
                        (src.Width - viewportW) / 2,
                        (src.Height - viewportH) / 2,
                        viewportW, viewportH, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return result;
        }

    }
}
