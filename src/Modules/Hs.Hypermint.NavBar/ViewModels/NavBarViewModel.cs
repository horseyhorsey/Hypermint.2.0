using System;
using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Hypermint.Base.Constants;

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

        private string _systemName = "";

        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Navigate command for the navbar
        /// </summary>
        public DelegateCommand<string> NavigateCommand { get; set; }

        public NavBarViewModel(IRegionManager manager, IEventAggregator eventAggregator)
        {
            _regionManager = manager;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(SetToDbView);
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void SetToDbView(string systemName)
        {
            _systemName = systemName;

            if (systemName.Contains("Main Menu"))
                _regionManager.RequestNavigate("ContentRegion", "MainMenuView");
            else
                _regionManager.RequestNavigate("ContentRegion", "DatabaseDetailsView");
        }

        private void Navigate(string uri = "")
        {
            CurrentView = "Views: ";            

            switch (uri)
            {
                case "DatabaseDetailsView":
                    CurrentView += "Database editor";                    
                    _regionManager.RequestNavigate("FilesRegion", "DatabaseOptionsView");
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
                    break;
                case "MultiSystemView":
                    CurrentView += "Hyperspin multiple system generator";
                    break;
                case "SimpleWheelView":
                    CurrentView += "Simple wheel creator";
                    break;
                default:                    
                    break;
            }

            if (_systemName.Contains("Main Menu") && uri == "DatabaseDetailsView")
                _regionManager.RequestNavigate("ContentRegion", "MainMenuView");
            else
                _regionManager.RequestNavigate("ContentRegion", uri);

        }


    }
}
