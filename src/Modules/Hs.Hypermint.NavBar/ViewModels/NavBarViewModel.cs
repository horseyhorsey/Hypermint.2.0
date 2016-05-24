using System;
using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Services;

namespace Hs.Hypermint.NavBar.ViewModels
{
    public class NavBarViewModel : ViewModelBase
    {
        private string currentView = "Views: Database editor";
        public string CurrentView
        {
            get { return currentView; }
            set { SetProperty(ref currentView, value); }
        }

        private bool isNotMainMenu = false;
        public bool IsNotMainMenu
        {
            get { return isNotMainMenu; }
            set { SetProperty(ref isNotMainMenu, value); }
        }

        private string _systemName = "";

        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private ISelectedService _selectedService;

        /// <summary>
        /// Navigate command for the navbar
        /// </summary>
        public DelegateCommand<string> NavigateCommand { get; set; }

        public NavBarViewModel(IRegionManager manager, IEventAggregator eventAggregator, ISelectedService selectedService)
        {
            _regionManager = manager;
            _eventAggregator = eventAggregator;
            _selectedService = selectedService;
            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(SetToDbView);

            _eventAggregator.GetEvent<NavigateRequestEvent>().Subscribe(Navigate);

            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void SetToDbView(string systemName)
        {
            _systemName = systemName;

            if (systemName.ToLower().Contains("main menu"))
            {
                IsNotMainMenu = false;
                //_regionManager.Regions.Remove(RegionNames.FilesRegion);
                _regionManager.RequestNavigate("ContentRegion", "SearchView");

                RemoveAllFilesRegionViews();
            }
            else
            {
                IsNotMainMenu = true;

                _regionManager.RequestNavigate("FilesRegion", "DatabaseOptionsView");

                if (!_regionManager.Regions.ContainsRegionWithName("FilesRegion"))
                {
                    _regionManager.Regions.Add(RegionNames.FilesRegion, _regionManager.Regions["FilesRegion"]);
                }

                _regionManager.RequestNavigate("ContentRegion", "DatabaseDetailsView");
                
            }
        }

        private void Navigate(string uri = "")
        {
            CurrentView = "Views: ";

            switch (uri)
            {
                case "DatabaseDetailsView":
                    if (_selectedService.CurrentSystem.ToLower().Contains("main menu"))
                    {
                        CurrentView += "Search view";
                        _regionManager.RequestNavigate("ContentRegion", "SearchView");
                    }
                    else
                    {
                        CurrentView += "Database editor";
                        _regionManager.RequestNavigate("FilesRegion", "DatabaseOptionsView");
                        _regionManager.RequestNavigate("ContentRegion", "DatabaseDetailsView");
                    }
                    break;
                case "HsMediaAuditView":
                    CurrentView += "Hyperspin media audit";
                    _regionManager.RequestNavigate("FilesRegion", "HyperspinFilesView");
                    break;
                case "RlMediaAuditView":
                    CurrentView += "RocketLaunch media audit";
                    _regionManager.RequestNavigate("FilesRegion", "FilesView");
                    break;
                case "IntroVideosView":
                    CurrentView += "Hyperspin video intros";
                    RemoveAllFilesRegionViews();
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
                case "WebBrowseView":
                    CurrentView += "Web Browser";
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

        private void RemoveAllFilesRegionViews()
        {            
            foreach (var view in _regionManager.Regions["FilesRegion"].Views)
            {
                _regionManager.Regions["FilesRegion"].Deactivate(view);
            }
        }


    }
}
