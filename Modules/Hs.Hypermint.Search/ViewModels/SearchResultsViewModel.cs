using Hs.Hypermint.DatabaseDetails.Services;
using Hypermint.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Model;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;

namespace Hs.Hypermint.Search.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        #region Fields
        private IEventAggregator _ea;
        private ISettingsHypermint _settingsRepo;
        private ISelectedService _selectedSvc;
 
        private IHyperspinXmlDataProvider _hsXmlPRovider;
        private IHyperspinManager _hyperspinManager;

        private int pageCount;
        private int currentPage;
        private string _noWheelImage = Path.Combine(Environment.CurrentDirectory, "Images\\noimage.png");
        #endregion

        #region Constructor
        public SearchResultsViewModel(IEventAggregator ea, ISettingsHypermint settingsRepo, 
            ISelectedService selectedSvc, IHyperspinXmlService xmlService, IHyperspinXmlDataProvider hsXmlPRovider, 
            IHyperspinManager hyperspinManager )
        {
            _ea = ea;
            _settingsRepo = settingsRepo;
            _selectedSvc = selectedSvc;
            _hsXmlPRovider = hsXmlPRovider;
            _hyperspinManager = hyperspinManager;

            SearchGames = new ObservableCollection<GameSearch>();
            FoundGames = new ListCollectionView(SearchGames);

            PageGamesCommand = new DelegateCommand<string>(x => PageGames(x));

            //Scan games on this event
            _ea.GetEvent<OnSearchForGames>().Subscribe( searchOptions =>  ScanForGamesAsync(searchOptions, false));

            SelectionChanged = new DelegateCommand<IList>(items => { OnMultipleItemsSelectionChanged(items); });

        }
        #endregion

        #region Properties

        private ObservableCollection<GameSearch> searchedGames;
        public ObservableCollection<GameSearch> SearchGames
        {
            get { return searchedGames; }
            set { SetProperty(ref searchedGames, value); }
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
        /// <summary>
        /// Gets or sets the filtered games.
        /// </summary>
        public ICollectionView FilteredGames
        {
            get { return filteredGames; }
            set { SetProperty(ref filteredGames, value); }
        }

        /// <summary>
        /// Gets or sets the games found count. << UNEEDED
        /// </summary>
        public int GamesFoundCount { get; set; }

        private string pageInfo;        

        public string PageInfo
        {
            get { return pageInfo; }
            set { SetProperty(ref pageInfo, value); }
        }

        #endregion

        #region Commands
        public ICommand PageGamesCommand { get; private set; }
        public ICommand SelectionChanged { get; set; }
        #endregion

        #region Support Methods

        /// <summary>
        /// Pages the games.
        /// </summary>
        /// <param name="direction">The direction.</param>
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

        private void OnMultipleItemsSelectionChanged(IList items)
        {
            if (items == null)
            {
                _selectedSvc.SelectedGames.Clear();
                return;
            }

            try
            {
                _selectedSvc.SelectedGames.Clear();
                foreach (var item in items)
                {
                    var game = item as GameSearch;

                    if (game.Game.RomName != null)
                        _selectedSvc.SelectedGames.Add(new GameItemViewModel(game.Game));
                };
            }
            catch (Exception)
            {


            }
        }

        /// <summary>
        /// Handles the CurrentChanged event of the FilteredGames control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FilteredGames_CurrentChanged(object sender, EventArgs e)
        {
            _selectedSvc.SelectedGames.Clear();

            var game = FilteredGames.CurrentItem as GameSearch;

            if (game != null)
                _selectedSvc.SelectedGames.Add(new GameItemViewModel(game.Game));
        }

        /// <summary>
        /// Scans for games asynchronous.
        /// </summary>
        /// <param name="searchOptions">The search options.</param>
        /// <param name="meh">if set to <c>true</c> [meh].</param>
        private async void ScanForGamesAsync(SearchOptions searchOptions, bool meh= false)
        {
            SearchGames.Clear();    

            foreach (var system in _hyperspinManager.Systems)
            {
                //xml path for system
                var dbPath = Path.Combine(_settingsRepo.HypermintSettings.HsPath, "Databases", system.Name, $"{system.Name}.xml");

                //only scan if xml exists
                if (File.Exists(dbPath))
                {
                    try
                    {
                        
                        var games = await _hsXmlPRovider.SearchXmlAsync(_settingsRepo.HypermintSettings.HsPath, system.Name, dbPath, searchOptions.SearchString);
                        foreach (var game in games)
                        {                            
                            //Get the wheel image if game has one or set to no image.
                            var wheelImage = Path.Combine(_settingsRepo.HypermintSettings.HsPath, "Media", system.Name, "Images\\Wheel\\" + game.RomName + ".png");
                            if (!File.Exists(wheelImage))
                                wheelImage = _noWheelImage;

                            SearchGames.Add(new GameSearch { Game = game , WheelImage = wheelImage });
                        }

                        GamesFoundCount = searchedGames.Count;

                        currentPage = 1;
                        pageCount = GamesFoundCount / 5;
                        if (GamesFoundCount % 5 != 0)
                            pageCount++;

                        PageInfo = currentPage + " | " + pageCount + "      Games found: " + GamesFoundCount;

                        FilteredGames = new ListCollectionView(searchedGames.Take(5).ToList());

                        FilteredGames.CurrentChanged += FilteredGames_CurrentChanged;

                    }

                    catch (Exception ex) { _ea.GetEvent<ErrorMessageEvent>().Publish($"Error searching: {ex.Message}"); }
                }                
            }
        }

        #endregion
    }
}
