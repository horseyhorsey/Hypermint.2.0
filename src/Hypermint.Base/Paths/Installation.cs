using System.IO;
using System.Linq;

namespace Hypermint.Base.Paths
{
    public static class Installation
    {
        public static string HsPath { get; set; }        
        /// <summary>
        /// Search for "Hyperspin" or "Rocketlauncher" directory on hard drives
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string SearchForInstall(string frontend)
        {
            string foundPath = string.Empty;
            DriveInfo[] drives = DriveInfo.GetDrives();

            for (int i = 0; i < drives.Count(); i++)
            {
                if ((Directory.Exists(drives[i].Name + frontend) 
                    && (drives[0].DriveType != DriveType.CDRom)))
                {
                    foundPath = drives[i].Name + frontend;
                }
            }

            return foundPath;
        }
    }
}
