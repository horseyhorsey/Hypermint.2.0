using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Events;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using Hypermint.Base.Services;

namespace Hs.Hypermint.MediaPane.ViewModels
{
    public class MediaPaneViewModel : ViewModelBase
    {
        private ImageSource wheelSource;
        private IEventAggregator _eventAggregator;
        private ISelectedService _selectedService;

        public ImageSource WheelSource
        {
            get { return wheelSource; }
            set { SetProperty(ref wheelSource, value); }
        }

        public MediaPaneViewModel(IEventAggregator eventAggregator, ISelectedService selectedService)
        {
            _eventAggregator = eventAggregator;
            _selectedService = selectedService;

            var tempWheel = @"I:\HyperSpin\Media\Amstrad CPC\Images\Wheel\1st Division Manager (Europe).png";

            if (Directory.Exists(tempWheel))
                WheelSource = BitmapFromUri(new System.Uri(tempWheel));

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(SetImage);
            //
        }

        private void SetImage(string obj)
        {
            WheelSource = _selectedService.SystemImage;
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
