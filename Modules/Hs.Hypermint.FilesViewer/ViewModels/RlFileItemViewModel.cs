using Hypermint.Base.Model;
using System.Collections.Generic;
using System.IO;

namespace Hs.Hypermint.FilesViewer.ViewModels
{
    /// <summary>
    /// File for rocketlauncher
    /// </summary>
    /// <seealso cref="Hypermint.Base.Model.TreeItemViewModel" />
    public class RlFileItemViewModel : TreeItemViewModel
    {
        public RlFileItemViewModel(string displayName, bool isDirectory = false) : base(displayName)
        {
            IsDirectory = isDirectory;
            FullPath = displayName;
            this.IsExpanded = true;
            Children = new List<RlFileItemViewModel>();

            //Make display name short
            if (isDirectory)
                DisplayName = Path.GetFileNameWithoutExtension(displayName);
            else
                DisplayName = Path.GetFileName(displayName);            
        }

        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }

        public IList<RlFileItemViewModel> Children { get; set; }
    }
}
