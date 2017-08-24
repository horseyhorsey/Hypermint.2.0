using Hypermint.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Model;
using Hypermint.Base.Models;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class GamesListDataGridViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IHyperspinManager _hyperspinManager;
        private ISelectedService _selectedService;

        public ICommand SelectionChanged { get; set; }

        public GamesListDataGridViewModel(IEventAggregator eventAggregator, 
            IHyperspinManager hyperspinManager, ISelectedService selectedService)
        {
            _eventAggregator = eventAggregator;
            _hyperspinManager = hyperspinManager;
            _selectedService = selectedService;

            //Set to the main menu games list
            _selectedService.CurrentSystem = "Main Menu";
            _selectedService.SelectedGames = new List<GameItemViewModel>();            

            SelectionChanged = new DelegateCommand<IList>(items => { OnMultipleItemsSelectionChanged(items); });

            //Set the observable game to a collection for the view.
            GamesList = new ListCollectionView(_hyperspinManager.CurrentSystemsGames);
            GamesList.CurrentChanged += GamesList_CurrentChanged;

            _eventAggregator.GetEvent<GameFilteredEvent>().Subscribe(FilterGamesByText);
            _eventAggregator.GetEvent<SystemDatabaseChanged>().Subscribe(SystemDatabaseChangedHandler);
            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(UpdateGamesAsync);
            _eventAggregator.GetEvent<UserRequestUpdateSelectedRows>().Subscribe((x) => UpdateSelectedRows(x));

            Load();
        }

        #region Properties

        private ICollectionView _gameList;
        public ICollectionView GamesList
        {
            get { return _gameList; }
            set { SetProperty(ref _gameList, value); }
        }

        private GameItemViewModel _selectedGame;        
        public GameItemViewModel SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                SetProperty(ref _selectedGame, value);
                DatabaseHeaderInfo = "Selected items: " + 1;
            }
        }

        public int SelectedItemsCount { get; private set; }

        private string databaseHeaderInfo = "Database Editor";
        public string DatabaseHeaderInfo
        {
            get { return databaseHeaderInfo; }
            set { SetProperty(ref databaseHeaderInfo, value); }
        }

        private bool _isMultiSystem;
        public bool IsMultiSystem
        {
            get { return _isMultiSystem; }
            set { SetProperty(ref _isMultiSystem, value); }
        }

        #endregion

        #region Support Methods

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
                    var g = o as GameItemViewModel;
                    var textFiltered = false;

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
                
            }
        }

        /// <summary>
        /// When games are changed in the list publish the selected game to display media in pane
        /// </summary>
        /// <param name="sender">ICollection</param>
        /// <param name="e"></param>
        private void GamesList_CurrentChanged(object sender, EventArgs e)
        {
            if (_hyperspinManager.CurrentSystemsGames.Count > 0 && GamesList != null)
            {
                var game = GamesList.CurrentItem as GameItemViewModel;

                if (game != null)
                {
                    _eventAggregator.GetEvent<GameSelectedEvent>().Publish(new string[] { game.Name, "" });
                }
            }
        }

        /// <summary>
        /// Loads this the games for this view model on load.
        /// </summary>
        public async void Load()
        {
            var games = await _hyperspinManager.SetGamesForSystem("Main Menu");

            foreach (var game in games)
            {
                _hyperspinManager.CurrentSystemsGames.Add(new GameItemViewModel(game));
            }

           // _eventAggregator.GetEvent<GamesUpdatedEvent>().Publish("");
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
                    var game = item as GameItemViewModel;
                    if (game.Name != null)
                        _selectedService.SelectedGames.Add(item as GameItemViewModel);
                }

                if (SelectedItemsCount > 1)
                {
                    //_eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish("");
                    _eventAggregator.GetEvent<GameSelectedEvent>().Publish(new string[] { _selectedService.SelectedGames[0].RomName, "" });
                    DatabaseHeaderInfo = "Selected items: " + SelectedItemsCount;
                }                    
                else
                    DatabaseHeaderInfo = "";                
            }
            catch { }
        }

        /// <summary>
        /// Updates the games list when system is changed.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        private void SystemDatabaseChangedHandler(string systemName)
        {
            UpdateGamesAsync(systemName);
        }

        /// <summary>
        /// Updates the games asynchronous.
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        private async void UpdateGamesAsync(string dbName)
        {
            var system = _selectedService.CurrentSystem;
            var hsPath = _hyperspinManager._hyperspinFrontEnd.Path;

            this.FilterGamesByText(new GameFilter() { ShowClones = true, FilterText = "", ShowEnabledOnly = false, ShowFavoritesOnly = false });

            if (GamesList != null)
            {
                try
                {
                    var games = await _hyperspinManager.SetGamesForSystem(system, dbName);
                    
                    //Genres
                    try
                    {
                        await _hyperspinManager.GetGenreDatabases(system);
                    }
                    catch (Exception) { }

                    //Build the viewmodel
                    foreach (var game in games)
                    {
                        _hyperspinManager.CurrentSystemsGames.Add(new GameItemViewModel(game));
                    }

                    if (!system.Contains("Main Menu"))
                        await _hyperspinManager.GetFavoritesForSystem(system);

                    //Publish after the gameslist is updated here
                    _eventAggregator.GetEvent<GamesUpdatedEvent>().Publish(dbName);
                }
                catch (FileNotFoundException)
                {
                    _eventAggregator.GetEvent<ErrorMessageEvent>().Publish($"Couldn't find a database xml for system: {dbName}");
                    _hyperspinManager.CurrentSystemsGames.Clear();
                }
                catch (System.Xml.XmlException)
                {
                    _eventAggregator.GetEvent<ErrorMessageEvent>().Publish($"Error parsing xml for {system}");
                    _hyperspinManager.CurrentSystemsGames.Clear();
                }
                catch (Exception) { throw; }
                finally
                {

                }
            }
        }

        /// <summary>
        /// Updates the selected rows from a UserRequestRowMessage
        /// </summary>
        /// <param name="x">The x.</param>
        private void UpdateSelectedRows(UserRequestRowMessage msg)
        {
            try
            {
                foreach (var item in msg.GameItems)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(item.Name))
                        {
                            var game = _hyperspinManager.CurrentSystemsGames.FirstOrDefault(y => y.Name == item.Name);

                            switch (msg.RowUpdateType)
                            {
                                case RowUpdateType.Description:
                                    game.Description = (string)msg.Value;
                                    break;
                                case RowUpdateType.Genre:
                                    game.Genre = (string)msg.Value;
                                    break;
                                case RowUpdateType.Manufacturer:
                                    game.Manufacturer = (string)msg.Value;
                                    break;
                                case RowUpdateType.Rating:
                                    game.Rating = (string)msg.Value;
                                    break;
                                case RowUpdateType.Year:
                                    int yr = 0000;
                                    int.TryParse((string)msg.Value, out yr);
                                    game.Year = yr;
                                    break;
                                case RowUpdateType.Enabled:
                                    game.GameEnabled = (int)msg.Value;
                                    break;
                                case RowUpdateType.Favorite:
                                    game.IsFavorite = Convert.ToBoolean((int)msg.Value);
                                    break;
                                default:
                                    break;
                            }
                        }
                            
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        } 

        #endregion
    }
}
