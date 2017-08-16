using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Windows.Data;
using Prism.Events;
using Hypermint.Base;
using System.Collections.Generic;
using System.IO;
using Hs.Hypermint.DatabaseDetails.Services;
using Hypermint.Base.Services;
using System.Collections;
using System.Xml;
using Hypermint.Base.Models;
using MahApps.Metro.Controls.Dialogs;
using Hypermint.Base.Events;
using System.Threading.Tasks;
using System.Windows.Input;
using Frontends.Models.Hyperspin;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabaseDetailsViewModel : HyperMintModelBase
    {
        #region Constructors
        public DatabaseDetailsViewModel(ISettingsRepo settings, IGameRepo gameRepo,
            IHyperspinXmlService xmlService, ISelectedService selectedService,
            IFavoriteService favoriteService, IGenreRepo genreRepo,
            IEventAggregator eventAggregator, IGameLaunch gameLaunch,
            IMainMenuRepo memuRepo, IDialogCoordinator dialogService
            ):base(eventAggregator,selectedService,gameLaunch,settings)
        {
            if (gameRepo == null) throw new ArgumentNullException(nameof(gameRepo));
            
            _gameRepo = gameRepo;            
            _favouriteService = favoriteService;            
            _xmlService = xmlService;
            _genreRepo = genreRepo;
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

            ScanRomsCommand = new DelegateCommand(ScanRoms);

            EnableDbItemsCommand = new DelegateCommand<string>(EnableDbItems);

            EnableFaveItemsCommand = new DelegateCommand<string>(EnableFaveItems);

            // Command for datagrid selectedItems
            SelectionChanged = new DelegateCommand<IList>(
                items =>
                {
                    OnMultipleItemsSelectionChanged(items);

                });

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(UpdateGamesAsync);
            _eventAggregator.GetEvent<GameFilteredEvent>().Subscribe(FilterGamesByText);
            _eventAggregator.GetEvent<CloneFilterEvent>().Subscribe(FilterRomClones);
            _eventAggregator.GetEvent<MultipleCellsUpdated>().Subscribe((x) =>
            {
                if (GamesList != null)
                    GamesList.Refresh();
            });
            _eventAggregator.GetEvent<SystemDatabaseChanged>().Subscribe(SystemDatabaseChangedHandler);
            _eventAggregator.GetEvent<SaveMainMenuEvent>().Subscribe(SaveCurrentMainMenuItems);            

        }

        #endregion

        #region Fields
        private IGameRepo _gameRepo;        
        private IFavoriteService _favouriteService;        
        private IHyperspinXmlService _xmlService;
        private IGenreRepo _genreRepo;        
        private IMainMenuRepo _menuRepo;
        private IDialogCoordinator _dialogService;        
        #endregion

        #region Properties
        private ICollectionView _gameList;
        public ICollectionView GamesList
        {
            get { return _gameList; }
            set { SetProperty(ref _gameList, value); }
        }                

        public int SelectedItemsCount { get; private set; }

        private string databaseHeaderInfo = "Database Editor";
        public string DatabaseHeaderInfo
        {
            get { return databaseHeaderInfo; }
            set { SetProperty(ref databaseHeaderInfo, value); }
        }

        #endregion

        #region Commands        
        public ICommand AuditScanStart { get; private set; }
        public ICommand SelectionChanged { get; set; }
        public ICommand EnableDbItemsCommand { get; set; }        
        public ICommand EnableFaveItemsCommand { get; set; }        
        public ICommand ScanRomsCommand { get; private set; } 

        #endregion                

        #region Support Methods
        /// <summary>
        /// Update games list event handler
        /// Published by selecting Systems in left system list        
        /// </summary>
        /// <param name="systemName"></param>
        private async void UpdateGamesAsync(string dbName)
        {
            var hsPath = _settingsRepo.HypermintSettings.HsPath;
            var system = _selectedService.CurrentSystem;

            if (GamesList != null)
            {
                _gameRepo.GamesList.Clear();

                if (Directory.Exists(hsPath))
                {
                    try
                    {
                        await PopulateGamesList(system, hsPath, dbName);

                        //Publish after the gameslist is updated here
                        _eventAggregator.GetEvent<GamesUpdatedEvent>().Publish(dbName);
                    }
                    catch (XmlException exception)
                    {
                        exception.GetBaseException();
                        var msg = exception.Message;
                        _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(exception.SourceUri + " : " + msg);
                    }
                    catch (Exception ex) { }
                    finally
                    {

                    }

                }
            }

        }
        private async Task PopulateGamesList(string systemName, string hsPath, string dbName)
        {
            try
            {
                if (systemName.ToLower().Contains("main Menu"))
                    await _gameRepo.GetGamesAsync (hsPath + @"\Databases\Main Menu\" + dbName + ".xml", systemName);
                else
                {
                    await _gameRepo.GetGamesAsync(hsPath + @"\Databases\" + systemName + "\\" + dbName + ".xml", systemName);
                }

                GamesList = new ListCollectionView(_gameRepo.GamesList);

                GamesList.CurrentChanged += GamesList_CurrentChanged;

                GamesList.MoveCurrentToPrevious();

                try
                {
                    GamesList.MoveCurrentToFirst();
                }
                catch (Exception)
                {
                    
                }
                
            }
            catch (XmlException exception)
            {
                exception.GetBaseException();
                var msg = exception.Message;
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(exception.SourceUri + " : " + msg);

                _gameRepo.GamesList.Clear();
            }

           await updateFavoritesForGamesList();
            
        }
        private async Task updateFavoritesForGamesList()
        {
            if (_selectedService != null)
            {
                await Task.Run(() =>
                {
                    try
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
                    catch (Exception ex)
                    {

                        throw;
                    }
                });
                
            }
            
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
                        _gameRepo.GamesList[gameIndex].GameEnabled = enableItems;                        
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
                            g.GameEnabled.Equals(enabled)
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
                            g.GameEnabled.Equals(enabled)
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
                              && g.GameEnabled.Equals(enabled);
                        }
                        else if (!favesOnly && showClones && enabledFilter)
                        {
                            textFiltered = g.CloneOf.Length >= 0
                            && g.GameEnabled.Equals(enabled);
                        }
                        else if (!favesOnly && !showClones) { textFiltered = g.CloneOf.Equals(string.Empty); }
                        else if (!favesOnly && showClones) { textFiltered = g.CloneOf.Length >= 0; }
                    }
                    else // Text is used as filter
                    {
                        if (showClones && favesOnly && enabledFilter)
                        {
                            textFiltered = g.Description.ToUpper().Contains(filter.ToUpper())
                            && g.GameEnabled.Equals(enabled)
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
                            && g.GameEnabled.Equals(enabled)
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
                            && g.GameEnabled.Equals(enabled)
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
                            && g.GameEnabled.Equals(enabled)
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

                #endregion


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

        /// <summary>
        /// Called when items are changed in the view
        /// </summary>
        /// <param name="items">The items.</param>
        private void OnMultipleItemsSelectionChanged(IList items)
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
                    _eventAggregator.GetEvent<GameSelectedEvent>().Publish(new string[] { _selectedService.SelectedGames[0].RomName, "" });
                }
                else
                    DatabaseHeaderInfo = "";
            }
            catch (Exception)
            {


            }
        }

        /// <summary>
        /// When games are changed in the list publish the selected game to display media in pane
        /// </summary>
        /// <param name="sender">ICollection</param>
        /// <param name="e"></param>
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

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
        }
        
        private void ScanRoms()
        {
            if (!Directory.Exists(_settingsRepo.HypermintSettings.RlPath)) return;

            try
            {
                if (!_selectedService.CurrentSystem.ToLower().Contains("main menu"))
                {
                    _gameRepo.ScanForRoms(
                        _settingsRepo.HypermintSettings.RlPath,
                        _selectedService.CurrentSystem);
                }

                if (GamesList != null)
                    GamesList.Refresh();
            }
            catch (Exception ex)
            {

            }

        }
        private void SystemDatabaseChangedHandler(string systemName)
        {
            UpdateGamesAsync(systemName);
        }
        private void SaveCurrentMainMenuItems(string xml)
        {
            if (_menuRepo.Systems != null || _menuRepo.Systems.Count > 0)
                _xmlService.SerializeMainMenuXml(
                    _menuRepo.Systems, _settingsRepo.HypermintSettings.HsPath, xml);
        }
        
    }
}
