using Hypermint.Base.Interfaces;
using System.Windows.Forms;

namespace Hypermint.Base.Services
{
    public class FileDialogHelper : IFileDialogHelper
    {
        #region Properties        
        public string SelectedFolder { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Opens a file dialog to pick file
        /// </summary>
        /// <param name="initialDirectory">The initial directory to open the dialog</param>
        /// <returns></returns>
        public string SetFileDialog(string initialDirectory = "")
        {
            var fileBrowserDialog = new OpenFileDialog();

            if (string.IsNullOrEmpty(initialDirectory))
                fileBrowserDialog.InitialDirectory = initialDirectory + "\\Settings";
            else
                fileBrowserDialog.InitialDirectory = initialDirectory;

            var result = fileBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
                return fileBrowserDialog.FileName;
            else return "";

        }

        /// <summary>
        /// Opens a folder browser and sets SelectedFolder
        /// </summary>
        public bool SetFolderDialog()
        {
            bool result = false;
            var folderBrowserDialog = new FolderBrowserDialog();
            var dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                SelectedFolder = folderBrowserDialog.SelectedPath;
                result = true;
            }
                        
            return result;
        }

        #endregion

    }
}
