using Hs.HyperSpin.Database;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Hypermint.Base.Services;
using Prism.Events;
using Hypermint.Base;
using System.IO;
using System.Threading;
using Hs.Hypermint.DatabaseDetails.Services;
using Hypermint.Base.Constants;
using System.Collections;

namespace Hs.Hypermint.Search.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        #region Constructors
        public SearchViewModel(IMainMenuRepo mainMenu,
            IEventAggregator eventAggregator,
            ISettingsRepo settings,
            IHyperspinXmlService xmlService,
            ISelectedService selectedSrv,
            IGameLaunch gameLaunch)
        {
            _mainmenuRepo = mainMenu;
            _eventAggregator = eventAggregator;
            _settings = settings;
            _gameLaunch = gameLaunch;
            _xmlService = xmlService;
            _selectedSrv = selectedSrv;

            //Search for games with the SearchString from this Vm.
            SearchGamesCommand = new DelegateCommand<string>(async x =>
            {
                x = SearchString;

                if (x.Length > 3)
                {
                    await ScanForGamesAsync(x);
                }
            }, o => canSearch);

            CancelCommand = new DelegateCommand(() =>
            {
                tokenSource.Cancel();
                canSearch = true;
                SearchGamesCommand.RaiseCanExecuteChanged();
            });

            PageGamesCommand = new DelegateCommand<string>(x => PageGames(x));

            AddMultiSystemCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<AddToMultiSystemEvent>().Publish(_selectedSrv.SelectedGames);
            });

            SelectSystemsCommand = new DelegateCommand<string>(x =>
            {
                try
                {
                    var sysEnabled = 0;

                    if (x == "all")
                        sysEnabled = 1;

                    foreach (MainMenu item in _systems)
                    {
                        item.Enabled = sysEnabled;
                    }

                    Systems.Refresh();

                }
                catch (Exception) { }

            });

            // Command for datagrid selectedItems
            SelectionChanged = new DelegateCommand<IList>(
                items =>
                {
                    if (items == null)
                    {
                        _selectedSrv.SelectedGames.Clear();
                        return;
                    }

                    try
                    {
                        _selectedSrv.SelectedGames.Clear();
                        foreach (var item in items)
                        {
                            var game = item as GameSearch;

                            if (game.Game.RomName != null)
                                _selectedSrv.SelectedGames.Add(game.Game);
                        };
                    }
                    catch (Exception)
                    {


                    }

                });

            LaunchGameCommand = new DelegateCommand(LaunchGame);

            DockSystemsCommand = new DelegateCommand(() => { SystemsVisible = !SystemsVisible; });

            _eventAggregator.GetEvent<SystemsGenerated>().Subscribe(x => SystemsUpdated(x));

        }
        #endregion

        #region Properties

        private ICollectionView systems;
        public ICollectionView Systems
        {
            get { return systems; }
            set { SetProperty(ref systems, value); }
        }
        public int GamesFoundCount { get; set; }

        private string pageInfo;
        public string PageInfo
        {
            get { return pageInfo; }
            set { SetProperty(ref pageInfo, value); }
        }

        private bool enabledSearchOn = true;
        public bool EnabledSearchOn
        {
            get { return enabledSearchOn; }
            set { SetProperty(ref enabledSearchOn, value); }
        }

        private bool cloneSearchOn = true;
        public bool CloneSearchOn
        {
            get { return cloneSearchOn; }
            set { SetProperty(ref cloneSearchOn, value); }
        }

        /// <summary>
        /// Full collection view list
        /// </summary>
        private ICollectionView foundGmes;
        public ICollectionView FoundGames
        {
            get { return foundGmes; }
            set { SetProperty(ref foundGmes, value); }
        }

        private ICollectionView filteredGames;
        public ICollectionView FilteredGames
        {
            get { return filteredGames; }
            set { SetProperty(ref filteredGames, value); }
        }

        private List<GameSearch> searchedGames;

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set { SetProperty(ref _searchString, value); }
        }

        private bool systemsVisible = true;
        public bool SystemsVisible
        {
            get { return systemsVisible; }
            set { SetProperty(ref systemsVisible, value); }
        }


        #endregion

        #region Fields
        private bool canSearch = true;
        private Systems _systems = new Systems();

        /// <summary>
        /// The current page count
        /// </summary>
        private int pageCount;

        /// <summary>
        /// The current search page
        /// </summary>
        private int currentPage;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        #endregion

        #region Repos
        private IMainMenuRepo _mainmenuRepo;
        private IEventAggregator _eventAggregator;
        private ISettingsRepo _settings;
        private IGameLaunch _gameLaunch;
        private IHyperspinXmlService _xmlService;
        private ISelectedService _selectedSrv;
        #endregion

        #region Commands
        public DelegateCommand<string> SearchGamesCommand { get; }
        public DelegateCommand<string> SelectSystemsCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand LaunchGameCommand { get; private set; }
        public DelegateCommand AddMultiSystemCommand { get; private set; }
        public DelegateCommand<string> PageGamesCommand { get; private set; }
        public DelegateCommand<IList> SelectionChanged { get; set; }
        public DelegateCommand<IList> ListBoxChanged { get; private set; }
        public DelegateCommand DockSystemsCommand { get; private set; }     

        #endregion

        #region Support Methods
        private void LaunchGame()
        {
            if (FoundGames != null)
            {
                var selectedGame = _selectedSrv.SelectedGames.FirstOrDefault();

                if (selectedGame != null)
                {
                    var rlPath = _settings.HypermintSettings.RlPath;
                    var hsPath = _settings.HypermintSettings.HsPath;

                    var sysName = selectedGame.System;
                    var romName = selectedGame.RomName;

                    _gameLaunch.RocketLaunchGame(rlPath, sysName, romName, hsPath);
                }
            }
        }

        private void SystemsUpdated(string system)
        {

            _systems = new Systems();

            try
            {
                if (_mainmenuRepo.Systems != null)
                {
                    foreach (var item in _mainmenuRepo.Systems)
                    {
                        if (!item.Name.Contains("Main Menu"))
                        {
                            _systems.Add(new MainMenu()
                            {
                                Name = item.Name,
                                SysIcon = item.SysIcon,
                                Enabled = 1
                            });
                        }
                    }

                    Systems = new ListCollectionView(_systems);
                }
            }
            catch (Exception) { }

        }

        /// <summary>
        /// Method to scan from button inside the custom search dialog
        /// </summary>
        /// <returns></returns>
        private async Task ScanForGamesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return;

            tokenSource = new CancellationTokenSource();

            canSearch = false;
            SearchGamesCommand.RaiseCanExecuteChanged();

            searchedGames = new List<GameSearch>();

            await Task.Run(() =>
            {
                foreach (var system in _systems)
                {
                    try
                    {
                        if (system.Enabled == 1)
                        {
                            var games = _xmlService.SearchGames(
                                _settings.HypermintSettings.HsPath,
                                system.Name, searchTerm,CloneSearchOn,EnabledSearchOn);

                            foreach (var game in games)
                            {
                                var mediaPath = Path.Combine(_settings.HypermintSettings.HsPath, Root.Media);

                                game.GameEnabled = 1;

                                var systemImage = Path.Combine(mediaPath, "Main Menu",
                                    Images.Wheels, system.Name + ".png");

                                var wheelImage = Path.Combine(mediaPath, system.Name,
                                    Images.Wheels, game.RomName + ".png");

                                searchedGames.Add(new GameSearch()
                                {
                                    Game = game,
                                    SystemImage = systemImage,
                                    WheelImage = wheelImage
                                });
                            }

                        }
                    }
                    catch (Exception ex) { }
                }

                FoundGames = new ListCollectionView(searchedGames);
                GamesFoundCount = searchedGames.Count;
                currentPage = 1;

                pageCount =  GamesFoundCount / 5 ;
                if (GamesFoundCount % 5 != 0)
                    pageCount++;

                PageInfo = currentPage + " | " + pageCount + "      Games found: " + GamesFoundCount;

                FilteredGames = new ListCollectionView(searchedGames.Take(5).ToList());

                FilteredGames.CurrentChanged += FilteredGames_CurrentChanged;

            }, tokenSource.Token);

            canSearch = true;
            SearchGamesCommand.RaiseCanExecuteChanged();

        }
        private void PageGames(string direction)
        {
            if (GamesFoundCount == 0) return;

            if (direction == "forward")
            {
                if (currentPage < pageCount)
                {
                    FilteredGames = new ListCollectionView(
                    searchedGames
                    .Skip(5 * currentPage)
                    .Take(5)
                    .ToList());

                    currentPage++;
                }
            }
            else
            {
                if (currentPage > 1)
                {
                    currentPage--;

                    if (currentPage == 1)
                        FilteredGames =
                            new ListCollectionView(
                                searchedGames
                                .Take(5).ToList());
                    else
                        FilteredGames = new ListCollectionView(
                        searchedGames
                        .Skip(5 * (currentPage - 1))
                        .Take(5)
                        .ToList());
                }
            }

            FilteredGames.CurrentChanged += FilteredGames_CurrentChanged;

            PageInfo = currentPage + " | " + pageCount + "  Games found: " + GamesFoundCount;
        }
        private void FilteredGames_CurrentChanged(object sender, EventArgs e)
        {
            _selectedSrv.SelectedGames.Clear();

            var game = FilteredGames.CurrentItem as GameSearch;

            if (game != null)
                _selectedSrv.SelectedGames.Add(game.Game);

        }
        #endregion
    }

    #region Support classes
    public class GameSearch
    {
        public Game Game { get; set; }
        public string WheelImage { get; set; }

        public string SystemImage { get; set; }
    }

    #endregion   
}

