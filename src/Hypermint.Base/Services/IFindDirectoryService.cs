namespace Hypermint.Base.Services
{
    public interface IFindDirectoryService
    {        
        string SelectedFolder { get; set; }

        void setFolderDialog();
    }
}
