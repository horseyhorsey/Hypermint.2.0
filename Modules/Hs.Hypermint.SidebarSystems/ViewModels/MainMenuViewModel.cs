using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Events;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using Prism.Commands;
using System.Windows.Input;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        #region Fields
        private ISettingsHypermint _settingsRepo;
        private ISelectedService _selectedService;
        private IEventAggregator _eventAggregator;
        private IHyperspinXmlDataProvider _dataProvider;
        private IHyperspinManager _hyperspinManager;
        #endregion

        #region Constructors

        public MainMenuViewModel(ISettingsHypermint settingsRepo, IEventAggregator ea,
            IHyperspinManager hyperspinManager,
            ISelectedService selectedService,
            IHyperspinXmlDataProvider dataProvider, IFolderExplore folderService)
        {
            _settingsRepo = settingsRepo;
            _selectedService = selectedService;
            _eventAggregator = ea;
            _dataProvider = dataProvider;
            _hyperspinManager = hyperspinManager;

            if (_settingsRepo.HypermintSettings.HsPath == null)
                _settingsRepo.LoadHypermintSettings();

            //Init the collections used
            MainMenuItemViewModels = new ObservableCollection<MainMenuItemViewModel>();
            SelectedMainMenuItem = new MainMenuItemViewModel();
            MainMenuDatabases = new ListCollectionView(MainMenuItemViewModels);
            MainMenuDatabases.CurrentChanged += MainMenuDatabases_CurrentChanged;

            OpenFolderCommand = new DelegateCommand<string>(x => folderService.OpenFolder(Path.Combine(_settingsRepo.HypermintSettings.HsPath, "Databases", "Main Menu")));

            SetMainMenuDatabases("Main Menu");
        }

        public MainMenuViewModel()
        {

        }

        #endregion

        public ICommand OpenFolderCommand { get; set; }

        #region Properties

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
        /// Populates all the main menu databases. Used on initializing
        /// </summary>
        /// <param name="mainMenuDbPath">The main menu database path.</param>
        private async void SetMainMenuDatabases(string mainMenuDbPath)
        {
            MainMenuItemViewModels.Clear();

            _hyperspinManager._hyperspinFrontEnd.Path = _settingsRepo.HypermintSettings.HsPath;
            if (_selectedService.CurrentSystem == null)
                await _hyperspinManager.GetSystemDatabases("Main Menu");
            
            //Create view models for each database file
            foreach (var dbFile in _hyperspinManager.DatabasesCurrentSystem)
            {
                MainMenuItemViewModels.Add(new MainMenuItemViewModel
                {
                    Name = dbFile.FileName,
                    Path = dbFile.FullPath
                });
            }

            //Move Main Menu to the first index
            var db = MainMenuItemViewModels.FirstOrDefault(x => x.Name == "Main Menu");
            MainMenuItemViewModels.Remove(db);
            MainMenuItemViewModels.Insert(0, db);            

            if (MainMenuItemViewModels.Count != 0)
            {
                MenusHeader = $"Main Menu Files: " + MainMenuItemViewModels.Count;
            }

            _selectedService.CurrentMainMenu = "Main Menu";

            try
            {
                MainMenuDatabases.MoveCurrentToFirst();
            }
            catch (Exception) {}
        }

        /// <summary>
        /// Combo box event of main menu xmls changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuDatabases_CurrentChanged(object sender, System.EventArgs e)
        {
            SelectedMainMenuItem = (MainMenuItemViewModel)MainMenuDatabases.CurrentItem;

            if (_selectedService.CurrentMainMenu == null) return;

            _selectedService.CurrentMainMenu = SelectedMainMenuItem.Name;            

            _eventAggregator.GetEvent<MainMenuSelectedEvent>().Publish(_selectedService.CurrentMainMenu);
        }

        #endregion

    }
}
