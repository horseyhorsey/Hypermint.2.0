namespace Hypermint.Base.Interfaces
{
    /// <summary>
    /// Can open a file dialog
    /// </summary>
    public interface IFileDialogHelper
    {        
        string SelectedFolder { get; set; }

        bool SetFolderDialog();

        string SetFileDialog(string initialDirectory = "");
    }
}
