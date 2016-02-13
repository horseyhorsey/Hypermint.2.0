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
using System;
using System.Runtime.CompilerServices;

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

        private string mainMenuDataBaseCount = "Main Menus: ";
        public string MainMenuDataBaseCount
        {
            get { return mainMenuDataBaseCount; }
            set { SetProperty(ref mainMenuDataBaseCount, value); }
        }

        private string systemTextFilter;
        public string SystemTextFilter
        {
            get { return systemTextFilter; }
            set
            {
                SetProperty(ref systemTextFilter, value);
                OnPropertyChanged(() => SystemTextFilter);
            }
        }

        public string SelectedMainMenu { get; set; }

        public ICollectionView MainMenuDatabases { get; private set; }
              
        #endregion

        #region Service Repos
        IMainMenuRepo _mainMenuRepo;
        ISettingsRepo _settingsRepo;
        #endregion

        private IEventAggregator _eventAggregator;

        protected override void OnPropertyChanged(string propertyName)
        {
            //base.OnPropertyChanged(propertyName);
            if (propertyName == "SystemTextFilter")
            {
                //Publish to SystemsViewModel with SystemFilteredEvent
                _eventAggregator.GetEvent<SystemFilteredEvent>().Publish(SystemTextFilter);
            }
        }

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
                    MainMenuDataBaseCount = "Main Menus: " + databases.Count;
                    MainMenuDatabases = new ListCollectionView(databases);

                    MainMenuDatabases.CurrentChanged += MainMenuDatabases_CurrentChanged;
                }
            }

        }

        /// <summary>
        /// Combo box event of main menu xmls changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
