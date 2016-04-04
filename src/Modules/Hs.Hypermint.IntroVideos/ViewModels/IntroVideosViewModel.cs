using Hs.Hypermint.IntroVideos.Models;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class IntroVideosViewModel : BindableBase
    {

        public IntroVideosViewModel(IRegionManager manager, IFileFolderChecker fileChecker,
            IEventAggregator ea, ISettingsRepo settings)
        {
            _regionManager = manager;
            _fileFolderChecker = fileChecker;
            _eventAggregator = ea;
            _settings = settings;

            processVideos = new List<IntroVideo>();
            VideoToProcessList = new ListCollectionView(processVideos);

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(x => SystemChanged(x));

            RandomVideoCommand = new DelegateCommand(GrabRandomVideos);
            AddSelectedCommand = new DelegateCommand(AddVideos);
            RemoveVideosCommand = new DelegateCommand(RemoveVideos);

            SelectionAvailableChanged = new DelegateCommand<IList>(items =>
            {
                if (items == null)
                {
                    SelectedAvailableItemsCount = 0;
                    SelectedAvailableVideos.Clear();
                    return;
                }
                else
                {
                    SelectedAvailableItemsCount = items.Count;
                }

                try
                {
                    SelectedAvailableVideos.Clear();
                    foreach (var item in items)
                    {
                        var video = item as IntroVideo;
                        if (video.FileName != null)
                            SelectedAvailableVideos.Add(video);
                    }

                    if (SelectedAvailableItemsCount > 1)
                        SelectedAvailableHeader = "Selected videos: " + SelectedAvailableItemsCount;
                    else if (SelectedAvailableItemsCount == 1)
                    {
                        var video = items[0] as IntroVideo;
                        SelectedAvailableHeader = "Selected item: " + video.FileName;
                    }
                    else
                        SelectedAvailableHeader = "";
                }
                catch (Exception)
                {


                }
            });

            SelectionProcessChanged = new DelegateCommand<IList>(items =>
            {
                if (items == null)
                {
                    SelectedprocessItemsCount = 0;
                    SelectedProcessVideos.Clear();
                    return;
                }
                else
                {
                    SelectedprocessItemsCount = items.Count;
                }

                try
                {
                    SelectedProcessVideos.Clear();
                    foreach (var item in items)
                    {
                        var video = item as IntroVideo;
                        if (video.FileName != null)
                            SelectedProcessVideos.Add(video);
                    }

                    if (SelectedprocessItemsCount > 1)
                        SelectedprocessHeader = "Selected videos: " + SelectedprocessItemsCount;
                    else if (SelectedprocessItemsCount == 1)
                    {
                        var video = items[0] as IntroVideo;
                        SelectedprocessHeader = "Selected item: " + video.FileName;
                    }
                    else
                        SelectedprocessHeader = "";
                }
                catch (Exception)
                {


                }
            });

        }

        public List<IntroVideo> SelectedAvailableVideos = new List<IntroVideo>();
        public List<IntroVideo> SelectedProcessVideos = new List<IntroVideo>();

        #region Services
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IFileFolderChecker _fileFolderChecker;
        private ISettingsRepo _settings;
        #endregion

        #region Properties
        private ICollectionView videoList;
        public ICollectionView VideoList
        {
            get { return videoList; }
            set { SetProperty(ref videoList, value); }
        }

        private ICollectionView videoToProcessList;
        public ICollectionView VideoToProcessList
        {
            get { return videoToProcessList; }
            set { SetProperty(ref videoToProcessList, value); }
        }

        private List<IntroVideo> scannedVideos;
        private List<IntroVideo> processVideos;

        private string selectedAvailableHeader;
        public string SelectedAvailableHeader
        {
            get { return selectedAvailableHeader; }
            set { SetProperty(ref selectedAvailableHeader, value); }
        }

        private string selectedprocessHeader;
        public string SelectedprocessHeader
        {
            get { return selectedprocessHeader; }
            set { SetProperty(ref selectedprocessHeader, value); }
        }

        private int randomCount = 12;
        public int RandomCount
        {
            get { return randomCount; }
            set { SetProperty(ref randomCount, value); }
        }

        public int SelectedAvailableItemsCount { get; private set; }
        public int SelectedprocessItemsCount { get; private set; }
        
        #endregion

        #region Commands
        public DelegateCommand RandomVideoCommand { get; private set; }
        public DelegateCommand<IList> SelectionAvailableChanged { get; set; }
        public DelegateCommand<IList> SelectionProcessChanged { get; set; }        
        public DelegateCommand AddSelectedCommand { get; private set; }
        public DelegateCommand RemoveVideosCommand { get; private set; }
        #endregion

        #region Methods

        /// <summary>
        /// Update the video list for this system
        /// </summary>
        /// <param name="systemName"></param>
        private void SystemChanged(string systemName)
        {
            try
            {
                processVideos.Clear();

                var systemVideoPath = _settings.HypermintSettings.HsPath +
                    "\\Media\\" + systemName + "\\" + Root.Video;

                ScanVideosForSystem(systemVideoPath);
            }
            catch (Exception)
            {
                
            }
            
        }

        private void ScanVideosForSystem(string hyperSpinVideoPath, string videoExtFilter = "*.mp4")
        {
            try
            {
                scannedVideos = new List<IntroVideo>();

                var videoFiles =
                    _fileFolderChecker.GetFiles(hyperSpinVideoPath + "\\", videoExtFilter);

                foreach (var video in videoFiles)
                {
                    scannedVideos.Add(new IntroVideo()
                    {
                        FileName = video
                    });
                }

                VideoList = new ListCollectionView(scannedVideos);
            }
            catch (Exception)
            {
                
            }
            
        }

        private void GrabRandomVideos()
        {
            try
            {
                var random = new Random(scannedVideos.Count);

                for (int i = 0; i < randomCount; i++)
                {

                    var randomVideo = scannedVideos[random.Next(scannedVideos.Count)];

                    processVideos.Add(randomVideo);                    
                    scannedVideos.Remove(randomVideo);
                }

                VideoToProcessList.Refresh();
                VideoList.Refresh();

            }
            catch (Exception)
            {

            }
           
        }

        private void AddVideos()
        {
            if (SelectedAvailableVideos.Count <= 0) return;

            foreach (var item in SelectedAvailableVideos)
            {
                processVideos.Add(item);
                scannedVideos.Remove(item);
            }

            VideoList.Refresh();
            VideoToProcessList.Refresh();
        }

        private void RemoveVideos()
        {
            if (SelectedProcessVideos.Count <= 0) return;

            foreach (var item in SelectedProcessVideos)
            {
                scannedVideos.Add(item);
                processVideos.Remove(item);
            }

            VideoList.Refresh();
            VideoToProcessList.Refresh();
        }
        #endregion
    }
}
