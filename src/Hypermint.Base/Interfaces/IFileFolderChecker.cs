namespace Hypermint.Base.Interfaces
{
    public interface IFileFolderChecker
    {
        bool CheckMediaFolderFiles(string fullpath, string extFilter);

        bool CheckForFile(string filenamePath);

    }
}
