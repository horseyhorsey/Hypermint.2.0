using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Services;
using Hypermint.Base.Interfaces;
using System.IO;

namespace Hs.Hypermint.NavBar.ViewModels
{
    public class NavBarViewModel : ViewModelBase
    {

        #region Constructors
        public NavBarViewModel(IRegionManager manager, IEventAggregator eventAggregator,
        ISelectedService selectedService, IFolderExplore folderExplore, ISettingsRepo settings)
        {
            _regionManager = manager;
            _eventAggregator = eventAggregator;
            _selectedService = selectedService;
            _folderExplore = folderExplore;
            _settings = settings;

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(ShowDatabaseOrSearchView);

            _eventAggregator.GetEvent<NavigateRequestEvent>().Subscribe(Navigate);

            _eventAggregator.GetEvent<NavigateMediaPaneRequestEvent>().Subscribe(NavigateMediaPane);

            _eventAggregator.GetEvent<RequestOpenFolderEvent>().Subscribe(x =>
            {
                _folderExplore.OpenFolder(x);
            });

            NavigateCommand = new DelegateCommand<string>(Navigate);

        }
        #endregion

        #region Fields
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private ISelectedService _selectedService;
        private IFolderExplore _folderExplore;
        private ISettingsRepo _settings;

        private string _systemName = "";
        #endregion

        #region Properties
        private string currentView = "Views: Database editor";
        /// <summary>
        /// Gets or sets the current view name.
        /// </summary>
        public string CurrentView
        {
            get { return currentView; }
            set { SetProperty(ref currentView, value); }
        }

        private bool isRlEnabled = false;
        /// <summary>
        /// Used to disable rocketlaunch if main menu selected.
        /// </summary>
        public bool IsRlEnabled
        {
            get { return isRlEnabled; }
            set { SetProperty(ref isRlEnabled, value); }
        }

        private bool isMainMenu = true;
        /// <summary>
        /// Show the search icon
        /// </summary>
        public bool IsMainMenu
        {
            get { return isMainMenu; }
            set { SetProperty(ref isMainMenu, value); }
        }

        private bool isNotMainMenu;
        /// <summary>
        /// Show the datbase Icon
        /// </summary>
        public bool IsNotMainMenu
        {
            get { return isNotMainMenu; }
            set { SetProperty(ref isNotMainMenu, value); }
        }
        
        #endregion

        #region Commands
        /// <summary>
        /// Navigate command for the navbar
        /// </summary>
        public DelegateCommand<string> NavigateCommand { get; set; }
        #endregion

        #region Support Methods

        /// <summary>
        /// Uses the RegionManager.RequestNavigate to show either the search view or database view.<para/>
        /// The Main menu shows the search view and any other system shows the database view.
        /// </summary>
        /// <param name="systemName"></param>
        private void ShowDatabaseOrSearchView(string systemName)
        {
            _systemName = systemName;

            NavigateMediaPane("MediaPaneView");

            //Check if a multi system file exists
            if (File.Exists(_settings.HypermintSettings.HsPath + @"\Databases\" + systemName + "\\_multiSystem"))
                _selectedService.IsMultiSystem = true;
            else
                _selectedService.IsMultiSystem = false;

            if (systemName.ToLower().Contains("main menu") || _selectedService.IsMultiSystem)
            {
                // Disable RL audit, main menu & multisystem
                IsRlEnabled = false;

                if (!_selectedService.IsMultiSystem)
                {
                    IsNotMainMenu = false;
                    IsMainMenu = true;
                    _regionManager.RequestNavigate("ContentRegion", "SearchView");
                    RemoveAllFilesRegionViews();
                }
                else
                {
                    IsNotMainMenu = true;
                    IsMainMenu = false;
                    _regionManager.RequestNavigate("ContentRegion", "DatabaseDetailsView");
                }
            }
            else
            {
                IsRlEnabled = true;
                IsMainMenu = false;
                IsNotMainMenu = true;

                _regionManager.RequestNavigate("FilesRegion", "DatabaseOptionsView");

                if (!_regionManager.Regions.ContainsRegionWithName("FilesRegion"))
                {
                    _regionManager.Regions.Add(RegionNames.FilesRegion, _regionManager.Regions["FilesRegion"]);
                }

                _regionManager.RequestNavigate("ContentRegion", "DatabaseDetailsView");

            }
        }

        /// <summary>
        /// Sets the current view and uses region manager requestNavigate.
        /// </summary>
        /// <param name="uri"></param>
        private void Navigate(string uri = "")
        {
            CurrentView = "Views: ";

            _eventAggregator.GetEvent<NavigateMediaPaneRequestEvent>().Publish("MediaPaneView");

            switch (uri)
            {
                case "DatabaseDetailsView":
                    if (_selectedService.CurrentSystem.ToLower().Contains("main menu"))
                    {
                        CurrentView += "Search view";
                        _regionManager.RequestNavigate("ContentRegion", "SearchView");
                        RemoveAllFilesRegionViews();
                    }
                    else
                    {
                        CurrentView += "Database editor";
                        _regionManager.RequestNavigate(RegionNames.FilesRegion, "DatabaseOptionsView");
                        _regionManager.RequestNavigate("ContentRegion", "DatabaseDetailsView");
                    }
                    break;
                case "HsMediaAuditView":
                    CurrentView += "Hyperspin media audit";
                    _regionManager.RequestNavigate(RegionNames.FilesRegion, "HyperspinFilesView");
                    break;
                case "RlMediaAuditView":
                    CurrentView += "RocketLaunch media audit";
                    _regionManager.RequestNavigate(RegionNames.FilesRegion, "FilesView");
                    break;
                case "IntroVideosView":
                    CurrentView += "Hyperspin video intros";
                    _regionManager.RequestNavigate(RegionNames.FilesRegion, "ProcessOptionsView");
                    break;
                case "MultiSystemView":
                    CurrentView += "Hyperspin multiple system generator";
                    RemoveAllFilesRegionViews();
                    break;
                case "SimpleWheelView":
                    CurrentView += "Simple wheel creator";
                    _regionManager.RequestNavigate("FilesRegion", "WheelProcessView");
                    break;
                case "StatsView":
                    CurrentView += "Rocketlaunch stats";
                    RemoveAllFilesRegionViews();
                    break;
                case "CreateImageView":
                    CurrentView += "Image edit";
                    _regionManager.RequestNavigate("FilesRegion", "ImagePresetView");
                    break;
                default:
                    break;
            }

            if (uri != "WebBrowseView" && uri != "DatabaseDetailsView")
                _regionManager.RequestNavigate("ContentRegion", uri);

        }

        /// <summary>
        /// Changes the media pane view.
        /// </summary>
        /// <param name="uri"></param>
        private void NavigateMediaPane(string view)
        {
            _regionManager.RequestNavigate(RegionNames.MediaPaneRegion, view);
        }

        /// <summary>
        /// Use to clear out the File Region of any views
        /// </summary>
        private void RemoveAllFilesRegionViews()
        {
            foreach (var view in _regionManager.Regions["FilesRegion"].Views)
            {
                _regionManager.Regions["FilesRegion"].Deactivate(view);
            }
        }

        #endregion

    }
}
