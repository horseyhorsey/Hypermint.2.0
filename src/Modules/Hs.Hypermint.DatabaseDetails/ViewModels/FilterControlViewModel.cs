using Hypermint.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Models;
using Prism.Events;
using Hypermint.Base.Services;
using Hypermint.Base.Interfaces;
using System.ComponentModel;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class FilterControlViewModel : HyperMintModelBase
    {
        #region Constructors
        public FilterControlViewModel(IEventAggregator eventAggregator,
            ISelectedService selectedSrv, ISettingsHypermint settingsRepo,
            IGameLaunch gameLaunch) : base(eventAggregator,selectedSrv,gameLaunch,settingsRepo)
        {
            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(RemoveFilters);
        }
        #endregion

        #region Fields
        private bool systemChanging;
        #endregion

        #region Properties
        private string filterText;
        public string FilterText
        {
            get { return filterText; }
            set { SetProperty(ref filterText, value);}
        }

        private bool showClones = true;
        public bool ShowClones
        {
            get { return showClones; }
            set
            {
                SetProperty(ref showClones, value);
                RaisePropertyChanged(nameof(ShowClones));
            }
        }

        private bool showFavoritesOnly;
        public bool ShowFavoritesOnly
        {
            get { return showFavoritesOnly; }
            set { SetProperty(ref showFavoritesOnly, value); }
        }

        private bool showEnabledOnly;
        public bool ShowEnabledOnly
        {
            get { return showEnabledOnly; }
            set { SetProperty(ref showEnabledOnly, value); }
        }

        private bool _isMainMenu;
        /// <summary>
        /// Gets or sets if main menu. Used to disable some controls.
        /// </summary>
        public bool IsMainMenu
        {
            get { return _isMainMenu; }
            set { SetProperty(ref _isMainMenu, value); }
        }
        
        #endregion

        #region Support Methods

        private void RemoveFilters(string obj)
        {
            IsMainMenu = _selectedService.IsMainMenu();
        
            systemChanging = true;
            FilterText = "";
            ShowClones = true;
            ShowFavoritesOnly = false;
            ShowEnabledOnly = false;
            systemChanging = false;
        }

        /// <summary>
        /// Publish the filter text to the details view
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (!systemChanging)
            {
                //Add the options to a dictionary passed to the filter
                var filterOptions = new GameFilter
                {
                    FilterText = FilterText,
                    ShowClones = ShowClones,
                    ShowFavoritesOnly = ShowFavoritesOnly,
                    ShowEnabledOnly = ShowEnabledOnly
                };

                _eventAggregator.GetEvent<GameFilteredEvent>().Publish(filterOptions);
            }
        }

        #endregion

    }

}
