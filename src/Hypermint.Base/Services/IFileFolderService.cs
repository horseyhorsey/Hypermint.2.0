namespace Hypermint.Base.Services
{
    public interface IFileFolderService
    {        
        string SelectedFolder { get; set; }

        void setFolderDialog();

        string setFileDialog(string initialDirectory = "");
    }
}
