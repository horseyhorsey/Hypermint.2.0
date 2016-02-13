using Hs.HyperSpin.Database;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Prism.Events;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using Hypermint.Base.Constants;
using Hypermint.Base.Base;
using System.Diagnostics;
using Hypermint.Base.Services;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class SystemsViewModel : ViewModelBase
    {
        private ICollectionView _systemItems;
        public ICollectionView SystemItems { 
            get { return _systemItems; }
            set { SetProperty(ref _systemItems, value); }
        }

        private bool systemListEnabled = true;
        public bool SystemListEnabled
        {
            get { return systemListEnabled; }
            set { SetProperty(ref systemListEnabled, value); }
        }

        IMainMenuRepo _mainMenuRepo;
        ISettingsRepo _settingsRepo;

        IEventAggregator _eventAggregator;
        ISelectedService _selectedService;

        public SystemsViewModel()
        {

        }

        public SystemsViewModel(IMainMenuRepo main, IEventAggregator eventAggregator,
            ISettingsRepo settings, ISelectedService selectedService)
        {
            _mainMenuRepo = main;
            _eventAggregator = eventAggregator;

            _settingsRepo = settings;
            _settingsRepo.LoadHypermintSettings();

            _selectedService = selectedService;

            // Setup the main menu database to read in all systems

            var mainMenuXml  = "";

            mainMenuXml = Path.Combine(
                    _settingsRepo.HypermintSettings.HsPath,Root.Databases,
                    @"Main Menu\Main Menu.xml");

            if (File.Exists(mainMenuXml))
            {
                _mainMenuRepo.BuildMainMenuItems(mainMenuXml, _settingsRepo.HypermintSettings.RlMediaPath + @"\Icons\");
                SystemItems = new ListCollectionView(_mainMenuRepo.Systems);
                SystemItems.CurrentChanged += SystemItems_CurrentChanged;
            }
            else
                SystemItems = null;

            _eventAggregator.GetEvent<MainMenuSelectedEvent>().Subscribe(UpdateSystems);
            _eventAggregator.GetEvent<SystemFilteredEvent>().Subscribe(FilterSystemsByText);


        }

        private void UpdateSystems(string mainMenuXml)
        {
            if (File.Exists(mainMenuXml))
            {
                _mainMenuRepo.BuildMainMenuItems(mainMenuXml, _settingsRepo.HypermintSettings.RlMediaPath + @"\Icons\");
                SystemItems = new ListCollectionView(_mainMenuRepo.Systems);

                //Subscribe here again?? 
                //##    Existing unsubscribes when the system list is changed.
                SystemItems.CurrentChanged += SystemItems_CurrentChanged;
            }
        }

        /// <summary>
        /// Filter the systems list
        /// </summary>
        /// <param name="obj"></param>
        private void FilterSystemsByText(string filter)
        {
            //Unsubscribe when the filter is being set
            //Avoiding the systems databases loading on each filter change            
            SystemItems.CurrentChanged -= SystemItems_CurrentChanged;
           
            if (SystemItems != null)
            {
                ICollectionView cv;

                cv = CollectionViewSource.GetDefaultView(SystemItems);

                cv.Filter = o =>
                {
                    var m = o as MainMenu;

                    var textFiltered = m.Name.ToUpper().Contains(filter.ToUpper());
                    return textFiltered;
                };

            }
            
            SystemItems.CurrentChanged += SystemItems_CurrentChanged;
            
        }

        private void SystemItems_CurrentChanged(object sender, System.EventArgs e)
        {
            
            MainMenu system = SystemItems.CurrentItem as MainMenu;
            
            if (system != null)
            {
                _selectedService.CurrentSystem = system.Name;
                this._eventAggregator.GetEvent<SystemSelectedEvent>().Publish(system.Name);
                
            }
        }
    }
}
