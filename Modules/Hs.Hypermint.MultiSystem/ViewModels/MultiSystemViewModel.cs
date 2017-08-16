using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System;
using Hs.Hypermint.DatabaseDetails.Services;
using System.IO;
using Hypermint.Base.Constants;
using Hypermint.Base.Services;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using Hs.Hypermint.MultiSystem.Views;
using Frontends.Models.Hyperspin;

namespace Hs.Hypermint.MultiSystem.ViewModels
{
    public class MultiSystemViewModel : ViewModelBase
    {

        #region Properties
        private string message = "Test Message";
        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }

        private ICollectionView multiSystemList;
        public ICollectionView MultiSystemList
        {
            get { return multiSystemList; }
            set { SetProperty(ref multiSystemList, value); }
        }

        private string multiSystemHeader = "Multi System Generator: 0";// Multi system generator;
        public string MultiSystemHeader
        {
            get { return multiSystemHeader; }
            set { SetProperty(ref multiSystemHeader, value); }
        }

        #endregion

        #region Commands
        private IEventAggregator _eventAggregator;
        public DelegateCommand<Game> RemoveGameCommand { get; set; }
        public DelegateCommand ClearListCommand { get; private set; }
        public DelegateCommand BuildMultiSystemCommand { get; private set; }
        public DelegateCommand ScanFavoritesCommand { get; private set; }
        public DelegateCommand<string> OpenSearchCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        #endregion

        #region Services
        private IFileFolderService _fileFolderService;
        private ISettingsRepo _settingsService;
        private IMultiSystemRepo _multiSystemRepo;
        private IHyperspinXmlService _xmlService;
        private IMainMenuRepo _mainmenuRepo;
        private IFavoriteService _favoriteService;
        private IDialogCoordinator _dialogService;
        private CustomDialog customDialog;
        #endregion

        #region Constructors
        public MultiSystemViewModel(IEventAggregator ea, IMultiSystemRepo multiSystem, IFileFolderService fileService,
            IDialogCoordinator dialogService,
          ISettingsRepo settings, IHyperspinXmlService xmlService,
          IMainMenuRepo mainMenuRepo, IFavoriteService favoritesService)
        {
            _eventAggregator = ea;
            _multiSystemRepo = multiSystem;
            _fileFolderService = fileService;
            _settingsService = settings;
            _xmlService = xmlService;
            _mainmenuRepo = mainMenuRepo;
            _favoriteService = favoritesService;
            _dialogService = dialogService;

            _eventAggregator.GetEvent<AddToMultiSystemEvent>().Subscribe(AddToMultiSystem);

            RemoveGameCommand = new DelegateCommand<Game>(RemoveFromMultisystemList);

            ClearListCommand = new DelegateCommand(() =>
            {
                if (_multiSystemRepo.MultiSystemList != null)
                    _multiSystemRepo.MultiSystemList.Clear();
            });

            CloseCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, customDialog);
            });            

            BuildMultiSystemCommand = new DelegateCommand(OpenBuildMultiSystemDialog);

            ScanFavoritesCommand = new DelegateCommand(ScanFavoritesAsync);

            //OpenSearchCommand = new DelegateCommand<string>(async x =>
            //{
            //    await RunCustomDialog();
            //});

        }

        #endregion

        #region Methods

        /// <summary>
        /// Scans all favorites.txt in available systems
        /// Needs to go into each databases xml to pull the info for each game 
        /// ****
        /// Why creating two lists..learn how to find if game.romname exists in collection already
        /// </summary>
        private void ScanFavorites()
        {
            if (_multiSystemRepo.MultiSystemList == null)
                _multiSystemRepo.MultiSystemList = new Games();

            var tempGames = new List<Game>();

            try
            {
                foreach (MainMenu system in _mainmenuRepo.Systems)
                {
                    var favoritesList = new List<string>();

                    var hsPath = _settingsService.HypermintSettings.HsPath;

                    var isMultiSystem = File.Exists(Path.Combine(
                        hsPath, Root.Databases, system.Name, "_multisystem")
                        );

                    // Dont want to be scanning the favorites of a multisystem
                    if (system.Name != "Main Menu" && !isMultiSystem)
                    {
                        favoritesList = _favoriteService.GetFavoritesForSystem(system.Name, hsPath);

                        var games = _xmlService.SearchRomStringsListFromXml(favoritesList, system.Name,
                            hsPath);

                        foreach (Game game in games)
                        {
                            if (!tempGames.Exists(x => x.RomName == game.RomName))
                            {
                                tempGames.Add(game);
                                _multiSystemRepo.MultiSystemList.Add(game);
                            }
                        }
                    }
                }


            }
            catch { }


            MultiSystemList = new ListCollectionView(_multiSystemRepo.MultiSystemList);

        }

        private async void ScanFavoritesAsync()
        {
            var mahSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Scan",
                NegativeButtonText = "Cancel"
            };

            var msg = "Search all systems for favorites. If a system is a existing multi system it will be skipped.";
            var result = await _dialogService.ShowMessageAsync(this, "Scan all favorites", msg,
                MessageDialogStyle.AffirmativeAndNegative, mahSettings);

            if (result == MessageDialogResult.Negative)
                return;

            var tempGames = new List<Game>();
            _multiSystemRepo.MultiSystemList = new Games();

            var progressResult = await _dialogService.ShowProgressAsync(this, "Favorite search", "Searching..");
            progressResult.SetCancelable(true);

            try
            {
                var hsPath = _settingsService.HypermintSettings.HsPath;
                var favoritesList = new List<string>();

                foreach (MainMenu system in _mainmenuRepo.Systems)
                {
                    var isMultiSystem = File.Exists(Path.Combine(
                        hsPath, Root.Databases, system.Name, "_multisystem")
                        );

                    // Dont want to be scanning the favorites of a multisystem
                    if (system.Name != "Main Menu" && !isMultiSystem)
                    {
                        favoritesList = _favoriteService.GetFavoritesForSystem(system.Name, hsPath);

                        if (favoritesList.Count > 0)
                        {
                            var games = _xmlService.SearchRomStringsListFromXml(favoritesList, system.Name,
                                                        hsPath);
                            foreach (Game game in games)
                            {
                                progressResult.SetMessage("System : " + game.RomName);
                                await ScanGames(game, tempGames);
                            }

                        }
                    }
                }
            }
            catch (Exception e) { System.Windows.MessageBox.Show(e.Message); }
            finally
            {
                await progressResult.CloseAsync();
            }

            MultiSystemHeader = string.Format("Multi System Generator: {0} Pending",
                _multiSystemRepo.MultiSystemList.Count);

            MultiSystemList = new ListCollectionView(_multiSystemRepo.MultiSystemList);

        }

        private Task ScanGames(Game game, List<Game> tempGames)
        {
            return Task.Run(() =>
            {
                if (!tempGames.Exists(x => x.RomName == game.RomName))
                {
                    tempGames.Add(game);
                }

                tempGames.Sort();

                _multiSystemRepo.MultiSystemList.Clear();

                foreach (Game sortedGame in tempGames)
                {
                    _multiSystemRepo.MultiSystemList.Add(sortedGame);
                }
            });
        }

        /// <summary>
        /// Remove a single item when X is clicked for a game
        /// </summary>
        /// <param name="game"></param>
        private void RemoveFromMultisystemList(Game game)
        {
            _multiSystemRepo.MultiSystemList.Remove(game);

            MultiSystemHeader = string.Format("Multi System Generator: {0} Pending",
                _multiSystemRepo.MultiSystemList.Count);
        }
        /// <summary>
        /// Add to a multisystem list from the main database menu event
        /// </summary>
        /// <param name="games"></param>
        private void AddToMultiSystem(object games)
        {
            if (_multiSystemRepo.MultiSystemList == null)
            {
                _multiSystemRepo.MultiSystemList = new Games();
                MultiSystemList = new ListCollectionView(_multiSystemRepo.MultiSystemList);
            }

            foreach (var game in (List<Game>)games)
            {
                //Only add the game if it doesn't already exits
                if (!_multiSystemRepo.MultiSystemList.Contains(game))
                    _multiSystemRepo.MultiSystemList.Add(game);
            }

            MultiSystemHeader = string.Format("Multi System Generator: {0} Pending",
                _multiSystemRepo.MultiSystemList.Count);

        }

        private async void OpenBuildMultiSystemDialog()
        {
            customDialog = new CustomDialog() { Title = "Save MultiSystem" };

            customDialog.Content = new SaveMultiSystemView { DataContext = new SaveMultiSystemViewModel(_dialogService, customDialog,_eventAggregator,_settingsService
                ,_multiSystemRepo, _xmlService,_mainmenuRepo,_fileFolderService) };

            await _dialogService.ShowMetroDialogAsync(this, customDialog);
        }

        private async Task ShowCancelGamesSearch()
        {
            var controller = await _dialogService.ShowMessageAsync(this, "Scan cancelled", "Add found games?", MessageDialogStyle.AffirmativeAndNegative);

            if (controller == MessageDialogResult.Affirmative)
            {
                // Add all games that have been scanned
            }
            else
            {
                // Clear the list and return to scan dialog
            }
        }

        #endregion

    }
}
