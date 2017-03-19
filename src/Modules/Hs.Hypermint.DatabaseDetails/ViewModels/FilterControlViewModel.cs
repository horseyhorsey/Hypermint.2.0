using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Models;
using Prism.Events;
using Prism.Commands;
using Hypermint.Base.Services;
using Hypermint.Base.Interfaces;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class FilterControlViewModel : ViewModelBase
    {
        #region Constructors
        public FilterControlViewModel(IEventAggregator eventAggregator,
            ISelectedService selectedSrv
            , ISettingsRepo settingsRepo,
            IGameLaunch gameLaunch)
        {
            _eventAggregator = eventAggregator;
            _selectedService = selectedSrv;
            _settingsRepo = settingsRepo;
            _gameLaunch = gameLaunch;

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(RemoveFilters);

            LaunchGameCommand = new DelegateCommand(LaunchGame);

            AddMultiSystemCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<AddToMultiSystemEvent>().Publish(_selectedService.SelectedGames);
            });
        }
        #endregion

        #region Fields
        private IEventAggregator _eventAggregator;

        private bool systemChanging;

        private ISelectedService _selectedService;
        private ISettingsRepo _settingsRepo;
        private IGameLaunch _gameLaunch;
        #endregion

        #region Commands
        public DelegateCommand LaunchGameCommand { get; private set; }
        public DelegateCommand AddMultiSystemCommand { get; private set; } 
        #endregion        

        #region Properties
        private string filterText;
        public string FilterText
        {
            get { return filterText; }
            set
            {
                SetProperty(ref filterText, value);
                OnPropertyChanged(() => FilterText);
            }
        }

        private bool showClones = true;
        public bool ShowClones
        {
            get { return showClones; }
            set
            {
                SetProperty(ref showClones, value);
                OnPropertyChanged(() => ShowClones);
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
        #endregion

        #region Support Methods
        private void RemoveFilters(string obj)
        {
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
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

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

        private void LaunchGame()
        {
            if (_selectedService.SelectedGames == null) return;

            if (_selectedService.SelectedGames.Count != 0)
            {
                var rlPath = _settingsRepo.HypermintSettings.RlPath;
                var hsPath = _settingsRepo.HypermintSettings.HsPath;
                var sysName = _selectedService.SelectedGames[0].System;
                var romName = _selectedService.SelectedGames[0].RomName;

                _gameLaunch.RocketLaunchGame(rlPath, sysName, romName, hsPath);
            }
        } 
        #endregion

    }

}
