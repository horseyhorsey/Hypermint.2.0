using Frontends.Models.Hyperspin;
using Hypermint.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Model;
using Hypermint.Base.Models;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System.ComponentModel;
using System.Windows.Input;

namespace Hs.Hypermint.Search.ViewModels
{
    public class SearchOptionsViewModel : ViewModelBase
    {
        #region Fields
        
        private Systems _systems = new Systems();
        private IEventAggregator _ea;
        private ISettingsHypermint _settingsRepo;
        private ISelectedService _selectedSvc;
        #endregion

        #region Commands
        public DelegateCommand<string> SearchGamesCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region Constructor

        public SearchOptionsViewModel(IEventAggregator ea, ISettingsHypermint settings, ISelectedService selectedSrv)
        {
            _ea = ea;
            _settingsRepo = settings;
            _selectedSvc = selectedSrv;

            SearchOptions = new SearchOptions();
            SearchOptions.PropertyChanged += SearchOptions_PropertyChanged;

            //Search for games with the SearchString from this Vm.
            SearchGamesCommand = new DelegateCommand<string>(x =>
            {
                ScanForGames(x);

            }, (x) => CanSearchGames(x)).ObservesProperty(() => CanSearch);

            //Cancel search with token
            CancelCommand = new DelegateCommand(() =>
            {
                SearchOptions.tokenSource.Cancel();
                CanSearch = true;
            });            
        }

        #endregion

        #region Properties

        private SearchOptions _searchOptions ;
        public SearchOptions SearchOptions
        {
            get { return _searchOptions; }
            set { SetProperty(ref _searchOptions, value); }
        }

        private bool canSearch;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can search.
        /// </summary>
        public bool CanSearch
        {
            get { return canSearch; }
            set { SetProperty(ref canSearch, value); }
        }

        #endregion

        #region Support Methods

        /// <summary>
        /// Determines whether this instance [can search games] the specified searchText.
        /// </summary>
        /// <param name="searchText">The search string</param>
        private bool CanSearchGames(string searchText) => searchText?.Length > 3 ? true : false;

        /// <summary>
        /// Scans for games.
        /// </summary>
        /// <param name="x">The x.</param>
        private void ScanForGames(string x)
        {
            CanSearch = false;

            _ea.GetEvent<OnSearchForGames>().Publish(SearchOptions);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the SearchOptions control. Acts on SearchString to enable command
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void SearchOptions_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SearchString")
                CanSearch = CanSearchGames(SearchOptions.SearchString);
        }

        #endregion

    }
}
