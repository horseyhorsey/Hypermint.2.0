using Hypermint.Base.Base;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hs.Hypermint.MediaPane.ViewModels
{
    public class MediaPaneViewModel : ViewModelBase
    {
        private ImageSource wheelSource;
        public ImageSource WheelSource
        {
            get { return wheelSource; }
            set { SetProperty(ref wheelSource, value); }
        }

        public MediaPaneViewModel()
        {
            var tempWheel = @"I:\HyperSpin\Media\Amstrad CPC\Images\Wheel\1st Division Manager (Europe).png";

            if (Directory.Exists(tempWheel))
                WheelSource = BitmapFromUri(new System.Uri(tempWheel));
            //
        }

        /// <summary>
        /// Get imagesource from URI file link
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ImageSource BitmapFromUri(System.Uri source)
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
            catch (System.Exception)
            {
                return null;
            }

        }
    }
}
