using System.IO;
using System.Linq;

namespace Hypermint.Base.Paths
{
    public static class Installation
    {
        /// <summary>
        /// Hyperspin path
        /// </summary>
        public static string HsPath { get; set; }      
          
        /// <summary>
        /// Search for a frontend install on all drives root
        /// </summary>
        /// <param name="frontend">Frontend to search for eg Hyperspin, Rocketlauncher</param>        
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
