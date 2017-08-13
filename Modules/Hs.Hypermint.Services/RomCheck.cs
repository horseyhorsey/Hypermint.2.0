using System.IO;

namespace Hs.Hypermint.Services
{
    public class RomCheck
    {
        public static bool RomExists(string[] romPaths, string[] romExts, string romName)
        {
            for (int p = 0; p < romPaths.Length; p++)
            {
                for (int e = 0; e < romExts.Length; e++)
                {
                    if (File.Exists(romPaths[p] + "\\" + romName + "." + romExts[e]))
                        return true;
                }
            }

            return false;
        }
    }
}
