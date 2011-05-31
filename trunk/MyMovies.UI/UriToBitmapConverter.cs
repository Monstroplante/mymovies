using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MyMovies
{
    public class UriToBitmapConverter : IValueConverter
    {
        public int? DecodePixelWidth { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Todo : gérer les exceptions.
            try
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                if (DecodePixelWidth != null)
                    bi.DecodePixelWidth = DecodePixelWidth.Value;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = new Uri(value.ToString());
                bi.EndInit();
                return bi;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "error");
                //Todo : retourner une image pour l'erreur ?
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
