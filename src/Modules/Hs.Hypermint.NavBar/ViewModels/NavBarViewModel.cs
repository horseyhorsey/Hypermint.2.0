using Hypermint.Base.Base;
using Prism.Commands;
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

        /// <summary>
        /// Navigate command for the navbar
        /// </summary>
        public DelegateCommand<string> NavigateCommand { get; set; }

        public NavBarViewModel(IRegionManager manager)
        {
            _regionManager = manager;

            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string uri)
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
