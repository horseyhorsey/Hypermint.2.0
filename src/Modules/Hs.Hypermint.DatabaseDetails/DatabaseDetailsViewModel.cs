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

namespace Hs.Hypermint.DatabaseDetails
{
    public class DatabaseDetailsViewModel : ViewModelBase
    {
        private IGameRepo _gameRepo;
        private ISettingsRepo _settingsRepo;
        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand AddGameCommand { get; private set; } 

        #region Properties
        private ICollectionView _gameList;
        public ICollectionView GamesList
        {
            get { return _gameList; }
            set { SetProperty(ref _gameList, value); }
        }

        /// <summary>
        /// Name of new game to add to database
        /// </summary>
        private string newGameName;
        public string NewGameName
        {
            get { return newGameName; }
            set { SetProperty(ref newGameName, value); }
        }
        #endregion

        #region Constructors
        public DatabaseDetailsViewModel(ISettingsRepo settings, IGameRepo gameRepo,
            IFavoriteService favoriteService, IEventAggregator eventAggregator)
        {
            if (gameRepo == null) throw new ArgumentNullException("gameRepo");
            //_getGamesCommand = new Commands.GetGamesCommand()
            _settingsRepo = settings;
            //_settingsRepo.LoadHypermintSettings();
            _gameRepo = gameRepo;
            _eventAggregator = eventAggregator;
            _favouriteService = favoriteService;

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

            if (_gameRepo.GamesList != null)
            {
                GamesList = new ListCollectionView(_gameRepo.GamesList);
                GamesList.CurrentChanged += GamesList_CurrentChanged;
            }

            AddGameCommand = new DelegateCommand(AddGame);

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(UpdateGames);
            _eventAggregator.GetEvent<GameFilteredEvent>().Subscribe(FilterGamesByText);
            _eventAggregator.GetEvent<CloneFilterEvent>().Subscribe(FilterRomClones);           
            
        }

        /// <summary>
        /// Create a new game and add to gameList
        /// </summary>
        private void AddGame()
        {
            _gameRepo.GamesList.Add(
                new Game(NewGameName, NewGameName));
        }

        #endregion

        #region Commands
        private ICommand _getGamesCommand;
        private IFavoriteService _favouriteService;

        public DelegateCommand SaveDb { get; set; }
        public DelegateCommand AuditScanStart { get; private set; }
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
            if (GamesList != null)
            {
                if (Directory.Exists(_settingsRepo.HypermintSettings.HsPath))
                {
                    try
                    {
                        if (systemName.Contains("Main Menu"))
                            _gameRepo.GetGames(_settingsRepo.HypermintSettings.HsPath + @"\Databases\Main Menu\" + systemName + ".xml", systemName);
                        else
                            _gameRepo.GetGames(_settingsRepo.HypermintSettings.HsPath + @"\Databases\" + systemName + "\\" + systemName + ".xml", systemName);

                        GamesList = new ListCollectionView(_gameRepo.GamesList);
                    }
                    catch (Exception exception)
                    {
                        exception.GetBaseException();
                        var msg = exception.Message;
                    }
                    finally
                    {
                        updateFavoritesForGamesList();
                        //Publish after the gameslist is updated here
                        _eventAggregator.GetEvent<GamesUpdatedEvent>().Publish(systemName);
                    }
                }
            }

        }

        private void updateFavoritesForGamesList()
        {
            //var favorites = new Favourite
            //_gameRepo.GamesList
            var favesList = _favouriteService.GetFavoritesForSystem
                ("Amstrad CPC", _settingsRepo.HypermintSettings.HsPath);

            foreach (var item in _gameRepo.GamesList)
            {
                if (favesList.Contains(item.RomName))
                    item.IsFavorite = true;
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
                    _eventAggregator.GetEvent<GameSelectedEvent>().Publish(game.Description);
                }
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
