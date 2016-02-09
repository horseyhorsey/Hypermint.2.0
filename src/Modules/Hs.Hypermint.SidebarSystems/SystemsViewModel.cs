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

namespace Hs.Hypermint.SidebarSystems
{
    public class SystemsViewModel : ViewModelBase
    {
        private ICollectionView _systemItems;
        public ICollectionView SystemItems { 
            get { return _systemItems; }
            set { SetProperty(ref _systemItems, value); }
        }

        IMainMenuRepo _mainMenuRepo;
        ISettingsRepo _settingsRepo;

        private IEventAggregator _eventAggregator;
        
        public SystemsViewModel(IMainMenuRepo main, IEventAggregator eventAggregator, ISettingsRepo settings)
        {
            _mainMenuRepo = main;
            _eventAggregator = eventAggregator;

            _settingsRepo = settings;
            _settingsRepo.LoadHypermintSettings();

            // Setup the main menu database to read in all systems

            var mainMenuXml  = "";
            try
            {
                mainMenuXml = Path.Combine(
                    _settingsRepo.HypermintSettings.HsPath,Root.Databases,
                    @"Main Menu\Main Menu.xml");
            }
            catch (System.Exception)
            {

            }

            if (File.Exists(mainMenuXml))
            {
                _mainMenuRepo.BuildMainMenuItems(mainMenuXml, _settingsRepo.HypermintSettings.RlMediaPath + @"\Icons\");
                SystemItems = new ListCollectionView(_mainMenuRepo.Systems);
                
            }
            else
                SystemItems = null;

            _eventAggregator.GetEvent<MainMenuSelectedEvent>().Subscribe(UpdateSystems);
            SystemItems.CurrentChanged += SystemItems_CurrentChanged;

        }

        private void UpdateSystems(string mainMenuXml)
        {            
            _mainMenuRepo.BuildMainMenuItems(mainMenuXml, _settingsRepo.HypermintSettings.RlMediaPath + @"\Icons\");
            SystemItems = new ListCollectionView(_mainMenuRepo.Systems);

            //Subscribe here again?? 
            //##    Existing unsubscribes when the system list is changed.
            SystemItems.CurrentChanged += SystemItems_CurrentChanged;

        }

        private void SystemItems_CurrentChanged(object sender, System.EventArgs e)
        {
            MainMenu system = SystemItems.CurrentItem as MainMenu;
            
            if (system != null)
            {
                this._eventAggregator.GetEvent<SystemSelectedEvent>().Publish(system.Name);
            }
        }
    }
}
