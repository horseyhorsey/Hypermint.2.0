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

        private int menuCount;
        public int MenuCount
        {
            get { return menuCount; }
            set { SetProperty(ref menuCount, value); }
        }

        private string menusHeader = "Menus";
        public string MenusHeader
        {
            get { return menusHeader; }
            set { SetProperty(ref menusHeader, value); }
        }

        private string mainMenuDatabasePath;
        private string selectedMainMenu;
        private IEventAggregator _eventAggregator;

        public MainMenuViewModel()
        {
                
        }

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

            if (string.IsNullOrEmpty(_settingsRepo.HypermintSettings.HsPath))            
                _settingsRepo.LoadHypermintSettings();               

            if (_fileCheckService.DirectoryExists(
                _settingsRepo.HypermintSettings.HsPath))
            {
               mainMenuDatabasePath = GetMainMenuPath(_settingsRepo.HypermintSettings.HsPath);
            }

            if (!string.IsNullOrEmpty(mainMenuDatabasePath))
                SetMainMenuDatabases(mainMenuDatabasePath);            
                
        }

        private void SetMainMenuDatabases(string mainMenuDbPath)
        {
            var databases = new List<string>();

            if (_fileCheckService.DirectoryExists(mainMenuDatabasePath))
            {                

                foreach (var item in _mainMenuRepo.GetMainMenuDatabases(mainMenuDatabasePath))
                {
                    databases.Add(item);
                }

                if (databases.Count != 0)
                {
                    MenuCount = databases.Count;
                    MenusHeader = "Main Menus: " + MenuCount;

                    MainMenuDatabases = new ListCollectionView(databases);
                    MainMenuDatabases.CurrentChanged += MainMenuDatabases_CurrentChanged;
                    MainMenuDatabases.CollectionChanged += MainMenuDatabases_CurrentChanged;
                    try
                    {
                        MainMenuDatabases.MoveCurrentTo("Main Menu");
                             
                    }
                    catch (Exception) { }
                    
                }
            }

        }

        private void MainMenuDatabases_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Folder location of hyperspin main menu
        /// </summary>
        /// <param name="hsMainMenuPath"></param>
        private string GetMainMenuPath(string hyperspinPath)
        {
            return _fileCheckService.CombinePath(new string[] {
                hyperspinPath, Root.Databases, @"Main Menu"
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

            if (_fileCheckService.FileExists(mainMenuXml))
            {                
                _eventAggregator.GetEvent<MainMenuSelectedEvent>().Publish(mainMenuXml);
            }


        }
    }
}
