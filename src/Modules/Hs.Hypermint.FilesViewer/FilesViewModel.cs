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

namespace Hs.Hypermint.FilesViewer
{
    public class FilesViewModel : ViewModelBase
    {
        private string selectedGameName;
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
            try
            {
                var mediaFiles = new List<MediaFile>();
                Files = new ListCollectionView(mediaFiles);

                SelectedGameName = HeaderAndGame[0] + " Files for: " + HeaderAndGame[1];

                var filesInFolder = _rlAuditer.GetFilesForMedia(_selectedSrv.CurrentSystem,
                HeaderAndGame[1], _settingsRepo.HypermintSettings.RlMediaPath, HeaderAndGame[0]);

                if (filesInFolder == null || filesInFolder.Length == 0)
                {
                    _eventAggregator.GetEvent<SetMediaFileRlEvent>().Publish("");
                    return;
                }

                foreach (var item in filesInFolder)
                {
                    mediaFiles.Add(new MediaFile()
                    { Name = Path.GetFileNameWithoutExtension(item), Extension = Path.GetExtension(item),FullPath = item });
                }

                Files = new ListCollectionView(mediaFiles);

                // Display media file
                var fileToDisplay = Files.CurrentItem as MediaFile;
                _eventAggregator.GetEvent<SetMediaFileRlEvent>().Publish(fileToDisplay.FullPath);

            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(ex.Message);
            }


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
    }


}
