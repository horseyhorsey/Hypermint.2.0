using System;
using Hs.Hypermint.Settings;
using Hypermint.Base.Interfaces;

namespace Hs.Hypermint.Services
{
    public class SettingsRepo : ISettingsRepo
    {
        private Setting hypermintSettings = new Setting();
        public Setting HypermintSettings
        {
            get { return hypermintSettings; }
            set { hypermintSettings = value; }
        }

        public void LoadHypermintSettings()
        {            
            HypermintSettings.HsPath = Properties.Settings.Default.HsPath;
            HypermintSettings.RlPath = Properties.Settings.Default.RlPath;
            HypermintSettings.RlMediaPath = Properties.Settings.Default.RlMediaPath;
            HypermintSettings.LaunchParams = Properties.Settings.Default.RlParams;
            HypermintSettings.ImageMagickPath = Properties.Settings.Default.ImPath;
            HypermintSettings.Author = Properties.Settings.Default.RlAuthor;
        }

        public void SaveHypermintSettings()
        {
            
            Properties.Settings.Default.HsPath = HypermintSettings.HsPath;
            Properties.Settings.Default.RlPath = HypermintSettings.RlPath;
            Properties.Settings.Default.RlMediaPath = HypermintSettings.RlMediaPath;
            Properties.Settings.Default.RlParams = HypermintSettings.LaunchParams;
            Properties.Settings.Default.ImPath = HypermintSettings.ImageMagickPath;
            Properties.Settings.Default.RlAuthor = HypermintSettings.Author;

            Properties.Settings.Default.Save();
        }

    }

}
