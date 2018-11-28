using Prism.Mvvm;
using System;
using System.IO;

namespace Hypermint.Base.Model
{
    public class Setting : BindableBase
    {
        public readonly string ExportPath;
        public string PresetPath { get; }
        public string TrashPath { get; }

        /// <summary>
        /// Initializes a new Setting and Export, Preset and Trash populated
        /// </summary>
        public Setting()
        {
            var hmDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Hypermint");
            ExportPath = Path.Combine(hmDataPath, "Exports");
            PresetPath = Path.Combine(hmDataPath, "Preset");
            TrashPath = Path.Combine(hmDataPath, "Trash");
        }

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

        private string ghostscriptPath;
        /// <summary>
        /// Path for Imagemagick
        /// This should be removed and just use the IM libary instead
        /// </summary>
        public string GhostscriptPath
        {
            get { return ghostscriptPath; }            
            set { SetProperty(ref ghostscriptPath, value); }

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

        private string _icons;
        /// <summary>
        /// Gets or sets the icons.
        /// </summary>
        public string Icons
        {
            get { return _icons; }
            set { SetProperty(ref _icons, value); }
        }

        private string _ffmpeg;
        /// <summary>
        /// Gets or sets the Ffmpeg path.
        /// </summary>
        public string Ffmpeg
        {
            get { return _ffmpeg; }
            set { SetProperty(ref _ffmpeg, value); }
        }
    }
}
