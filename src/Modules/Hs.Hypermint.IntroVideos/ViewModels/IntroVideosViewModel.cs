using GongSolutions.Wpf.DragDrop;
using Hs.Hypermint.IntroVideos.Models;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using MediaToolkit.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class IntroVideosViewModel : ViewModelBase, IDropTarget
    {

        #region Constructors
        public IntroVideosViewModel(IRegionManager manager,
    IFileFolderChecker fileChecker,
    IEventAggregator ea, ISettingsRepo settings,
    ISelectedService selected)
        {
            _regionManager = manager;
            _fileFolderChecker = fileChecker;
            _eventAggregator = ea;
            _settings = settings;
            _selectedService = selected;

            processVideos = new List<IntroVideo>();
            scannedVideos = new List<IntroVideo>();
            VideoToProcessList = new ListCollectionView(processVideos);

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(x => SystemChanged(x));

            _eventAggregator.GetEvent<GetProcessVideosEvent>().Subscribe(x =>
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

            });

            RandomVideoCommand = new DelegateCommand(GrabRandomVideos);
            AddSelectedCommand = new DelegateCommand(AddVideos);
            ScanFormatCommand = new DelegateCommand(ScanFormat);

            try
            {
                RemoveVideosCommand = new DelegateCommand<string>(RemoveVideos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }


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
                        _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish(video.FileName);
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

        private int randomCount = 12;
        public int RandomCount
        {
            get { return randomCount; }
            set { SetProperty(ref randomCount, value); }
        }

        public int SelectedAvailableItemsCount { get; private set; }
        public int SelectedprocessItemsCount { get; private set; }

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

        #region Services
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IFileFolderChecker _fileFolderChecker;
        private ISettingsRepo _settings;
        private ISelectedService _selectedService;
        #endregion

        #region Commands
        public DelegateCommand RandomVideoCommand { get; private set; }
        public DelegateCommand<IList> SelectionAvailableChanged { get; set; }
        public DelegateCommand<IList> SelectionProcessChanged { get; set; }
        public DelegateCommand AddSelectedCommand { get; private set; }
        public DelegateCommand<string> RemoveVideosCommand { get; private set; }
        public DelegateCommand ScanFormatCommand { get; private set; }
        #endregion

        #region Public Methods

        public void DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as IntroVideo;
            var targetItem = dropInfo.TargetItem as IntroVideo;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }

        }

        public void Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as IntroVideo;
            var targetItem = dropInfo.TargetItem as IntroVideo;

            var AddInIndex = processVideos.IndexOf(targetItem);

            processVideos.Remove(sourceItem);
            processVideos.Insert(AddInIndex, sourceItem);

            VideoToProcessList = new ListCollectionView(processVideos);
        } 
        #endregion

        #region Support Methods

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

        private void ScanVideosForSystem(string hyperSpinVideoPath, string videoExtFilter = "*.mp4")
        {
            try
            {

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

                VideosAvailableHeader = "Videos Available: " + scannedVideos.Count;
            }
            catch (Exception)
            {
                
            }
            
        }

        private void GrabRandomVideos()
        {
            if (scannedVideos.Count <= 0) return;

            if (randomCount > scannedVideos.Count) return;

            try
            {
                // Add all videos
                if (randomCount == scannedVideos.Count)
                {
                    foreach (var item in scannedVideos)
                    {
                        processVideos.Add(item);
                        scannedVideos.Remove(item);
                    }                                        
                }
                else
                {
                    // Add random
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

        /// <summary>
        /// Remove selected or clears whole list
        /// Note: using a string for delegate command?
        /// </summary>
        /// <param name="removeSelected"></param>
        private void RemoveVideos(string removeSelected = "true")
        {
            if (processVideos.Count <= 0) return;

            if (removeSelected == "true")
            {
                if (SelectedProcessVideos.Count <= 0) return;

                foreach (var video in SelectedProcessVideos)
                {
                    scannedVideos.Add(video);
                    processVideos.Remove(video);
                }
            }
            else
            {

                foreach (IntroVideo video in VideoToProcessList)
                {
                    scannedVideos.Add(video);
                }

                processVideos.Clear();
            }


            VideoList.Refresh();
            VideoToProcessList.Refresh();
        }
        private void ScanFormat()
        {
            foreach (var video in processVideos)
            {
                try
                {
                    if (video.FrameRate == 0)
                    {
                        if (_fileFolderChecker.FileExists(video.FileName))
                        {
                            var inputFile = new MediaFile { Filename = video.FileName };
                            var GetFrameSize = "";

                            try
                            {
                                using (var engine = new MediaToolkit.Engine())
                                {
                                    engine.GetMetadata(inputFile);
                                    video.Format = inputFile.Metadata.VideoData.FrameSize;
                                    video.FrameRate = inputFile.Metadata.VideoData.Fps;
                                    video.Duration = inputFile.Metadata.Duration.Duration();
                                    engine.Dispose();
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                    System.Windows.MessageBox.Show(ex.InnerException.Message);

                }

            }

            VideoToProcessList.Refresh();

        }
        #endregion
    }
}
