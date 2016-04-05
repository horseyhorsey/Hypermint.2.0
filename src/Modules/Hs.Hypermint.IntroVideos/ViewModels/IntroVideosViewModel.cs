using Hs.Hypermint.IntroVideos.Models;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Models;
using Hypermint.Base.Services;
using MediaToolkit.Model;
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
    public class IntroVideosViewModel : ViewModelBase
    {

        public IntroVideosViewModel(IRegionManager manager, IFileFolderChecker fileChecker,
            IEventAggregator ea, ISettingsRepo settings, IAviSynthScripter aviSythnScripter,
            ISelectedService selected, IFolderExplore folderexplorer)
        {
            _regionManager = manager;
            _fileFolderChecker = fileChecker;
            _eventAggregator = ea;
            _settings = settings;
            _selectedService = selected;
            _avisynthScripter = aviSythnScripter;
            _folderExplorer = folderexplorer;

            processVideos = new List<IntroVideo>();
            scannedVideos = new List<IntroVideo>();
            VideoToProcessList = new ListCollectionView(processVideos);

            AviSynthOptions = new AviSynthOption();

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(x => SystemChanged(x));

            RandomVideoCommand = new DelegateCommand(GrabRandomVideos);
            AddSelectedCommand = new DelegateCommand(AddVideos);
            ScanFormatCommand = new DelegateCommand(ScanFormat);
            SaveScriptCommand = new DelegateCommand(SaveScript);
            OpenExportFolderCommand = new DelegateCommand(() =>
            {
                _folderExplorer.OpenFolder("exports");
            });

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
        private ISelectedService _selectedService;
        private IAviSynthScripter _avisynthScripter;
        private IFolderExplore _folderExplorer;
        #endregion

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

        #region Properties      

        private AviSynthOption aviSynthOptions;
        public AviSynthOption AviSynthOptions
        {
            get { return aviSynthOptions; }
            set { SetProperty(ref aviSynthOptions, value); }
        }

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

        #region AviSynthProperties
        private bool overlay;
        public bool Overlay
        {
            get { return overlay; }
            set { SetProperty(ref overlay, value); }
        }
        private bool resizeOverlay;
        public bool ResizeOverlay
        {
            get { return resizeOverlay; }
            set { SetProperty(ref resizeOverlay, value); }
        }
        private int startFrame = 60;
        public int StartFrame
        {
            get { return startFrame; }
            set { SetProperty(ref startFrame, value); }
        }
        private int endFrame = 300;
        public int EndFrame
        {
            get { return endFrame; }
            set { SetProperty(ref endFrame, value); }
        }
        private int dissolveAmount = 2;
        public int DissolveAmount
        {
            get { return dissolveAmount; }
            set { SetProperty(ref dissolveAmount, value); }
        }
        private int fadeIn = 2;
        public int FadeIn
        {
            get { return fadeIn; }
            set { SetProperty(ref fadeIn, value); }
        }
        private int fadeOut = 2;
        public int FadeOut
        {
            get { return fadeOut; }
            set { SetProperty(ref fadeOut, value); }
        }
        private int resizeWidth = 200;
        public int ResizeWidth
        {
            get { return resizeWidth; }
            set { SetProperty(ref resizeWidth, value); }
        }
        private int resizeHeight = 50;
        public int ResizeHeight
        {
            get { return resizeHeight; }
            set { SetProperty(ref resizeHeight, value); }
        }
        private int wheelPosX = 3;
        public int WheelPosX
        {
            get { return wheelPosX; }
            set { SetProperty(ref wheelPosX, value); }
        }
        private int wheelPosY = 420;        
        public int WheelPosY
        {
            get { return wheelPosY; }
            set { SetProperty(ref wheelPosY, value); }
        }
        #endregion

        #region Commands
        public DelegateCommand RandomVideoCommand { get; private set; }
        public DelegateCommand<IList> SelectionAvailableChanged { get; set; }
        public DelegateCommand<IList> SelectionProcessChanged { get; set; }        
        public DelegateCommand AddSelectedCommand { get; private set; }
        public DelegateCommand<string> RemoveVideosCommand { get; private set; }
        public DelegateCommand ScanFormatCommand { get; private set; }
        public DelegateCommand SaveScriptCommand { get; private set; }
        public DelegateCommand OpenExportFolderCommand { get; private set; }
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

        private void SaveScript()
        {
            if (processVideos.Count < 2) return;            

            aviSynthOptions = new AviSynthOption()
            {
               DissolveAmount = DissolveAmount,
               StartFrame = StartFrame,
               EndFrame = EndFrame,
               FadeIn = FadeIn,
               FadeOut = FadeOut,
               ResizeHeight = ResizeHeight,
               ResizeWidth = ResizeWidth,
               WheelPosX = WheelPosX,
               WheelPosY = WheelPosY,              
                };

            string[] videos = new string[processVideos.Count];
            var i = 0;
            foreach (var video in processVideos)
            {
                videos[i] = video.FileName;
                i++;
            }

            var wheelPath = _settings.HypermintSettings.HsPath + "\\Media\\" + _selectedService.CurrentSystem + "\\" +
                Images.Wheels + "\\";

            _avisynthScripter.CreateScript(videos, aviSynthOptions, _selectedService.CurrentSystem, Overlay,ResizeOverlay, wheelPath);
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
