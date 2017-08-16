using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        #region Fields
        private IMainMenuRepo _mainMenuRepo;
        private IFileFolderChecker _fileCheckService;
        private ISettingsRepo _settingsRepo;
        private ISelectedService _selectedService;
        private string mainMenuDatabasePath;
        private IEventAggregator _eventAggregator;
        #endregion

        #region Constructors

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

            //Init the items from main menu databases
            MainMenuItemViewModels = new ObservableCollection<MainMenuItemViewModel>();
            SelectedMainMenuItem = new MainMenuItemViewModel();            

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
        #endregion


        #region Properties

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

        private ICollectionView mainMenuDatabases;
        public ICollectionView MainMenuDatabases
        {
            get { return mainMenuDatabases; }
            set { SetProperty(ref mainMenuDatabases, value); }
        }

        private ObservableCollection<MainMenuItemViewModel> _mainMenuItemViewModels;
        public ObservableCollection<MainMenuItemViewModel> MainMenuItemViewModels
        {
            get { return _mainMenuItemViewModels; }
            set { SetProperty(ref _mainMenuItemViewModels, value); }
        }

        private MainMenuItemViewModel selectedMainMenuItem;
        public MainMenuItemViewModel SelectedMainMenuItem
        {
            get { return selectedMainMenuItem; }
            set { SetProperty(ref selectedMainMenuItem, value); }
        }

        #endregion

        #region Support Methods
        /// <summary>
        /// Sets the main menu collection of main menu databases.
        /// </summary>
        /// <param name="mainMenuDbPath">The main menu database path.</param>
        private void SetMainMenuDatabases(string mainMenuDbPath)
        {
            if (_fileCheckService.DirectoryExists(mainMenuDatabasePath))
            {
                foreach (string item in _mainMenuRepo.GetMainMenuDatabases(mainMenuDatabasePath))
                {
                    MainMenuItemViewModels.Add(new MainMenuItemViewModel
                    {
                        Name = item
                    });
                }

                if (MainMenuItemViewModels.Count != 0)
                {

                    //Dont need to set count here.
                    MenuCount = MainMenuItemViewModels.Count;
                    //or here?
                    MenusHeader = "Main Menus: " + MenuCount;

                    MainMenuDatabases = new ListCollectionView(MainMenuItemViewModels);
                    MainMenuDatabases.CurrentChanged += MainMenuDatabases_CurrentChanged;

                    try
                    {
                        MainMenuDatabases.MoveCurrentToFirst();

                    }
                    catch (Exception) { }

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
            SelectedMainMenuItem = (MainMenuItemViewModel)MainMenuDatabases.CurrentItem;

            _selectedService.CurrentMainMenu = SelectedMainMenuItem.Name;

            var mainMenuXml = SelectedMainMenuItem.Path;

            if (_fileCheckService.FileExists(mainMenuXml))
            {
                _eventAggregator.GetEvent<MainMenuSelectedEvent>().Publish(mainMenuXml);
            }
        }

        [Obsolete("Use static method GetMainMenuPath in frontend helpers")]
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

        #endregion

    }
}
