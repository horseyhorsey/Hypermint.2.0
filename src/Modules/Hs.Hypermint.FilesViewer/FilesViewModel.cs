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
using System.Windows;
using Hypermint.Base.Constants;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using Hs.Hypermint.FilesViewer.Dialog;

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

        private string selectedFolder;
        public string SelectedFolder
        {
            get { return selectedFolder; }
            set { SetProperty(ref selectedFolder, value); }
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
        private string _currentPath;

        private IEventAggregator _eventAggregator;
        private IAuditerRl _rlAuditer;
        private ISelectedService _selectedSrv;
        private ISettingsRepo _settingsRepo;
        private IFolderExplore _folderService;
        private IDialogCoordinator _dialogService;
        CustomDialog customDialog;
        public DelegateCommand CloseDialogCommand { get; set; }

        public FilesViewModel(IEventAggregator eventAggregator, IAuditerRl auditRl, IDialogCoordinator dialogService,
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

            _dialogService = dialogService;

            CloseDialogCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, customDialog);
                
            });

        }


        public void DragOver(IDropInfo dropInfo)
        {

            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null;
            }) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public async void Drop(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null && extension.Equals(".*");
            }) ? DragDropEffects.Copy : DragDropEffects.None;

            _currentPath = GetSelectedPath();

            foreach (var file in dragFileList)
            {
                var newFileName = await DroppedFileAsync(file);

                if (newFileName != null)
                    DroppedFile(file, _currentPath + "\\" + SelectedFolder, newFileName);

            }                       

            UpdateMediaFiles(SelectedFolder);

            if (Files != null)
            {
                Files.CurrentChanged += Files_CurrentChanged;

                if (Files.CurrentItem == null)
                    _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish("");
                else
                    Files_CurrentChanged(null, null);
            }

        }

        private async Task<string> DroppedFileAsync(string file)
        {
            var settings = new MetroDialogSettings();
            settings.AffirmativeButtonText = "Copy";
            settings.NegativeButtonText = "Skip";                        

            var message = "File: " + file;
            var pathToCopyTo = _currentPath + "\\" + SelectedFolder;

            settings.DefaultText = Path.GetFileName(file);

            return await _dialogService.ShowInputAsync(this, pathToCopyTo, file, settings);  
            
        }

        public void DroppedFile(string file, string pathToCopy, string newFileName)
        {
            int i = 0;
            string Filename, ext, filename3, path;          

                Filename = System.IO.Path.GetFileName(newFileName);
                ext = System.IO.Path.GetExtension(newFileName);
                filename3 = System.IO.Path.GetFileNameWithoutExtension(newFileName);

                path = null;
                path = pathToCopy + "\\" + filename3 + ext;

                if (!Directory.Exists(pathToCopy))
                    Directory.CreateDirectory(pathToCopy);


            while (File.Exists(path))
            {
                path = pathToCopy + "\\" + filename3 + i + ext;
                i++;
            }

            System.IO.File.Copy(file, path);
        }

        private string GetRlGameMediaPath(string rlMediaPath, string mediaType,
            string systemName, string romName, string parentMediaType = "")
        {
            if (parentMediaType != "")
            {
                if (mediaType == "Screenshots")
                    return Path.Combine(rlMediaPath, parentMediaType, systemName, romName, mediaType);
                else
                    return Path.Combine(rlMediaPath, parentMediaType, systemName, romName);
            }
            else
                return Path.Combine(rlMediaPath, mediaType, systemName, romName);
        }

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
                UpdateRlFolders();

                UpdateMediaFiles();

            }
            catch (Exception ex)
            {                
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(ex.Message);
            }

            //Folders_CurrentChanged(null, null);

            ShowMediaPaneSelectedFile();            

        }

        private void SetSelectedClearFiles(string[] HeaderAndGame)
        {
            _systemName = _selectedSrv.CurrentSystem;
            _romName = HeaderAndGame[1];
            _mediaType = HeaderAndGame[0];            

            SelectedGameName = HeaderAndGame[0] + " Files for: " + HeaderAndGame[1];
            SelectedFolder = "";

            Files = null;
            Folders = null;
        }

        private void UpdateMediaFiles(string addFolder = "")
        {
            var mediaFiles = new List<MediaFile>();
            Files = new ListCollectionView(mediaFiles);

            if (addFolder == "Root")
                addFolder = "";

            string[] filesInFolder;
            try
            {
                filesInFolder = _rlAuditer.GetFilesForMedia(_systemName,
                _romName,
                _settingsRepo.HypermintSettings.RlMediaPath,
                _mediaType, addFolder);
            }
            catch (Exception) { return; }
                 

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

            string gameMediaRootPath = GetSelectedPath();

            _currentPath = gameMediaRootPath;

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
                _mediaType);

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

            Folders.CurrentChanged += Folders_CurrentChanged;

        }

        private string GetSelectedPath()
        {
            string parentMediaType = GetParentMediaType();

            if (_mediaType == "Saved Game")
                _mediaType = "Saved Games";
            if (_mediaType == "Guide")
                _mediaType = "Guides";

            var gameMediaRootPath =
            GetRlGameMediaPath(_settingsRepo.HypermintSettings.RlMediaPath, _mediaType, _systemName, _romName, parentMediaType);

            return gameMediaRootPath;
        }

        private string GetParentMediaType()
        {
            var parent = _mediaType;

            switch (parent)
            {
                case "Screenshots":
                    parent = "Artwork";
                    break;
                case "_Default Folder":
                    parent = "Fade";
                    break;
                case "Bezel":
                case "BezelBg":
                case "Cards":
                    parent = "Bezels";
                    break;
                case "Layer 1":
                case "Layer 2":
                case "Layer 3":
                    parent = "Fade";
                    break;
                default:
                    parent = "";
                    break;
            }
                
            return parent;
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
            if (Folders != null)
            {
                var folder = Folders.CurrentItem as MediaFolder;

                if (folder != null)
                {                    
                    SelectedFolder = folder.Name;
                    if (SelectedFolder.EndsWith("Root"))
                        SelectedFolder = SelectedFolder.Remove(SelectedFolder.Length - 4, 4);
                    UpdateMediaFiles(SelectedFolder);
                }

            }

            ShowMediaPaneSelectedFile();
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
