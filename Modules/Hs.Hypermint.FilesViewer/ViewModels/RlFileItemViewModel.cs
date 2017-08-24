using Hypermint.Base.Model;
using Prism.Commands;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System;

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

            OpenFileFolderCommand = new DelegateCommand(OpenFileFolder);
        }

        private void OpenFileFolder()
        {
            
        }

        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }

        public IList<RlFileItemViewModel> Children { get; set; }

        public ICommand OpenFileFolderCommand { get; set; }
    }
}
