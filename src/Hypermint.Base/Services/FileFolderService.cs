using System;
using System.Windows.Forms;

namespace Hypermint.Base.Services
{
    public class FileFolderService : IFileFolderService
    {

        private string setFolder;
        public string SelectedFolder
        {
            get
            {
                return setFolder;
            }
            set
            {
                setFolder = value;
            }
        }

        public string setFileDialog(string initialDirectory = "")
        {
            var fileBrowserDialog = new OpenFileDialog();

            if (!string.IsNullOrEmpty(initialDirectory))
                fileBrowserDialog.InitialDirectory = initialDirectory + "\\Settings";

            var result = fileBrowserDialog.ShowDialog();
            
            if (result == DialogResult.OK)
                return fileBrowserDialog.FileName;
            else return "";

            
        }

        public void setFolderDialog()
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
                SelectedFolder = folderBrowserDialog.SelectedPath;

        }

    }
}
