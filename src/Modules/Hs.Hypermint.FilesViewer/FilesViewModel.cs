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

namespace Hs.Hypermint.FilesViewer
{
    public class FilesViewModel : ViewModelBase
    {
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

        public FilesViewModel(IEventAggregator eventAggregator, IAuditerRl auditRl,
            ISelectedService selectedSrv, ISettingsRepo settings)
        {
            _eventAggregator = eventAggregator;
            _rlAuditer = auditRl;
            _selectedSrv = selectedSrv;
            _settingsRepo = settings;

            _eventAggregator.GetEvent<UpdateFilesEvent>().Subscribe(UpdateFiles);
        }

        private void UpdateFiles(string[] HeaderAndGame)
        {
            _systemName = _selectedSrv.CurrentSystem;
            _romName = HeaderAndGame[1];
            _mediaType = HeaderAndGame[0];

            try
            {
                UpdateMediaFiles();

                UpdateRlFolders();

                SelectedGameName = HeaderAndGame[0] + " Files for: " + HeaderAndGame[1];

                ShowMediaPaneSelectedFile();

            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(ex.Message);
            }

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

            var gameMediaRootPath = Path.Combine(
                _settingsRepo.HypermintSettings.RlMediaPath,
                _mediaType, _systemName, _romName);

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

        private void ShowMediaPaneSelectedFile()
        {
            // Display media file
            try
            {
                var fileToDisplay = Files.CurrentItem as MediaFile;
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
