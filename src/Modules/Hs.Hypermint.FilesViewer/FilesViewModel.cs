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
using Hs.Hypermint.Services.Helpers;
using System.Runtime.CompilerServices;

namespace Hs.Hypermint.FilesViewer
{
    public class FilesViewModel : ViewModelBase, IDropTarget
    {
        public DelegateCommand OpenFolderCommand { get; private set; }
        public DelegateCommand SaveNewFileCommand { get; private set; }
        public DelegateCommand CloseDialogCommand { get; set; }
        public DelegateCommand CloseAllPendingFileDropCommand { get; set; }

        #region Properties
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

        private string fileNameToSave;
        public string FileNameToSave
        {
            get { return fileNameToSave; }
            set { SetProperty(ref fileNameToSave, value); }
        }

        private bool convertEnabled = false;
        public bool ConvertEnabled
        {
            get { return convertEnabled; }
            set { SetProperty(ref convertEnabled, value); }
        }

        private bool imageConvertEnabled = false;
        public bool ImageConvertEnabled
        {
            get { return imageConvertEnabled; }
            set { SetProperty(ref imageConvertEnabled, value); }
        }

        private bool isFadeOptions;
        public bool IsFadeOptions
        {
            get { return isFadeOptions; }
            set { SetProperty(ref isFadeOptions, value); }
        }

        private bool fileNameOptionsOn;
        public bool FileNameOptionsOn
        {
            get { return fileNameOptionsOn; }
            set { SetProperty(ref fileNameOptionsOn, value); }
        }

        private bool fileNameOptionsOff = true;
        public bool FileNameOptionsOff
        {
            get { return fileNameOptionsOff; }
            set { SetProperty(ref fileNameOptionsOff, value); }
        }

        private bool formatJpg = true;
        public bool FormatJpg
        {
            get { return formatJpg; }
            set { SetProperty(ref formatJpg, value); }
        }

        private string droppedFileName;
        public string DroppedFileName
        {
            get { return droppedFileName; }
            set { SetProperty(ref droppedFileName, value); }
        }

        private string mediaType;
        public string MediaType
        {
            get { return mediaType; }
            set { SetProperty(ref mediaType, value); }
        }

        private string ratio;
        public string Ratio
        {
            get { return ratio; }
            set { SetProperty(ref ratio, value); }
        }

        private string author;
        public string Author
        {
            get { return author; }
            set { SetProperty(ref author, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private string outputFileName;
        public string OutputFileName
        {
            get { return outputFileName; }
            set { SetProperty(ref outputFileName, value); }
        }

        private ICollectionView cardPositions;
        public ICollectionView CardPositionsArray
        {
            get { return cardPositions; }
            set { SetProperty(ref cardPositions, value); }
        }
        #endregion       

        #region Fields
        private string _systemName;
        private string _romName;
        private string _currentPath;
        CustomDialog customDialog;
        #endregion

        #region Services
        private IEventAggregator _eventAggregator;
        private IAuditerRl _rlAuditer;
        private ISelectedService _selectedSrv;
        private ISettingsRepo _settingsRepo;
        private IFolderExplore _folderService;
        private IDialogCoordinator _dialogService;
        private IImageEditService _imageEdit;
        private bool cancelPending;
        #endregion

        public FilesViewModel(IEventAggregator eventAggregator, IAuditerRl auditRl, IDialogCoordinator dialogService,
            ISelectedService selectedSrv,
            ISettingsRepo settings,
            IFolderExplore folderExplore,
            IImageEditService imageEdit)
        {
            _eventAggregator = eventAggregator;
            _rlAuditer = auditRl;
            _selectedSrv = selectedSrv;
            _settingsRepo = settings;
            _folderService = folderExplore;
            _imageEdit = imageEdit;
            _dialogService = dialogService;

            _eventAggregator.GetEvent<UpdateFilesEvent>().Subscribe(UpdateFiles);

            Author = _settingsRepo.HypermintSettings.Author;

            OpenFolderCommand = new DelegateCommand(() =>
            {
                var folder = Folders.CurrentItem as MediaFolder;

                _folderService.OpenFolder(folder.FullPath);

            });

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(x =>
            {
                ResetViews();
            });

            CloseDialogCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, customDialog);

            });

            SaveNewFileCommand = new DelegateCommand(async () =>
            {
                try
                {
                    if (SelectedFolder.EndsWith("Root"))
                        SelectedFolder = SelectedFolder.Remove(SelectedFolder.Length - 4, 4);

                    ProcessFile(DroppedFileName, _currentPath + "//" + SelectedFolder, FileNameToSave);
                }
                catch (Exception) { }

                await _dialogService.HideMetroDialogAsync(this, customDialog);
            });

            CloseAllPendingFileDropCommand = new DelegateCommand(async () =>
            {
                cancelPending = true;

                await _dialogService.HideMetroDialogAsync(this, customDialog);

                cancelPending = false;
            });

            CardPositionsArray = new ListCollectionView(Enum.GetNames(typeof(CardPositions)));

            CardPositionsArray.MoveCurrentToFirst();

        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            try
            {
                switch (propertyName)
                {
                    case "Description":
                    case "Ratio":
                    case "Author":
                        if (MediaType != "Cards")
                            FileNameToSave =
                                RlStaticMethods
                                .CreateFileNameForRlImage(MediaType, Ratio, Description, Author);
                        else
                            FileNameToSave =
                                RlStaticMethods
                                .CreateCardFileName(Description, Author,
                                (string)CardPositionsArray.CurrentItem);
                        break;
                }
            }
            catch (Exception)
            {
                
            }


        }

        #region DragDrop
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

            if (string.IsNullOrWhiteSpace(MediaType)) return;

            if (MediaType == "RomName" || MediaType == "Description") return;

            if (!Directory.Exists(_settingsRepo.HypermintSettings.RlMediaPath)) return;

            await ShowFilesDialogsAsync(dragFileList);

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

        private async Task ShowFilesDialogsAsync(IEnumerable<string> dragFileList)
        {
            _currentPath = RlStaticMethods.GetSelectedPath(
                _settingsRepo.HypermintSettings.RlMediaPath, MediaType, _systemName, _romName);

            foreach (var file in dragFileList)
            {
                InitFileDialog(file);

                await DroppedFileCustomDialogAsync(file);

                await customDialog.WaitUntilUnloadedAsync();

                if (cancelPending)
                    break;
            }

            DroppedFileName = ""; FileNameToSave = "";
        }

        /// <summary>
        /// Initialize dialog values, check if image
        /// </summary>
        /// <param name="file"></param>
        private void InitFileDialog(string file)
        {
            if (RlStaticMethods.GetMediaFormatFromFile(file) == "image")
                ConvertEnabled = true;
            else
                ConvertEnabled = false;

            FileNameToSave = Path.GetFileNameWithoutExtension(file);

            var parentType = RlStaticMethods.GetParentMediaType(MediaType);

            if (parentType == "Fade" || parentType == "Bezels")
            {
                IsFadeOptions = true;
                FileNameOptionsOn = true;
                FileNameOptionsOff = false;

                if (MediaType != "Cards")
                    FileNameToSave =
                        RlStaticMethods
                        .CreateFileNameForRlImage(MediaType, Ratio, Description, Author);
                else
                    FileNameToSave =
                        RlStaticMethods
                        .CreateCardFileName(Description, Author,
                        (string)CardPositionsArray.CurrentItem);
            }
            else
            {
                FileNameOptionsOn = false;
                FileNameOptionsOff = true;

                IsFadeOptions = false;
            }

            DroppedFileName = file;
        }

        private async Task DroppedFileCustomDialogAsync(string file)
        {
            var settings = new MetroDialogSettings();
            settings.ColorScheme = MetroDialogColorScheme.Theme;

            var title = string.Format(
                "RL Drop: {0} : {1} {2}",
                MediaType, _systemName, _romName);

            customDialog = new CustomDialog() { Title = title };

            customDialog.Content = new DroppedFilesView { DataContext = this };

            await _dialogService.ShowMetroDialogAsync(this, customDialog, settings);

        }

        public void ProcessFile(string file, string pathToCopy, string newFileName)
        {
            int i = 1;
            string path;

            var originalExt = Path.GetExtension(file);

            if (!Directory.Exists(pathToCopy))
                Directory.CreateDirectory(pathToCopy);

            var ext = ChangeImageExtension(originalExt);

            path = pathToCopy + "\\" + newFileName + ext;

            while (File.Exists(path))
            {
                path = pathToCopy + "\\" + newFileName + " (" + i + ")" + ext;
                i++;
            }

            if (ImageConvertEnabled && ext != originalExt)
                _imageEdit.ConvertImageFormat(file, path, FormatJpg);
            else
                File.Copy(file, path);
        }

        private string ChangeImageExtension(string ext)
        {
            if (ImageConvertEnabled)
            {
                if (FormatJpg)
                    ext = ".jpg";
                else
                    ext = ".png";
            }

            return ext;
        }
        #endregion        

        #region Methods

        private void ResetViews()
        {
            MediaType = "";

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
                MediaType, addFolder);
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

            _currentPath = RlStaticMethods.GetSelectedPath(
                _settingsRepo.HypermintSettings.RlMediaPath, MediaType, _systemName, _romName);

            if (!Directory.Exists(_currentPath)) return;

            folders.Add(new MediaFolder
            {
                FullPath = _currentPath,
                Name = "Root"
            });

            var foldersInFolder =
                _rlAuditer.GetFoldersForMediaColumn(
                _systemName,
                _romName,
                _settingsRepo.HypermintSettings.RlMediaPath,
                MediaType);

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

        private void SetSelectedClearFiles(string[] HeaderAndGame)
        {
            _systemName = _selectedSrv.CurrentSystem;
            _romName = HeaderAndGame[1];
            MediaType = HeaderAndGame[0];

            SelectedGameName = HeaderAndGame[0] + " Files for: " + HeaderAndGame[1];
            SelectedFolder = "";

            Files = null;
            Folders = null;
        }

        private void ShowMediaPaneSelectedFile()
        {
            // Display media file
            try
            {
                var fileToDisplay = Files.CurrentItem as MediaFile;

                _eventAggregator
                        .GetEvent<SetMediaFileRlEvent>()
                        .Publish("");

                if (fileToDisplay != null)
                {
                    if (fileToDisplay.Name.ToLower().Contains("bezel") && fileToDisplay.Extension.ToLower() == ".png")
                    {
                        if (RlStaticMethods.GetMediaFormatFromFile(fileToDisplay.FullPath) == "image")
                        {
                            _eventAggregator.GetEvent<NavigateMediaPaneRequestEvent>().Publish("BezelEditView");
                        }

                        _eventAggregator.GetEvent<SetBezelImagesEvent>()
                        .Publish(fileToDisplay.FullPath);
                    }
                    else
                    {
                        _eventAggregator.GetEvent<NavigateMediaPaneRequestEvent>().Publish("MediaPaneView");

                        _eventAggregator
                        .GetEvent<SetMediaFileRlEvent>()
                        .Publish(fileToDisplay.FullPath);
                    }

                }
            }
            catch (NullReferenceException ex)
            {
                _eventAggregator
                    .GetEvent<ErrorMessageEvent>().Publish(ex.Message);
            }

        }
        #endregion

        #region FilesFolderEvent
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
        #endregion

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

        public enum CardPositions
        {
            leftCenter,
            rightCenter,
            topCenter,
            bottomCenter,
            topLeft,
            bottomLeft,
            topRight,
            bottomRight,
        }
    }


}
