using Hypermint.Base.Interfaces;

namespace Hs.Hypermint.Services
{
    public class SettingsRepo : ISettingsRepo
    {
        public string HyperSpinPath { get; set; }

        public string RocketLauncherPath { get; set; }

        public string GetHsPathFromSettings()
        {            
            return Properties.Settings.Default.HyperSpinPath;
        }

        public string GetRlPathFromSettings()
        {
            return Properties.Settings.Default.RocketLauncherPath;
        }

        public bool SaveHsPathToSettings()
        {
            if (string.IsNullOrEmpty(HyperSpinPath))
                return false;
            else {

                Properties.Settings.Default.HyperSpinPath = HyperSpinPath;

                Properties.Settings.Default.Save();

                return true;
            }
        }

        public bool SaveRlPathToSettings()
        {
            if (RocketLauncherPath == "")
                return false;

            Properties.Settings.Default.RocketLauncherPath = RocketLauncherPath;

            Properties.Settings.Default.Save();

            return true;

        }
    }
}
