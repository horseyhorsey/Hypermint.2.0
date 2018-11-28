using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;

namespace Hypermint.Base
{
    /// <summary>
    /// A base that has commands for launching games.
    /// </summary>
    /// <seealso cref="Hypermint.Base.Base.ViewModelBase" />
    public abstract class HyperMintModelBase : ViewModelBase
    {
        public HyperMintModelBase(IEventAggregator evtAggregator, ISelectedService selectedService, IGameLaunch gameLaunch, ISettingsHypermint settingsRepo)
        {
            _eventAggregator = evtAggregator;
            _selectedService = selectedService;
            _gameLaunch = gameLaunch;
            _settingsRepo = settingsRepo;

            AddMultiSystemCommand = new DelegateCommand(() => AddToMultiSystem());
            LaunchGameCommand = new DelegateCommand(() => LaunchGame());
        }

        #region Commands
        public DelegateCommand AddMultiSystemCommand { get; private set; }
        public DelegateCommand LaunchGameCommand { get; private set; }
        #endregion

        #region Fields
        protected IEventAggregator _eventAggregator;
        protected ISelectedService _selectedService;
        protected IGameLaunch _gameLaunch;
        protected ISettingsHypermint _settingsRepo;
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a game to the multisystem via the event aggregator.
        /// </summary>
        public virtual void AddToMultiSystem()
        {
            _eventAggregator.GetEvent<AddToMultiSystemEvent>().Publish(_selectedService.SelectedGames);
        }

        /// <summary>
        /// Launches a game with RocketLauncher.
        /// </summary>
        public virtual void LaunchGame()
        {
            if (_selectedService.SelectedGames == null) return;

            if (_selectedService.SelectedGames.Count != 0)
            {
                var rlPath = _settingsRepo.HypermintSettings.RlPath;
                var hsPath = _settingsRepo.HypermintSettings.HsPath;
                var sysName = _selectedService.SelectedGames[0].System;
                var romName = _selectedService.SelectedGames[0].Name;

                _gameLaunch.RocketLaunchGame(rlPath, sysName, romName, hsPath);
            }            
        }
        #endregion        
    }
}
