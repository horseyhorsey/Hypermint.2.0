﻿using Microsoft.Practices.Prism.Mvvm;

namespace Hs.Hypermint.Settings
{
    public class Setting : BindableBase
    {
        private string hsPath;
        /// <summary>
        /// HyperSpin Path
        /// </summary>
        public string HsPath
        {
            get { return hsPath; }
            set { SetProperty(ref hsPath, value); }
        }

        private string rlPath;
        /// <summary>
        /// Launcher Path
        /// </summary>
        public string RlPath
        {
            get { return rlPath; }
            set { SetProperty(ref rlPath, value); }
        }

        private string rlMediaPath;            
        /// <summary>
        /// RocketLauncher Media Path
        /// </summary>
        public string RlMediaPath
        {
            get { return rlMediaPath; }            
            set { SetProperty(ref rlMediaPath, value); }
        }

        private string launchParams;
        /// <summary>
        /// Launcher params for RocketLaunch
        /// </summary>
        public string LaunchParams
        {
            get { return launchParams; }
            set { SetProperty(ref launchParams, value); }            
        }

        private string imageMagickPath;
        /// <summary>
        /// Path for Imagemagick
        /// This should be removed and just use the IM libary instead
        /// </summary>
        public string ImageMagickPath
        {
            get { return imageMagickPath; }            
            set { SetProperty(ref imageMagickPath, value); }

        }

        private string author;
        /// <summary>
        /// Author for when creating media files
        /// </summary>
        public string Author
        {
            get { return author; }            
            set { SetProperty(ref author, value); }
        }



    }
}
