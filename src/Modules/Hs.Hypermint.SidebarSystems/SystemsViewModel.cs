using Hs.HyperSpin.Database;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Prism.Events;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using Hs.Hypermint.Settings;

namespace Hs.Hypermint.SidebarSystems
{
    public class SystemsViewModel
    {
        private ICollectionView _systemItems;
        public ICollectionView SystemItems { get; private set; }

        IMainMenuRepo _mainMenuRepo;
        ISettingsRepo _settingsRepo;

        private IEventAggregator _eventAggregator;
        
        public SystemsViewModel(ISettingsRepo settings, IMainMenuRepo main, IEventAggregator eventAggregator)
        {
            _settingsRepo = settings;
            _mainMenuRepo = main;
            _eventAggregator = eventAggregator;

            var mainMenuXml = _settingsRepo.HypermintSettings.HsPath  + @"\Databases\Main Menu\Main Menu.xml";

            if (File.Exists(mainMenuXml))
            {
                SystemItems = new ListCollectionView(
            _mainMenuRepo.BuildMainMenuItems(mainMenuXml,
            _settingsRepo.HypermintSettings.RlMediaPath + @"\Icons\"));

            SystemItems.CurrentChanged += SystemItems_CurrentChanged;
            }
            else
                SystemItems = null;
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
