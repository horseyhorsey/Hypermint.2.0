using Hs.Hypermint.Services.Helpers;
using System.IO;

namespace Hs.Hypermint.Services
{
    public class RocketSettingsRepo
    {
        public string GetDefaultEmulator(string rlPath, string systemName)
        {
            string section = "ROMS";
            string key = "Default_Emulator";

            var iniFile = BuildEmuIniPath(rlPath, systemName);

            var defaultEmu = GetIniValue(iniFile, section, key);

            return defaultEmu ?? "";
        }

        public string[] GetRomExtensions(string rlPath, string emuName)
        {
            string section = emuName;

            string key = "Rom_Extension";

            var iniFile = BuildGlobalEmuIniPath(rlPath);

            var extensions = GetIniValue(iniFile, section, key).Split('|');

            return extensions;
        }

        public string[] GetRomPaths(string rlPath, string systemName)
        {
            string section = "ROMS";
            string key = "Rom_Path";

            var iniFile = BuildEmuIniPath(rlPath, systemName);

            var rlDriveLetter = Directory.GetDirectoryRoot(rlPath);

            var paths = GetIniValue(iniFile, section, key).Split('|');
            string[] pathsNormalized = new string[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].Contains(@"..\"))
                    pathsNormalized[i] =
                        paths[i].Replace(@"..\", rlDriveLetter);
            }

            return pathsNormalized;
        }

        private string GetIniValue(string iniFile, string section, string key)
        {
            if (!File.Exists(iniFile)) return null;

            IniFile ini = new IniFile();

            ini.Load(iniFile);

            return ini.GetKeyValue(section, key); ;
        }

        private string BuildEmuIniPath(string rlPath, string systemName) =>
            rlPath + "\\Settings\\" + systemName + "\\Emulators.ini";

        private string BuildGlobalEmuIniPath(string rlPath) =>
            rlPath + "\\Settings\\Global Emulators.ini";
    }
}
