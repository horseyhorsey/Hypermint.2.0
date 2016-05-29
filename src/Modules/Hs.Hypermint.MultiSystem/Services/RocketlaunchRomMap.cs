using Hs.Hypermint.Services.Helpers;
using Hs.HyperSpin.Database;
using System.Collections.Generic;
using System.IO;

namespace Hs.Hypermint.MultiSystem.Services
{
    public static class RocketlaunchRomMap
    {
        public static void CreateGamesIni(Games gamesList, string gamesIniPath)
        {
            var iniEndPath = new DirectoryInfo(gamesIniPath);
            var fi = new FileInfo(gamesIniPath + "\\games.ini");
            iniEndPath.Attributes &= FileAttributes.Normal;

            if (File.Exists(fi.FullName))
                fi.Attributes &= ~FileAttributes.ReadOnly;

            using (StreamWriter file = new StreamWriter(gamesIniPath + "\\games.ini", false))
            {
                file.WriteLine("# This file is only used for remapping specific games to other Emulators and/or Systems.");
                file.WriteLine("# If you don't want your game to use the Default_Emulator, you would set the Emulator key here.");
                file.WriteLine("# This file can also be used when you have Wheels with games from other Systems.");
                file.WriteLine("# You would then use the System key to tell HyperLaunch what System to find the emulator settings.");
                file.WriteLine("");
                foreach (var game in gamesList)
                {
                    file.WriteLine("[{0}]", game.RomName);
                    file.WriteLine(@"System={0}", game.System);
                }
            }
        }

        public static Dictionary<string, string> GamesIniDict(string gamesIniPath)
        {            

            IniFile ini = new IniFile();

            ini.Load(gamesIniPath);

            var dict = new Dictionary<string, string>();

            foreach (var game in ini.Sections)
            {
                dict.Add(game.ToString(),ini.GetKeyValue(game.ToString(), "System"));
            }
            
            return dict;

        }
    }
}
