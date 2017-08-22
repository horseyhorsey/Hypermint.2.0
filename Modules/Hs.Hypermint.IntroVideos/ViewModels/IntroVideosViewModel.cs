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
    }
}
