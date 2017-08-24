using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System;
using Hs.Hypermint.DatabaseDetails.Services;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using Hs.Hypermint.MultiSystem.Views;
using Frontends.Models.Hyperspin;
using Hypermint.Base.Model;
using System.Windows.Input;
using System.Linq;
using Hypermint.Base.Services;
using System.Collections;

namespace Hs.Hypermint.MultiSystem.ViewModels
{
    public class MultiSystemViewModel : ViewModelBase
    {
        #region Services
        private IFileDialogHelper _fileFolderService;
        private ISettingsHypermint _settingsService;
        private IMultiSystemRepo _multiSystemRepo;
        private IHyperspinXmlService _xmlService;
        private IMainMenuRepo _mainmenuRepo;
        private IDialogCoordinator _dialogService;
        private IHyperspinManager _hyperspinManager;
        private ISelectedService _selectedService;
        private CustomDialog customDialog;
        #endregion

        #region Constructors
        public MultiSystemViewModel(IEventAggregator ea, IMultiSystemRepo multiSystem, IFileDialogHelper fileService,
            IDialogCoordinator dialogService, IHyperspinManager hyperspinManager,
          ISettingsHypermint settings, IHyperspinXmlService xmlService,
          IMainMenuRepo mainMenuRepo, ISelectedService selectedService)
        {
            _eventAggregator = ea;
            _multiSystemRepo = multiSystem;
            _fileFolderService = fileService;
            _settingsService = settings;
            _xmlService = xmlService;
            _mainmenuRepo = mainMenuRepo;
            _dialogService = dialogService;
            _hyperspinManager = hyperspinManager;
            _selectedService = selectedService;

            MultiSystemList = new ListCollectionView(_hyperspinManager.MultiSystemGamesList);

            _eventAggregator.GetEvent<AddToMultiSystemEvent>().Subscribe(AddToMultiSystem);
            _eventAggregator.GetEvent<BuildMultiSystemEvent>().Subscribe((x) =>  OpenBuildMultiSystemDialog());
            _eventAggregator.GetEvent<ScanMultiSystemFavoritesEvent>().Subscribe( async (x) =>  await ScanFavoritesAsync());

            //Commands
            RemoveGameCommand = new DelegateCommand<GameItemViewModel>(RemoveFromMultisystemList);

            //OpenSearchCommand = new DelegateCommand<string>(async x =>
            //{
            //    await RunCustomDialog();
            //});

            //Used for the save dialog
            CloseCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, customDialog);
            });

            SelectionChanged = new DelegateCommand<IList>(items => { OnMultipleItemsSelectionChanged(items); });
        }

        /// <summary>
        /// Called when items are changed in the view
        /// </summary>
        /// <param name="items">The items.</param>
        private void OnMultipleItemsSelectionChanged(IList items)
        {
            if (items == null)
            {
                _selectedService.SelectedGames.Clear();
                return;
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

                if (items.Count > 1)
                {
                    _eventAggregator.GetEvent<GameSelectedEvent>().Publish(new string[] { _selectedService.SelectedGames[0].RomName, "" });
                }
            }
            catch { }
        }

        #endregion

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
        public DelegateCommand<GameItemViewModel> RemoveGameCommand { get; set; }
        public DelegateCommand<string> OpenSearchCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand SelectionChanged { get; set; }
        #endregion

        #region Support Methods

        /// <summary>
        /// Scans the all the systems favorites asynchronous.
        /// </summary>
        private async Task ScanFavoritesAsync()
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

            var progressResult = await _dialogService.ShowProgressAsync(this, "Favorite search", "Searching..");
            progressResult.SetCancelable(true);

            _hyperspinManager.MultiSystemGamesList.Clear();

            //Get all favorites
            try
            {
                var hsPath = _settingsService.HypermintSettings.HsPath;
                var favoritesList = new List<string>();
                var faves = await _hyperspinManager.GetGamesFromAllFavorites();

                if (faves != null && faves.Count() > 0)
                {
                    foreach (var fave in faves)
                    {
                        _hyperspinManager.MultiSystemGamesList.Add(new GameItemViewModel(fave));
                    }
                }
            }
            catch (Exception e) { System.Windows.MessageBox.Show(e.Message); }
            finally
            {
                //Close the dialog window
                await progressResult.CloseAsync();
            }

            MultiSystemHeader = string.Format("Multi System Generator: {0} Pending",
                _hyperspinManager.MultiSystemGamesList.Count);

        }

        /// <summary>
        /// Scans the games.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="tempGames">The temporary games.</param>
        /// <returns></returns>
        private Task ScanGames(Game game, List<Game> tempGames)
        {
            return Task.Run(() =>
            {
                if (!tempGames.Exists(x => x.RomName == game.RomName))
                {
                    tempGames.Add(game);
                }

                tempGames.Sort();
                _hyperspinManager.MultiSystemGamesList.Clear();

                foreach (Game sortedGame in tempGames)
                {
                    _multiSystemRepo.MultiSystemList.Add(sortedGame);
                }
            });
        }

        /// <summary>
        /// Add to a multisystem list from the main database menu event with a hyperspin manager.
        /// </summary>
        /// <param name="games"></param>
        private void AddToMultiSystem(IEnumerable<GameItemViewModel> games)
        {
            if (_hyperspinManager.MultiSystemGamesList == null) throw new NullReferenceException("Multi system games list is null!");

            foreach (var game in games)
            {
                //Only add the game if it doesn't already exits
                if (!_hyperspinManager.MultiSystemGamesList.Contains(game))
                    _hyperspinManager.MultiSystemGamesList.Add(game);
            }

            MultiSystemHeader = string.Format("Multi System Generator: {0} Pending",
               _hyperspinManager.MultiSystemGamesList.Count);
        }

        /// <summary>
        /// Remove a single item when X is clicked for a game
        /// </summary>
        /// <param name="game"></param>
        private void RemoveFromMultisystemList(GameItemViewModel game)
        {
            _hyperspinManager.MultiSystemGamesList.Remove(game);

            MultiSystemHeader = string.Format("Multi System Generator: {0} Pending",
                _hyperspinManager.MultiSystemGamesList.Count);
        }

        /// <summary>
        /// Opens the build multi system dialog.
        /// </summary>
        private async void OpenBuildMultiSystemDialog()
        {
            customDialog = new CustomDialog() { Title = "Save MultiSystem" };

            customDialog.Content = new SaveMultiSystemView { DataContext = new SaveMultiSystemViewModel(_dialogService, customDialog,_eventAggregator,_settingsService
                ,_multiSystemRepo, _hyperspinManager, _xmlService,_mainmenuRepo,_fileFolderService, _selectedService) };

            await _dialogService.ShowMetroDialogAsync(this, customDialog);
        }

        /// <summary>
        /// Shows the cancel games search.
        /// </summary>
        /// <returns></returns>
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
