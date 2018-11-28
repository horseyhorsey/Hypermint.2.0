namespace Hypermint.Base.Interfaces
{
    public interface ITrashMaster
    {
        void RlFileToTrash(string fileName, string system, string mediaType, string romName);

        void HsFileToTrash(string fileName, string system, string mediaType, string romName);

        string GetHsTrashPath(string system, string mediaType);
    }
}
