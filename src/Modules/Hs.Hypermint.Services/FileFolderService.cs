using Hypermint.Base.Interfaces;
using System.Diagnostics;
using System.IO;

namespace Hs.Hypermint.Services
{
    public class FileFolderService : IFolderExplore
    {
        public bool OpenFolder(string path)
        {            
            if (Directory.Exists(path))
            {                
                Process.Start(path);                               
                return true;
            }
            else
                return false;
        }
    }
}
