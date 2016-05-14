using Hs.HyperSpin.Database;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Hypermint.Base.Services;
using Prism.Events;
using Hypermint.Base;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Threading;
using Hs.Hypermint.DatabaseDetails.Services;

namespace Hs.Hypermint.Search.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        public DelegateCommand<string> SearchGamesCommand { get; }
        public DelegateCommand<string> SelectSystemsCommand { get; }
        public DelegateCommand CancelCommand { get;}
        public DelegateCommand LaunchGameCommand { get; private set; }
        public DelegateCommand AddMultiSystemCommand { get; private set; } 


        #region Properties

        CancellationTokenSource tokenSource = new CancellationTokenSource();

        private IMainMenuRepo _mainmenuRepo;
        private ICollectionView systems;
        private IEventAggregator _eventAggregator;
        private ISettingsRepo _settings;
        private IGameLaunch _gameLaunch;

        private bool canSearch = true;

        private Systems _systems = new Systems();
        public ICollectionView Systems
        {
            get { return systems; }
            set { SetProperty(ref systems, value); }
        }
        private List<Game> searchedGames;
        private ICollectionView foundGmes;
        private IHyperspinXmlService _xmlService;

        public ICollectionView FoundGmes
        {
            get { return foundGmes; }
            set { SetProperty(ref foundGmes, value); }
        }
        #endregion

        public SearchViewModel(IMainMenuRepo mainMenu, 
            IEventAggregator eventAggregator, 
            ISettingsRepo settings,
            IHyperspinXmlService xmlService,
            IGameLaunch gameLaunch)
        {
            _mainmenuRepo = mainMenu;
            _eventAggregator = eventAggregator;
            _settings = settings;
            _gameLaunch = gameLaunch;
            _xmlService = xmlService;

            SearchGamesCommand = new DelegateCommand<string>(async x =>
            {
                await ScanForGamesAsync(x);
            }, o => canSearch);            

            CancelCommand = new DelegateCommand(() =>
            {
                tokenSource.Cancel();
                canSearch = true;
                SearchGamesCommand.RaiseCanExecuteChanged();
            });

            //AddMultiSystemCommand = new DelegateCommand(() =>
            //{
            //    _eventAggregator.GetEvent<AddToMultiSystemEvent>().Publish(_selectedService.SelectedGames);
            //});

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

            LaunchGameCommand = new DelegateCommand(LaunchGame);

        _eventAggregator.GetEvent<SystemsGenerated>().Subscribe(x => SystemsUpdated(x));
        }

        private void LaunchGame()
        {
            if (FoundGmes != null)
            {
                var rlPath = _settings.HypermintSettings.RlPath;
                var hsPath = _settings.HypermintSettings.HsPath;
                var selectedGame = FoundGmes.CurrentItem as Game;                
                
                var sysName = selectedGame.System;
                var romName = selectedGame.RomName;

                _gameLaunch.RocketLaunchGame(rlPath, sysName, romName, hsPath);
            }
        }

        private void SystemsUpdated(string system)
        {
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
                                Enabled = 0
                            });
                        }
                    }

                    Systems = new ListCollectionView(_systems);
                }
            }
            catch (Exception)
            {


            }

        }

        /// <summary>
        /// Method to scan from button inside the custom search dialog
        /// </summary>
        /// <returns></returns>
        private async Task ScanForGamesAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return;            

            tokenSource = new CancellationTokenSource();

            canSearch = false;
            SearchGamesCommand.RaiseCanExecuteChanged();

            searchedGames = new List<Game>();

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
                                system.Name, searchTerm);

                            foreach (var game in games)
                            {
                                searchedGames.Add(game);
                            } 

                        }
                    }
                    catch (Exception ex) { }                    
                }

                FoundGmes = new ListCollectionView(searchedGames);

            }, tokenSource.Token);

            canSearch = true;
            SearchGamesCommand.RaiseCanExecuteChanged();

        }
        
    }

}

