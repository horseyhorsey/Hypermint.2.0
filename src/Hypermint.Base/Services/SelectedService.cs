﻿using Hypermint.Base.Model;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hypermint.Base.Services
{
    public class SelectedService : ISelectedService
    {
        public string CurrentSystem { get; set; }

        public ImageSource GameImage { get; set; }

        public BitmapImage SystemImage { get; set; }

        public List<GameItemViewModel> SelectedGames { get; set; }

        public string CurrentMainMenu { get; set; }

        public string CurrentRomname { get; set; }

        public string CurrentDescription { get; set; }

        public bool IsMultiSystem { get; set; }

        public bool IsMainMenu()
        {
            if (CurrentSystem.ToLower().Contains("main menu"))
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
