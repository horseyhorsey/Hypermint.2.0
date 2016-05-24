using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hypermint.Base.Converters
{
    public class ConvertToImageSource
    {
        /// <summary>
        /// Get imagesource from URI file link
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ImageSource ImageSourceFromUri(Uri source)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = source;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();

                return bitmap;

            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
