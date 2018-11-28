using Hypermint.Base.Interfaces;
using Prism.Events;
using Hypermint.Base.Services;
using Hypermint.Base;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        #region Service Repos

        //IMainMenuRepo _mainMenuRepo;
        ISettingsHypermint _settingsRepo;
        IEventAggregator _eventAggregator;
        ISelectedService _selectedService;
        IHyperspinManager _hyperManager;

        #endregion

        #region Constructor

        public SidebarViewModel(IEventAggregator eventAggregator, 
            ISettingsHypermint settings, ISelectedService selectedService, 
            IHyperspinManager hyperManager)
        {
            _settingsRepo = settings;
            _eventAggregator = eventAggregator;
            _selectedService = selectedService;
            _hyperManager = hyperManager;
        }

        #endregion

        #region Properties

        public string SelectedMainMenu { get; set; }

        private string _systemWheelImage;
        public string SystemWheelImage
        {
            get { return _systemWheelImage; }
            set { SetProperty(ref _systemWheelImage, value); }
        }

        private string mainMenuDataBaseCount = "Main Menus: ";
        public string MainMenuDataBaseCount
        {
            get { return mainMenuDataBaseCount; }
            set { SetProperty(ref mainMenuDataBaseCount, value); }
        }

        #endregion

        #region Public Methods

        public async void Load()
        {
           await _hyperManager.PopulateMainMenuSystems();
        }

        #endregion
        
    }
}
