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

        public List<Game> SelectedGames { get; set; }

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
        #endregion

        #region Commands & Event
        private readonly IEventAggregator _eventAggregator;                        
        public DelegateCommand AuditScanStart { get; private set; }
        public DelegateCommand<IList> SelectionChanged { get; set; }
        public DelegateCommand<string> EnableDbItemsCommand { get; set; }
        public DelegateCommand<string> OpenFolderCommand { get; set; }
        public DelegateCommand<string> SaveXmlCommand { get; set; }
        public DelegateCommand<string> EnableFaveItemsCommand { get; set; }
        #endregion

        #region Constructors
        public DatabaseDetailsViewModel(ISettingsRepo settings, IGameRepo gameRepo, 
            IHyperspinXmlService xmlService, ISelectedService selectedService, 
            IFavoriteService favoriteService, IGenreRepo genreRepo,
            IEventAggregator eventAggregator, IFolderExplore folderService)
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

            SetUpGamesListFromMainMenuDb();
            SelectedGames = new List<Game>();

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
            SaveXmlCommand = new DelegateCommand<string>(SaveXml);
            EnableFaveItemsCommand = new DelegateCommand<string>(EnableFaveItems);

            // Command for datagrid selectedItems
            SelectionChanged = new DelegateCommand<IList>(
                items =>
                {
                    if (items == null)
                    {
                        SelectedItemsCount = 0;
                        SelectedGames.Clear();
                        return;
                    }

                    try
                    {
                        SelectedGames.Clear();
                        foreach (var item in items)
                        {
                            SelectedGames.Add(item as Game);
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

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(UpdateGames);
            _eventAggregator.GetEvent<GameFilteredEvent>().Subscribe(FilterGamesByText);
            _eventAggregator.GetEvent<CloneFilterEvent>().Subscribe(FilterRomClones);

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
        #endregion

        #region Filter Methods
        /// <summary>
        /// Filter the current GamesList with textbox from filter controls
        /// </summary>
        /// <param name="obj"></param>
        private void FilterGamesByText(Dictionary<string, bool> options)
        {
            if (GamesList != null)
            {
                ICollectionView cv;

                cv = CollectionViewSource.GetDefaultView(GamesList);

                var filter = "";
                var showClones = false;

                foreach (var item in options)
                {
                    filter = item.Key;
                    showClones = item.Value;
                }

                cv.Filter = o =>
                {
                    var g = o as Game;
                    var textFiltered = false;
                    var flag = false;

                    if (showClones)
                    {
                        textFiltered = g.Description.ToUpper().Contains(filter.ToUpper());
                                
                    }
                    else
                        textFiltered = g.Description.ToUpper().Contains(filter.ToUpper())
                        && g.CloneOf.Equals(string.Empty);

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
                        if (systemName.Contains("Main Menu"))
                            _gameRepo.GetGames(hsPath + @"\Databases\Main Menu\" + systemName + ".xml", systemName);
                        else
                        {
                            _gameRepo.GetGames(hsPath + @"\Databases\" + systemName + "\\" + systemName + ".xml", systemName);

                            //Populate genres
                            var genrePath = Path.Combine(hsPath, Root.Databases, systemName, "Genre.xml");
                            if (File.Exists(genrePath))
                            {
                                 
                                _genreRepo.PopulateGenres(genrePath);
                            }
                            else { _genreRepo.GenreList.Clear(); }
                        }
                       
                        GamesList = new ListCollectionView(_gameRepo.GamesList);
                        
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
                        //Publish after the gameslist is updated here
                        _eventAggregator.GetEvent<GamesUpdatedEvent>().Publish(systemName);

                        updateFavoritesForGamesList();

                        updateSystemDatabases();
                        
                    }

                    GamesList.CurrentChanged += GamesList_CurrentChanged;
                    
                }
            }

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

            foreach (var item in Directory.GetFiles(pathToScan, "*.xml"))
            {
                xmlsInDirectory.Add(item);
            }

            SystemDatabases = new ListCollectionView(xmlsInDirectory);         
        }

        private void updateGenres()
        {            
            if (_genreRepo.GenreList.Count != 0 && _genreRepo.GenreList != null)
            {                
                _genreRepo.PopulateGenres(_selectedService.CurrentSystem);
                GenreDatabases = new ListCollectionView(_genreRepo.GenreList);
                
            }
        
        }

        private void EnableDbItems(string enabled)
        {
            var enableItems = Convert.ToInt32(enabled);
            if (SelectedGames != null && SelectedGames.Count > 0)
            {
                try
                {
                    foreach (var game in SelectedGames)
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

            if (SelectedGames != null && SelectedGames.Count > 0)
            {
                try
                {
                    foreach (var game in SelectedGames)
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
                    _settingsRepo.HypermintSettings.HsPath, dbName);
                }
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(e.TargetSite + " : " + e.Message);
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
                    _eventAggregator.GetEvent<GameSelectedEvent>().Publish(game.RomName);
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
