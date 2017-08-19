using Hypermint.Base.Interfaces;
using Hypermint.Base.Model;
using System.IO;

namespace Hypermint.Base
{
    public class SettingsRepo : ISettingsHypermint
    {
        private Setting hypermintSettings = new Setting();
        public Setting HypermintSettings
        {
            get { return hypermintSettings; }
            set { hypermintSettings = value; }
        }

        public void LoadHypermintSettings()
        {
            var settingsPath = "settings.bin";

            if (!File.Exists(settingsPath))
            {
                CreateDefaultSettings();
            }

            var binReader = new BinaryReader(File.OpenRead(settingsPath));

            HypermintSettings.HsPath = binReader.ReadString();
            HypermintSettings.RlPath = binReader.ReadString();
            HypermintSettings.RlMediaPath = binReader.ReadString();
            HypermintSettings.LaunchParams = binReader.ReadString();
            HypermintSettings.Author = binReader.ReadString();            

            try
            {
                HypermintSettings.GhostscriptPath = binReader.ReadString();
                HypermintSettings.Icons = binReader.ReadString();
            }
            catch (EndOfStreamException) { }

            binReader.Close();

        }

        /// <summary>
        /// Creates the default settings for Hypermint
        /// </summary>
        public void CreateDefaultSettings()
        {
            var binWriter = new BinaryWriter(File.Create("settings.bin"));

            binWriter.Write(@"C:\Hyperspin");
            binWriter.Write(@"C:\RocketLauncher");
            binWriter.Write(@"C:\RocketLauncher\Media");
            binWriter.Write(@"");
            binWriter.Write(@"Hypermint");
            binWriter.Write(@"C:\Program Files\gs\gs9.14\bin");
            binWriter.Write(@"C:\RocketLauncher\RocketLauncherUI\Media\Icons");

            binWriter.Close();

        }

        public void SaveHypermintSettings()
        {
            var binWriter = new BinaryWriter(File.Create("settings.bin"));

            binWriter.Write(HypermintSettings.HsPath);
            binWriter.Write(HypermintSettings.RlPath);
            binWriter.Write(HypermintSettings.RlMediaPath);
            binWriter.Write(HypermintSettings.LaunchParams);
            binWriter.Write(HypermintSettings.Author);
            binWriter.Write(HypermintSettings.GhostscriptPath);
            binWriter.Write(HypermintSettings.Icons);

            binWriter.Close();
        }
    }

}
