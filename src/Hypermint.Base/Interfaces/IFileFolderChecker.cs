using System.IO;

namespace Hypermint.Base.Interfaces
{
    public interface IFileFolderChecker
    {
        bool CheckMediaFolderFiles(string fullpath, string extFilter);

        bool FileExists(string filenamePath);

        bool DirectoryExists(string directoryPath);

        string CombinePath(string[] paths);

        string GetFileNameNoExt(string file);

        string[] GetFiles(string path, string filter = "");

        FileStream CreateFile(string path);

        void CreateDirectory(string path);

    }
}
