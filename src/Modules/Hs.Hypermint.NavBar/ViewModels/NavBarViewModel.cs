using System;
using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

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

        private void SetToDbView(string obj)
        {
            _regionManager.RequestNavigate("ContentRegion", "DatabaseDetailsView");
        }

        private void Navigate(string uri = "")
        {
            CurrentView = "Views: ";
                            
            switch (uri)
            {
                case "DatabaseDetailsView":
                    CurrentView += "Database editor";
                    break;
                case "HsMediaAuditView":
                    CurrentView += "Hyperspin media audit";
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
            _regionManager.RequestNavigate("ContentRegion",uri);
        }
    }
}
