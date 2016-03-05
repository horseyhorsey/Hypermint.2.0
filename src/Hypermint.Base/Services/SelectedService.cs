using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hs.HyperSpin.Database;

namespace Hypermint.Base.Services
{
    public class SelectedService : ISelectedService
    {
        public string CurrentSystem { get; set; }

        public ImageSource GameImage { get; set; }

        public BitmapImage SystemImage { get; set; }

        public List<Game> SelectedGames { get; set; }

        public bool IsMainMenu()
        {
            if (CurrentSystem.Contains("Main Menu"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get imagesource from URI file link
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ImageSource SetBitmapFromUri(Uri source)
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
