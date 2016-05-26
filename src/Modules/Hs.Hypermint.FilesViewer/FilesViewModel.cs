using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Events;
using System.Collections.Generic;
using System;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using System.IO;
using System.ComponentModel;
using System.Windows.Data;
using Prism.Commands;
using System.Linq;
using Hypermint.Base.Events;
using GongSolutions.Wpf.DragDrop;

namespace Hs.Hypermint.FilesViewer
{
    public class FilesViewModel : ViewModelBase, IDropTarget
    {
        public DelegateCommand OpenFolderCommand { get; private set; } 

        private string selectedGameName = "Files for:";
        public string SelectedGameName
        {
            get { return selectedGameName; }
            set { SetProperty(ref selectedGameName, value); }
        }

        private ICollectionView files;
        public ICollectionView Files
        {
            get { return files; }
            set { SetProperty(ref files, value); }
        }

        private ICollectionView folders;
        public ICollectionView Folders
        {
            get { return folders; }
            set { SetProperty(ref folders, value); }
        }

        private string _systemName;
        private string _mediaType;
        private string _romName;

        private IEventAggregator _eventAggregator;
        private IAuditerRl _rlAuditer;
        private ISelectedService _selectedSrv;
        private ISettingsRepo _settingsRepo;
        private IFolderExplore _folderService;

        public FilesViewModel(IEventAggregator eventAggregator, IAuditerRl auditRl,
            ISelectedService selectedSrv, ISettingsRepo settings, IFolderExplore folderExplore)
        {
            _eventAggregator = eventAggregator;
            _rlAuditer = auditRl;
            _selectedSrv = selectedSrv;
            _settingsRepo = settings;
            _folderService = folderExplore;

            _eventAggregator.GetEvent<UpdateFilesEvent>().Subscribe(UpdateFiles);

            OpenFolderCommand = new DelegateCommand(() =>
            {
                var folder = Folders.CurrentItem as MediaFolder;

                _folderService.OpenFolder(folder.FullPath);

            });

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(x =>
            {
                ResetViews();
            });

        }

        //public void DragOver(IDropInfo dropInfo)
        //{
        //    System.Windows.MessageBox.Show("");
        //    //var sourceItem = dropInfo.Data as MainMenu;
        //    //var targetItem = dropInfo.TargetItem as MainMenu;

        //    //if (sourceItem != null && targetItem != null)
        //    //{
        //    //    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        //    //    dropInfo.Effects = DragDropEffects.Copy;
        //    //}

        //}

        //public void Drop(IDropInfo dropInfo)
        //{
        //    System.Windows.MessageBox.Show("");
        //    //var sourceItem = dropInfo.Data as MainMenu;
        //    //var targetItem = dropInfo.TargetItem as MainMenu;

        //    //var AddInIndex = _mainMenuRepo.Systems.IndexOf(targetItem);

        //    //if (AddInIndex == 0)
        //    //    AddInIndex = 1;

        //    //_mainMenuRepo.Systems.Remove(sourceItem);
        //    //_mainMenuRepo.Systems.Insert(AddInIndex, sourceItem);
        //}

        private void ResetViews()
        {
            var mediaFiles = new List<MediaFile>();
            Files = new ListCollectionView(mediaFiles);

            var folders = new List<MediaFolder>();
            Folders = new ListCollectionView(folders);

            SelectedGameName = "";
        }

        private void UpdateFiles(string[] HeaderAndGame)
        {
            SetSelectedClearFiles(HeaderAndGame);

            try
            {
                UpdateMediaFiles();

                UpdateRlFolders();

            }
            catch (Exception ex)
            {                
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(ex.Message);
            }

            ShowMediaPaneSelectedFile();

        }

        private void SetSelectedClearFiles(string[] HeaderAndGame)
        {
            _systemName = _selectedSrv.CurrentSystem;
            _romName = HeaderAndGame[1];
            _mediaType = HeaderAndGame[0];
            
            SelectedGameName = HeaderAndGame[0] + " Files for: " + HeaderAndGame[1];

            Files = null;
            Folders = null;
        }

        private void UpdateMediaFiles(string addFolder = "")
        {
            var mediaFiles = new List<MediaFile>();
            Files = new ListCollectionView(mediaFiles);            

            var filesInFolder =
                _rlAuditer.GetFilesForMedia(
                _systemName,
                _romName,
                _settingsRepo.HypermintSettings.RlMediaPath,
                _mediaType,addFolder);

            if (filesInFolder == null || filesInFolder.Length == 0)
            {
                _eventAggregator.GetEvent<SetMediaFileRlEvent>().Publish("");

                return;
            }            

            foreach (var item in filesInFolder)
            {
                mediaFiles.Add(new MediaFile()
                { Name = Path.GetFileNameWithoutExtension(item), Extension = Path.GetExtension(item), FullPath = item });
            }

            Files = new ListCollectionView(mediaFiles);

            Files.CurrentChanged += Files_CurrentChanged;

        }

        private void UpdateRlFolders()
        {
            var folders = new List<MediaFolder>();

            Folders = new ListCollectionView(folders);

            var newMediaType = _mediaType;
            if (_mediaType == "_Default Folder")
                newMediaType = "Fade";

            var gameMediaRootPath = Path.Combine(
                _settingsRepo.HypermintSettings.RlMediaPath,
                newMediaType, _systemName, _romName);

            if (!Directory.Exists(gameMediaRootPath)) return;

            folders.Add(new MediaFolder
            {
                FullPath = gameMediaRootPath,
                Name = "Root"
            });

            var foldersInFolder =
                _rlAuditer.GetFoldersForMediaColumn(
                _systemName,
                _romName,
                _settingsRepo.HypermintSettings.RlMediaPath,
                newMediaType);
            
            if (foldersInFolder != null)
            {
                foreach (var item in foldersInFolder)
                {
                    folders.Add(new MediaFolder
                    {
                        FullPath = item,
                        Name = Path.GetFileName(item)
                    });
                }
            }

            Folders = new ListCollectionView(folders);

            //Folders_CurrentChanged(null, null);

            Folders.CurrentChanged += Folders_CurrentChanged;

        }

        private void ShowMediaPaneSelectedFile()
        {
            // Display media file
            try
            {
                var fileToDisplay = Files.CurrentItem as MediaFile;

                if (fileToDisplay == null)
                    _eventAggregator.GetEvent<SetMediaFileRlEvent>().Publish("");
                else
                    _eventAggregator.GetEvent<SetMediaFileRlEvent>().Publish(fileToDisplay.FullPath);
            }
            catch (NullReferenceException ex)
            {
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(ex.Message);
            }

        }

        private void Files_CurrentChanged(object sender, EventArgs e)
        {
            ShowMediaPaneSelectedFile();
        }

        private void Folders_CurrentChanged(object sender, EventArgs e)
        {
            var folder = Folders.CurrentItem as MediaFolder;

            if (folder.Name == "Root")
                UpdateMediaFiles();
            else
                UpdateMediaFiles(folder.Name);

            ShowMediaPaneSelectedFile();
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public class MediaFile
        {
            public string Name { get; set; }
            public string Extension { get; set; }
            public string FullPath { get; set; }

            public override string ToString()
            {
                return Name + Extension;
            }
        }

        public class MediaFolder
        {
            public string Name { get; set; }
            public string FullPath { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
    }


}
