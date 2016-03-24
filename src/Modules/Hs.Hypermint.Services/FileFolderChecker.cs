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
        public bool FileExists(string filenamePath)
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

        public string CombinePath(string[] paths)
        {
            if (paths == null) return "";

            return Path.Combine(paths);
        }

        public bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);            
        }

        public string[] GetFiles(string path, string filter = "")
        {
            return Directory.GetFiles(path, filter);            
        }

        public string GetFileNameNoExt(string file)
        {
            return Path.GetFileNameWithoutExtension(file);
        }

        public FileStream CreateFile(string path)
        {
           return File.Create(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
