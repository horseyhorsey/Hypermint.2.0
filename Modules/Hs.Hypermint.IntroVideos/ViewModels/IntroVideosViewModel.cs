using Hypermint.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Model;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class IntroVideosViewModel : ViewModelBase
    {
        #region Fields
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IFileFolderChecker _fileFolderChecker;
        private ISettingsHypermint _settings;
        private ISelectedService _selectedService;
        #endregion

        #region Constructors

        public IntroVideosViewModel(IRegionManager manager,
            IFileFolderChecker fileChecker,
            IEventAggregator ea, ISettingsHypermint settings,
            ISelectedService selected)
        {
            //Init Services
            _regionManager = manager;
            _fileFolderChecker = fileChecker;
            _eventAggregator = ea;
            _settings = settings;
            _selectedService = selected;

            //Init collections
            processVideos = new List<IntroVideo>();
            scannedVideos = new List<IntroVideo>();
            VideoToProcessList = new ListCollectionView(processVideos);

            //Events
            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(x => SystemChanged(x));
            _eventAggregator.GetEvent<GetProcessVideosEvent>().Subscribe(x =>
            {
                OnGetProcessVideos();

            });

            //Commands            
            //AddSelectedCommand = new DelegateCommand(AddVideos);
            //ScanFormatCommand = new DelegateCommand(ScanFormat);

            SelectionAvailableChanged = new DelegateCommand<IList>(items =>
            {
                OnVideoSelectionAvailableChanged(items);
            });

            SelectionProcessChanged = new DelegateCommand<IList>(items =>
            {
                OnVideoSelectionChanged(items);
            });

            //What?
            try
            {
                //RemoveVideosCommand = new DelegateCommand<string>(RemoveVideos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        #endregion

        #region Properties      

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

        private string videosAvailableHeader = "Videos Available";
        public string VideosAvailableHeader
        {
            get { return videosAvailableHeader; }
            set { SetProperty(ref videosAvailableHeader, value); }
        }

        public List<IntroVideo> SelectedAvailableVideos = new List<IntroVideo>();
        public List<IntroVideo> SelectedProcessVideos = new List<IntroVideo>();

        #region Collections
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
        #endregion    

        #endregion        

        #region Commands
        public DelegateCommand<IList> SelectionAvailableChanged { get; set; }
        public DelegateCommand<IList> SelectionProcessChanged { get; set; }
        public DelegateCommand AddSelectedCommand { get; private set; }
        public DelegateCommand<string> RemoveVideosCommand { get; private set; }
        public DelegateCommand ScanFormatCommand { get; private set; }
        #endregion

        #region Public Methods

        #endregion

        #region Support Methods

        private void OnGetProcessVideos()
        {
            if (processVideos.Count > 0)
            {
                string[] vids = new string[processVideos.Count];
                for (int i = 0; i < processVideos.Count; i++)
                {
                    vids[i] = processVideos[i].FileName;
                }

                _eventAggregator.GetEvent<ReturnProcessVideosEvent>()
                .Publish(vids);
            }
        }

        [Obsolete]
        private void OnVideoSelectionAvailableChanged(IList items)
        {
            //if (items == null)
            //{
            //    SelectedAvailableItemsCount = 0;
            //    SelectedAvailableVideos.Clear();
            //    return;
            //}
            //else
            //{
            //    SelectedAvailableItemsCount = items.Count;
            //}

            //try
            //{
            //    SelectedAvailableVideos.Clear();
            //    foreach (var item in items)
            //    {
            //        var video = item as IntroVideo;
            //        if (video.FileName != null)
            //            SelectedAvailableVideos.Add(video);
            //    }

            //    if (SelectedAvailableItemsCount > 1)
            //        SelectedAvailableHeader = "Selected videos: " + SelectedAvailableItemsCount;
            //    else if (SelectedAvailableItemsCount == 1)
            //    {
            //        var video = items[0] as IntroVideo;
            //        SelectedAvailableHeader = "Selected item: " + video.FileName;
            //        _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish(video.FileName);
            //    }
            //    else
            //        SelectedAvailableHeader = "";
            //}
            //catch (Exception)
            //{


            //}
        }

        [Obsolete]
        private void OnVideoSelectionChanged(IList items)
        {
            //if (items == null)
            //{
            //    SelectedprocessItemsCount = 0;
            //    SelectedProcessVideos.Clear();
            //    return;
            //}
            //else
            //{
            //    SelectedprocessItemsCount = items.Count;
            //}

            //try
            //{
            //    SelectedProcessVideos.Clear();
            //    foreach (var item in items)
            //    {
            //        var video = item as IntroVideo;
            //        if (video.FileName != null)
            //            SelectedProcessVideos.Add(video);
            //    }

            //    if (SelectedprocessItemsCount > 1)
            //        SelectedprocessHeader = "Selected videos: " + SelectedprocessItemsCount;
            //    else if (SelectedprocessItemsCount == 1)
            //    {
            //        var video = items[0] as IntroVideo;
            //        SelectedprocessHeader = "Selected item: " + video.FileName;
            //    }
            //    else
            //        SelectedprocessHeader = "";
            //}
            //catch (Exception)
            //{


            //}
        }

        /// <summary>
        /// Update the video list for this system
        /// </summary>
        /// <param name="systemName"></param>
        private void SystemChanged(string systemName)
        {
            try
            {
                processVideos.Clear();
                VideoToProcessList.Refresh();
                scannedVideos.Clear();

                var systemVideoPath = _settings.HypermintSettings.HsPath +
                    "\\Media\\" + systemName + "\\" + Root.Video;

                ScanVideosForSystem(systemVideoPath);
            }
            catch (Exception)
            {

            }

        }

        [Obsolete("Dont scan videos when system is clicked. Add option to do that in view.")]
        private void ScanVideosForSystem(string hyperSpinVideoPath, string videoExtFilter = "*.mp4")
        {
            //try
            //{

            //    var videoFiles =
            //        _fileFolderChecker.GetFiles(hyperSpinVideoPath + "\\", videoExtFilter);

            //    foreach (var video in videoFiles)
            //    {
            //        scannedVideos.Add(new IntroVideo()
            //        {
            //            FileName = video
            //        });
            //    }

            //    VideoList = new ListCollectionView(scannedVideos);

            //    VideosAvailableHeader = "Videos Available: " + scannedVideos.Count;
            //}
            //catch (Exception)
            //{

            //}            
        }

        #endregion
    }
}
