using System.Windows.Forms;

namespace Hypermint.Base.Services
{
    public class FindDirectoryService : IFindDirectoryService
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

        public void setFolderDialog()
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
                SelectedFolder = folderBrowserDialog.SelectedPath;

        }

    }
}
