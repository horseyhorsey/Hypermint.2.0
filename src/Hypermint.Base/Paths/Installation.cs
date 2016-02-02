using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Paths
{
    public static class Installation
    {
        public static string HsPath { get; set; }        
        /// <summary>
        /// Search for "Hyperspin" or "Rocketlauncher" directory on hard drive
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string SearchForInstall()
        {
            string foundPath = string.Empty;
            DriveInfo[] drives = DriveInfo.GetDrives();

            for (int i = 0; i < drives.Count(); i++)
            {
                if ((Directory.Exists(drives[i].Name + @"HyperSpin") 
                    && (drives[0].DriveType != DriveType.CDRom)))
                {
                    foundPath = drives[i].Name + @"HyperSpin";
                }
            }

            return foundPath;
        }
    }
}
