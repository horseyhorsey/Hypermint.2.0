using Hypermint.Base.Base;
using System.Collections.ObjectModel;
using Hypermint.Base.Interfaces;
using System.IO;
using Hypermint.Base.Constants;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Generic;
using Prism.Events;
using Hypermint.Base;

namespace Hs.Hypermint.SidebarSystems
{
    public class SidebarSystemsViewModel : ViewModelBase
    {
        #region Properties
        private string _systemWheelImage;
        public string SystemWheelImage
        {
            get { return _systemWheelImage; }
            set { SetProperty(ref _systemWheelImage, value); }
        }

        private string _systemTitleCount;
        public string SystemTitleCount
        {
            get { return "Systems: "; }
            set { SetProperty(ref _systemTitleCount, value); }
        }
        
        public ICollectionView MainMenuDatabases { get; private set; }

        private IEventAggregator _eventAggregator;

        public string SelectedMainMenu { get; set; }
        #endregion

        #region Service Repos
            IMainMenuRepo _mainMenuRepo;
            ISettingsRepo _settingsRepo;
        #endregion

        public SidebarSystemsViewModel(IEventAggregator eventAggregator, IMainMenuRepo main, ISettingsRepo settings)
        {
            _mainMenuRepo = main;            
            _settingsRepo = settings;
            _settingsRepo.LoadHypermintSettings();
            _eventAggregator = eventAggregator; 

            //SelectedMainMenu
            var mainMenuDatabasePath = Path.Combine(    
                _settingsRepo.HypermintSettings.HsPath,
                Root.Databases, @"Main Menu");

            var databases = new List<string>();

            if (Directory.Exists(mainMenuDatabasePath))
            {
                foreach (var item in _mainMenuRepo.GetMainMenuDatabases(mainMenuDatabasePath))
                {
                    databases.Add(item);
                }

                if (databases.Count != 0)
                {
                    SelectedMainMenu = "Main Menu";

                    MainMenuDatabases = new ListCollectionView(databases);

                    MainMenuDatabases.CurrentChanged += MainMenuDatabases_CurrentChanged;
                }
            }

        }

        private void MainMenuDatabases_CurrentChanged(object sender, System.EventArgs e)
        {
            var mainMenuXml = Path.Combine(
                _settingsRepo.HypermintSettings.HsPath,
                Root.Databases,
                @"Main Menu\", SelectedMainMenu + ".xml");

               _mainMenuRepo.BuildMainMenuItems(mainMenuXml);

               _eventAggregator.GetEvent<MainMenuSelectedEvent>().Publish(mainMenuXml);

        }
    }
}
