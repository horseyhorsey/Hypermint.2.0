using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Hs.HyperSpin.Database;
using System.Windows.Input;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Runtime.CompilerServices;
using Prism.Events;
using Hypermint.Base;
using System.Collections.Generic;
using System.IO;
using Hs.Hypermint.DatabaseDetails.Services;
using Hypermint.Base.Services;
using System.Collections;
using Hypermint.Base.Constants;
using System.Xml;
using Hypermint.Base.Models;
using MahApps.Metro.Controls.Dialogs;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabaseDetailsViewModel : ViewModelBase
    {

        #region Properties
        private ICollectionView _gameList;
        public ICollectionView GamesList
        {
            get { return _gameList; }
            set { SetProperty(ref _gameList, value); }
        }                

        private ICollectionView systemDatabases;
        public ICollectionView SystemDatabases
        {
            get { return systemDatabases; }
            set { SetProperty(ref systemDatabases, value); }
        }

        private ICollectionView genreDatabases;
        public ICollectionView GenreDatabases
        {
            get { return genreDatabases; }
            set { SetProperty(ref genreDatabases, value); }
        }

        private bool saveFavoritesXml;
        public bool SaveFavoritesXml
        {
            get { return saveFavoritesXml; }
            set { SetProperty(ref saveFavoritesXml, value); }
        }

        private bool addToGenre;
        public bool AddToGenre
        {
            get { return addToGenre; }
            set { SetProperty(ref addToGenre, value); }
        }

        public int SelectedItemsCount { get; private set; }

        private string databaseHeaderInfo = "Database Editor";
        public string DatabaseHeaderInfo
        {
            get { return databaseHeaderInfo; }
            set { SetProperty(ref databaseHeaderInfo, value); }
        }

        private string dbName;
        public string DbName
        {
            get { return dbName; }
            set { SetProperty(ref dbName, value); }
        }
        #endregion

        #region Commands & Event
        private readonly IEventAggregator _eventAggregator;                        
        public DelegateCommand AuditScanStart { get; private set; }
        public DelegateCommand<IList> SelectionChanged { get; set; }
        public DelegateCommand<string> EnableDbItemsCommand { get; set; }
        public DelegateCommand<string> OpenFolderCommand { get; set; }
        public DelegateCommand<string> SaveXmlCommand { get; set; }
        public DelegateCommand SaveGenresCommand { get; private set; } 
        public DelegateCommand<string> EnableFaveItemsCommand { get; set; }
        public DelegateCommand AddMultiSystemCommand { get; private set; }
        public DelegateCommand LaunchGameCommand { get; private set; }
        #endregion

        #region Constructors
        public DatabaseDetailsViewModel(ISettingsRepo settings, IGameRepo gameRepo, 
            IHyperspinXmlService xmlService, ISelectedService selectedService, 
            IFavoriteService favoriteService, IGenreRepo genreRepo,
            IEventAggregator eventAggregator, IFolderExplore folderService,
            IGameLaunch gameLaunch,
            IMainMenuRepo memuRepo, IDialogCoordinator dialogService
            )
        {
            if (gameRepo == null) throw new ArgumentNullException("gameRepo");
            _settingsRepo = settings;
            _gameRepo = gameRepo;
            _eventAggregator = eventAggregator;
            _favouriteService = favoriteService;
            _selectedService = selectedService;
            _folderExploreService = folderService;
            _xmlService = xmlService;
            _genreRepo = genreRepo;
            _gameLaunch = gameLaunch;
            _menuRepo = memuRepo;
            _dialogService = dialogService;


            _selectedService.CurrentSystem = "Main Menu";

            SetUpGamesListFromMainMenuDb();
            _selectedService.SelectedGames = new List<Game>();

            if (_gameRepo.GamesList != null)
            {
                GamesList = new ListCollectionView(_gameRepo.GamesList);
                GamesList.CurrentChanged += GamesList_CurrentChanged;
            }

            if (_genreRepo.GenreList != null)
            {
                GenreDatabases = new ListCollectionView(_genreRepo.GenreList);                
            }

            EnableDbItemsCommand = new DelegateCommand<string>(EnableDbItems);
            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);
            
            SaveGenresCommand = new DelegateCommand(SaveGenres);
            EnableFaveItemsCommand = new DelegateCommand<string>(EnableFaveItems);
            AddMultiSystemCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<AddToMultiSystemEvent>().Publish(_selectedService.SelectedGames);
            });

            LaunchGameCommand = new DelegateCommand(LaunchGame);

            // Command for datagrid selectedItems
            SelectionChanged = new DelegateCommand<IList>(
                items =>
                {
                    if (items == null)
                    {
                        SelectedItemsCount = 0;
                        _selectedService.SelectedGames.Clear();                        
                        return;
                    }
                    else
                    {
                        SelectedItemsCount = items.Count;
                    }

                    try
                    {
                        _selectedService.SelectedGames.Clear();
                        foreach (var item in items)
                        {
                            var game = item as Game;
                            if (game.RomName != null)
                                _selectedService.SelectedGames.Add(item as Game);
                        }

                        if (SelectedItemsCount > 1)
                            DatabaseHeaderInfo = "Selected items: " + SelectedItemsCount;
                        else if (SelectedItemsCount == 1)
                        {
                            var game = items[0] as Game;
                            DatabaseHeaderInfo = "Selected item: " + game.RomName;
                        }
                        else
                            DatabaseHeaderInfo = "";
                    }
                    catch (Exception)
                    {


                    }

                });

            SaveXmlCommand = new DelegateCommand<string>(async x =>
            {
                var mahSettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Save",
                    NegativeButtonText = "Cancel"
                };

                var result = await _dialogService.ShowMessageAsync(this, "Save database", "Do you want to save? " + dbName,
                    MessageDialogStyle.AffirmativeAndNegative, mahSettings);

                if (result == MessageDialogResult.Affirmative)
                    SaveXml(x);

            });

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(UpdateGames);
            _eventAggregator.GetEvent<GameFilteredEvent>().Subscribe(FilterGamesByText);
            _eventAggregator.GetEvent<CloneFilterEvent>().Subscribe(FilterRomClones);
            _eventAggregator.GetEvent<MultipleCellsUpdated>().Subscribe((x) =>
            {
                GamesList.Refresh();
            });

            _eventAggregator.GetEvent<SaveMainMenuEvent>().Subscribe(SaveMainMenu);

        }

        private void SaveMainMenu(string xml)
        {
            _xmlService.SerializeMainMenuXml(_menuRepo.Systems, _settingsRepo.HypermintSettings.HsPath, xml);
        }

        #endregion

        #region Services
        private IGameRepo _gameRepo;
        private ISettingsRepo _settingsRepo;
        private IFavoriteService _favouriteService;
        private ISelectedService _selectedService;
        private IFolderExplore _folderExploreService;
        private IHyperspinXmlService _xmlService;
        private IGenreRepo _genreRepo;
        private IGameLaunch _gameLaunch;
        private IMainMenuRepo _menuRepo;
        private IDialogCoordinator _dialogService;
        #endregion

        #region Filter Methods
        /// <summary>
        /// Filter the current GamesList with textbox from filter controls
        /// </summary>
        /// <param name="obj"></param>
        private void FilterGamesByText(GameFilter gameFilter)
        {
            if (GamesList != null)
            {
                ICollectionView cv;

                cv = CollectionViewSource.GetDefaultView(GamesList);

                var filter = gameFilter.FilterText;
                var showClones = gameFilter.ShowClones;
                var favesOnly = gameFilter.ShowFavoritesOnly;
                var enabledFilter = gameFilter.ShowEnabledOnly;

                int enabled = 0;

                if (enabledFilter) { enabled = 1; }

                cv.Filter = o =>
                {
                    var g = o as Game;
                    var textFiltered = false;
                    var flag = false;

                    if (string.IsNullOrEmpty(filter))
                    {
                        if (favesOnly && showClones && enabledFilter)
                        {
                            textFiltered =
                            g.Enabled.Equals(enabled)
                            && g.IsFavorite.Equals(favesOnly)
                            && g.CloneOf.Length >= 0;
                        }
                        else if (favesOnly && showClones)
                        {
                            textFiltered =
                            g.IsFavorite.Equals(favesOnly)
                            && g.CloneOf.Length >= 0;
                        }
                        else if (favesOnly && !showClones && enabledFilter)
                        {
                            textFiltered =
                            g.Enabled.Equals(enabled)
                            && g.IsFavorite.Equals(favesOnly)
                            && g.CloneOf.Equals(string.Empty);
                        }
                        else if (favesOnly && !showClones)
                        {
                            textFiltered =
                            g.IsFavorite.Equals(favesOnly)
                            && g.CloneOf.Equals(string.Empty);
                        }
                        else if (!favesOnly && !showClones && enabledFilter)
                        {
                            textFiltered = g.CloneOf.Equals(string.Empty)
                              && g.Enabled.Equals(enabled);
                        }
                        else if (!favesOnly && showClones && enabledFilter)
                        {
                            textFiltered = g.CloneOf.Length >= 0
                            && g.Enabled.Equals(enabled);
                        }
                        else if (!favesOnly && !showClones) { textFiltered = g.CloneOf.Equals(string.Empty); }
                        else if (!favesOnly && showClones) { textFiltered = g.CloneOf.Length >= 0; }
                    }
                    else // Text is used as filter
                    {
                        if (showClones && favesOnly && enabledFilter)
                        {
                            textFiltered = g.Description.ToUpper().Contains(filter.ToUpper())
                            && g.Enabled.Equals(enabled)
                            && g.IsFavorite.Equals(favesOnly)
                            && g.CloneOf.Length >= 0;
                        }
                        else if (showClones && favesOnly)
                        {
                            textFiltered = g.Description.ToUpper().Contains(filter.ToUpper())
                            && g.IsFavorite.Equals(favesOnly)
                            && g.CloneOf.Length >= 0;
                        }
                        else if (showClones && !favesOnly && enabledFilter)
                        {
                            textFiltered = g.Description.ToUpper().Contains(filter.ToUpper())
                            && g.Enabled.Equals(enabled)
                            && g.CloneOf.Length >= 0;
                        }
                        else if (showClones && !favesOnly)
                        {
                            textFiltered = g.Description.ToUpper().Contains(filter.ToUpper())
                            && g.CloneOf.Length >= 0;
                        }
                        else if (!showClones && favesOnly && enabledFilter)
                        {
                            textFiltered = g.Description.ToUpper().Contains(filter.ToUpper())
                            && g.Enabled.Equals(enabled)
                            && g.IsFavorite.Equals(favesOnly) && g.CloneOf.Equals(string.Empty);
                        }
                        else if (!showClones && favesOnly)
                        {
                            textFiltered = g.Description.ToUpper().Contains(filter.ToUpper())
                            && g.IsFavorite.Equals(favesOnly) && g.CloneOf.Equals(string.Empty);
                        }
                        else if (!showClones && !favesOnly && enabledFilter)
                        {
                            textFiltered = g.Description.ToUpper().Contains(filter.ToUpper())
                            && g.Enabled.Equals(enabled)
                            && g.CloneOf.Equals(string.Empty);
                        }
                        else if (!showClones && !favesOnly)
                        {
                            textFiltered = g.Description.ToUpper().Contains(filter.ToUpper())
                            && g.CloneOf.Equals(string.Empty);
                        }
                    }

                    return textFiltered;
                };
           
            }
        }

        private void FilterRomClones(bool showClones)
        {
            if (GamesList != null)
            {                                
                var cv = CollectionViewSource.GetDefaultView(GamesList);
                if (showClones)
                    //cv.Filter = null;
                    ;
                else
                {
                    cv.Filter = o =>
                    {
                        var game = o as Game;

                        bool flag = false;
                        if (game.CloneOf != string.Empty)
                        {
                            flag = false;
                            return (flag);
                        }
                        else
                        {
                            flag = true;
                            return (flag);
                        }
                    };
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update games list event handler
        /// Published by selecting Systems in left system list
        /// </summary>
        /// <param name="systemName"></param>
        private void UpdateGames(string systemName)
        {
            _eventAggregator.GetEvent<ErrorMessageEvent>().Publish("Status: ");
            if (GamesList != null)
            {
                _gameRepo.GamesList.Clear();
                var hsPath = _settingsRepo.HypermintSettings.HsPath;
                if (Directory.Exists(hsPath))
                {
                    try
                    {
                        PopulateGamesList(systemName, hsPath,systemName);

                        //Populate genres
                        var genrePath = Path.Combine(hsPath, Root.Databases, systemName, "Genre.xml");
                        if (File.Exists(genrePath))
                        {

                            _genreRepo.PopulateGenres(genrePath);
                        }
                        else { _genreRepo.GenreList.Clear(); }                        

                        GenreDatabases = new ListCollectionView(_genreRepo.GenreList);
                    }
                    catch (System.Xml.XmlException exception)
                    {
                        exception.GetBaseException();
                        var msg = exception.Message;
                        _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(exception.SourceUri +  " : " + msg);
                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {                                          

                        updateSystemDatabases();

                        updateGenres();

                    }
                    
                }
            }

        }

        private void PopulateGamesList(string systemName, string hsPath, string dbName)
        {
            try
            {
                if (systemName.Contains("Main Menu"))
                    _gameRepo.GetGames(hsPath + @"\Databases\Main Menu\" + dbName + ".xml", systemName);
                else
                {
                    _gameRepo.GetGames(hsPath + @"\Databases\" + systemName + "\\" + dbName + ".xml", systemName);
                }

                GamesList = new ListCollectionView(_gameRepo.GamesList);

                GamesList.CurrentChanged += GamesList_CurrentChanged;

                GamesList.MoveCurrentToPrevious();

                GamesList.MoveCurrentToFirst();
            }
            catch (XmlException exception)
            {
                exception.GetBaseException();
                var msg = exception.Message;
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(exception.SourceUri + " : " + msg);

                _gameRepo.GamesList.Clear();
            }

            updateFavoritesForGamesList();

            //Publish after the gameslist is updated here
            _eventAggregator.GetEvent<GamesUpdatedEvent>().Publish(systemName);


        }

        private void updateFavoritesForGamesList()
        {
            if (_selectedService != null)
            {
                var selectedSystemName = _selectedService.CurrentSystem;

                //_gameRepo.GamesList
                var favesList = _favouriteService.GetFavoritesForSystem
                    (selectedSystemName, _settingsRepo.HypermintSettings.HsPath);

                foreach (var item in _gameRepo.GamesList)
                {
                    if (favesList.Contains(item.RomName))
                        item.IsFavorite = true;
                }
            }
            
        }

        private void updateSystemDatabases()
        {
            
            var pathToScan = _settingsRepo.HypermintSettings.HsPath +
                "\\" +
                Root.Databases + "\\" +
                _selectedService.CurrentSystem;

            if (!Directory.Exists(pathToScan)) return;

            var xmlsInDirectory = new List<string>();

            foreach (var xmlFile in Directory.GetFiles(pathToScan, "*.xml"))
            {
                var dbFileName = Path.GetFileNameWithoutExtension(xmlFile);

                if (dbFileName.ToLower() != "genre")
                {
                   xmlsInDirectory.Add(dbFileName);                    
                }
            }

            SystemDatabases = new ListCollectionView(xmlsInDirectory);            
        }

        private void updateGenres()
        {            
            if (_genreRepo.GenreList != null)
            {                                
                var updatedGameDbs = new List<string>();

                //Add the system database that was clicked
                updatedGameDbs.Add(_selectedService.CurrentSystem);

                foreach (var item in SystemDatabases)
                {
                    try
                    {
                        if (!GenreDatabases.Contains(item as string))
                        {
                            if ((string)item != _selectedService.CurrentSystem)
                                updatedGameDbs.Add(item as string);
                        }
                    }
                    catch (Exception)
                    {

                        
                    }
            
                }

                SystemDatabases = new ListCollectionView(updatedGameDbs);

                DbName = _selectedService.CurrentSystem;

                SystemDatabases.CurrentChanged += SystemDatabases_CurrentChanged;                           
            }
        
        }

        private void SystemDatabases_CurrentChanged(object sender, EventArgs e)
        {
            var hsPath = _settingsRepo.HypermintSettings.HsPath;

            PopulateGamesList(_selectedService.CurrentSystem, hsPath, (string)SystemDatabases.CurrentItem);

            DbName = (string)SystemDatabases.CurrentItem;
        }

        private void EnableDbItems(string enabled)
        {
            var enableItems = Convert.ToInt32(enabled);
            if (_selectedService.SelectedGames != null && _selectedService.SelectedGames.Count > 0)
            {
                try
                {
                    foreach (var game in _selectedService.SelectedGames)
                    {
                        var gameIndex = _gameRepo.GamesList.IndexOf(game);
                        _gameRepo.GamesList[gameIndex].Enabled = enableItems;                        
                    }
                                                               
                    GamesList.Refresh();
                }
                catch (Exception) { }
            }
        }

        private void EnableFaveItems(string enabled)
        {
            var enableItems = false;

            if (enabled == "0")
                enableItems = false;
            else
                enableItems = true;

            if (_selectedService.SelectedGames != null && _selectedService.SelectedGames.Count > 0)
            {
                try
                {
                    foreach (var game in _selectedService.SelectedGames)
                    {
                        var gameIndex = _gameRepo.GamesList.IndexOf(game);
                        _gameRepo.GamesList[gameIndex].IsFavorite = enableItems;
                    }

                    GamesList.Refresh();
                }
                catch (Exception) { }
            }
        }

        private void SetUpGamesListFromMainMenuDb()
        {
            if (Directory.Exists(_settingsRepo.HypermintSettings.HsPath))
            {
                try
                {
                    _gameRepo.GetGames(_settingsRepo.HypermintSettings.HsPath + @"\Databases\Main Menu\Main Menu.xml");
                }
                catch (Exception)
                {
                    //                
                }
            }
        }

        private void OpenFolder(string hyperspinDirType)
        {
            switch (hyperspinDirType)
            {
                case "Databases":
                    var pathToOpen = _settingsRepo.HypermintSettings.HsPath;
                    _folderExploreService.OpenFolder(pathToOpen + "\\" +
                        Root.Databases + "\\" +
                        _selectedService.CurrentSystem);
                        break;
                default:
                    break;
            }
                        
        }

        private void SaveXml(string dbName)
        {
            try
            {
                if (dbName == "Favorites")
                {
                    //Boxing lists, why? Must be better way.
                    var s = new List<Game>(_gameRepo.GamesList);
                    var filterFavorites = s.FindAll(m => m.IsFavorite == true);
                    var games = new Games();
                    foreach (var item in filterFavorites)
                    {
                        games.Add(item);
                    }

                    if (SaveFavoritesXml)
                    {
                        _xmlService.SerializeHyperspinXml(games, _selectedService.CurrentSystem,
                           _settingsRepo.HypermintSettings.HsPath, dbName);
                    }

                    //Save faves to text
                    try
                    {
                        var favesTextPath = Path.Combine(
                            _settingsRepo.HypermintSettings.HsPath,
                            Root.Databases, _selectedService.CurrentSystem,
                            "favorites.txt");

                        if (!File.Exists(favesTextPath))
                        {
                            File.CreateText(favesTextPath);
                        }

                        using (StreamWriter writer =
                            new StreamWriter(favesTextPath))
                        {
                            foreach (var favoriteGame in games)
                            {                                
                              writer.WriteLine(favoriteGame.RomName);
                            }

                            writer.Close();
                        }                        
                    }
                    catch (Exception)
                    {

                        
                    }
                    finally { }

                }
                else {
                    _xmlService.SerializeHyperspinXml(_gameRepo.GamesList, _selectedService.CurrentSystem,
                    _settingsRepo.HypermintSettings.HsPath, DbName);
                }
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(e.TargetSite + " : " + e.Message);
            }
            
        }

        private void SaveGenres()
        {

            _xmlService.SerializeGenreXml(_gameRepo.GamesList,
                _selectedService.CurrentSystem,
                _settingsRepo.HypermintSettings.HsPath);
        }

        private void LaunchGame()
        {
            if (_selectedService.SelectedGames[0] != null)
            {
                var rlPath = _settingsRepo.HypermintSettings.RlPath;
                var hsPath = _settingsRepo.HypermintSettings.HsPath;
                var sysName = _selectedService.SelectedGames[0].System;
                var romName = _selectedService.SelectedGames[0].RomName;

                _gameLaunch.RocketLaunchGame(rlPath, sysName, romName, hsPath);            
            }
        }

        #endregion

        #region Events
        private void GamesList_CurrentChanged(object sender, EventArgs e)
        {
            
            if (GamesList != null)
            {
                Game game = GamesList.CurrentItem as Game;
                if (game != null)
                {
                    _eventAggregator.GetEvent<GameSelectedEvent>().Publish(new string[] { game.RomName, "" });
                }
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {            
            base.OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
