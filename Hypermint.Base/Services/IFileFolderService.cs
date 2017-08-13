namespace Hypermint.Base.Services
{
    public interface IFileFolderService
    {        
        string SelectedFolder { get; set; }

        void SetFolderDialog();

        string SetFileDialog(string initialDirectory = "");
    }
}
