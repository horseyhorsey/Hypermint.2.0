using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private IMainMenuRepo _mainMenuRepo;
        private IFileFolderChecker _fileCheckService;
        private ISettingsRepo _settingsRepo;
        private ISelectedService _selectedService;        

        private ICollectionView mainMenuDatabases;
        public ICollectionView MainMenuDatabases
        {
            get { return mainMenuDatabases; }
            set { SetProperty(ref mainMenuDatabases, value); }
        }

        private readonly string mainMenuDatabasePath;
        private string selectedMainMenu;
        private IEventAggregator _eventAggregator;

        public MainMenuViewModel(IMainMenuRepo mainMenuRepo,
            IFileFolderChecker fileCheckService,
            ISettingsRepo settingsRepo,
            ISelectedService selectedService,
            IEventAggregator ea)
        {
            _mainMenuRepo = mainMenuRepo;
            _fileCheckService = fileCheckService;
            _settingsRepo = settingsRepo;
            _selectedService = selectedService;
            _eventAggregator = ea;

            if (_settingsRepo.HypermintSettings.HsPath == null)
                _settingsRepo.LoadHypermintSettings();

            mainMenuDatabasePath = GetMainMenuPath();

            var databases = new List<string>();
            if (_fileCheckService.DirectoryExists(mainMenuDatabasePath))
            {
                foreach (var item in _mainMenuRepo.GetMainMenuDatabases(mainMenuDatabasePath))
                {
                    databases.Add(item);
                }

                if (databases.Count != 0)
                {
                    selectedMainMenu = "Main Menu";
                    _selectedService.CurrentMainMenu = selectedMainMenu;
                    
                    //MainMenuDataBaseCount = "Main Menus: " + databases.Count;
                    MainMenuDatabases = new ListCollectionView(databases);

                    MainMenuDatabases.CurrentChanged += MainMenuDatabases_CurrentChanged;
                }
            }
        }

        /// <summary>
        /// Folder location of hyperspin main menu
        /// </summary>
        /// <param name="hsMainMenuPath"></param>
        private string GetMainMenuPath()
        {
            return _fileCheckService.CombinePath(new string[] {
                _settingsRepo.HypermintSettings.HsPath,
                Root.Databases, @"Main Menu"
            });
        }

        /// <summary>
        /// Combo box event of main menu xmls changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuDatabases_CurrentChanged(object sender, System.EventArgs e)
        {
            selectedMainMenu = (string)MainMenuDatabases.CurrentItem;

            _selectedService.CurrentMainMenu = selectedMainMenu;

            var mainMenuXml = _fileCheckService.CombinePath( new string[] {
                mainMenuDatabasePath, selectedMainMenu  + ".xml" });

            if (_fileCheckService.CheckForFile(mainMenuXml))
            {
                _mainMenuRepo.BuildMainMenuItems(mainMenuXml);
                _eventAggregator.GetEvent<MainMenuSelectedEvent>().Publish(mainMenuXml);
            }


        }
    }
}
