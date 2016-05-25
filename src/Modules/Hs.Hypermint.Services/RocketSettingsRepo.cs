using Hs.Hypermint.Services.Helpers;
using System.IO;

namespace Hs.Hypermint.Services
{
    public class RocketSettingsRepo
    {
        public static string GetDefaultEmulator(string rlPath, string systemName)
        {
            string section = "ROMS";
            string key = "Default_Emulator";

            var iniFile = BuildEmuIniPath(rlPath, systemName);

            var defaultEmu = GetIniValue(iniFile, section, key);

            return defaultEmu ?? "";
        }

        public static string[] GetRomExtensions(string rlPath, string emuName)
        {
            string section = emuName;

            string key = "Rom_Extension";

            var iniFile = BuildGlobalEmuIniPath(rlPath);

            var extensions = GetIniValue(iniFile, section, key).Split('|');

            return extensions;
        }

        public static string[] GetRomPaths(string rlPath, string systemName)
        {
            string section = "ROMS";
            string key = "Rom_Path";

            var iniFile = BuildEmuIniPath(rlPath, systemName);

            var rlDriveLetter = Directory.GetDirectoryRoot(rlPath);

            var paths = GetIniValue(iniFile, section, key).Split('|');
                        
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].Contains(@"..\"))
                {
                    do
                    {
                        paths[i] = paths[i].Replace(@"..\", "");

                    } while (paths[i].Contains(@"..\"));

                    paths[i] = rlDriveLetter + paths[i];                    
                }
            }

            return paths;
        }

        private static string GetIniValue(string iniFile, string section, string key)
        {
            if (!File.Exists(iniFile)) return null;

            IniFile ini = new IniFile();

            ini.Load(iniFile);

            return ini.GetKeyValue(section, key);
        }

        private static string BuildEmuIniPath(string rlPath, string systemName) =>
            rlPath + "\\Settings\\" + systemName + "\\Emulators.ini";

        private static string BuildGlobalEmuIniPath(string rlPath) =>
            rlPath + "\\Settings\\Global Emulators.ini";
    }
}
