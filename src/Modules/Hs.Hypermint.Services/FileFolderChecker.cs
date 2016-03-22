using Hypermint.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.Services
{
    public class FileFolderChecker : IFileFolderChecker
    {
        /// <summary>
        /// Check a given filename to exist
        /// </summary>
        /// <param name="filenamePath"></param>
        /// <returns></returns>
        public bool CheckForFile(string filenamePath)
        {
            if (File.Exists(filenamePath))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check all files in directory with given Extension
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="extFilter"></param>
        /// <returns></returns>
        public bool CheckMediaFolderFiles(string fullpath, string extFilter)
        {
            if (!Directory.Exists(fullpath))
                return false;

            string[] getFiles;
            getFiles = Directory.GetFiles(fullpath, extFilter);
            if (getFiles.Length != 0)
                return true;
            else return false;
        }
    }
}
