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

namespace Hs.Hypermint.DatabaseDetails
{
    public class DatabaseDetailsViewModel : ViewModelBase
    {

        #region Constructors
        public DatabaseDetailsViewModel(ISettingsRepo settings, IGameRepo gameRepo, IEventAggregator eventAggregator)
        {
            if (gameRepo == null) throw new ArgumentNullException("gameRepo");
            //_getGamesCommand = new Commands.GetGamesCommand()
            _settingsRepo = settings;
            //_settingsRepo.LoadHypermintSettings();
            _gameRepo = gameRepo;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(UpdateGames);

            _gameRepo.GetGames( _settingsRepo.HypermintSettings.HsPath + @"\Databases\Main Menu\Main Menu.xml");
                        
            GamesList = new ListCollectionView(_gameRepo.GamesList);
                                  
            AuditScanStart = new DelegateCommand(AuditRunScan);

            GamesList.CurrentChanged += GamesList_CurrentChanged;
            
        }
        #endregion

        #region Properties
        private ICollectionView _gameList;
        public ICollectionView GamesList
            {
                get { return _gameList; }
                set { SetProperty(ref _gameList, value); }
            }
        #endregion

        private IGameRepo _gameRepo;
        private readonly IEventAggregator _eventAggregator;

        #region Commands
        private ICommand _getGamesCommand;
        private ISettingsRepo _settingsRepo;

        public DelegateCommand SaveDb { get; set; }
        public DelegateCommand AuditScanStart { get; private set; }
        #endregion

        #region Methods
        private void UpdateGames(string obj)
        {

            if (GamesList != null)
            {
                try
                {
                    _gameRepo.GetGames(_settingsRepo.HypermintSettings.HsPath + @"\Databases\" + obj + "\\" + obj + ".xml", obj);

                    GamesList = new ListCollectionView(_gameRepo.GamesList);
                }
                catch (Exception exception)
                {
                    exception.GetBaseException();
                    var msg = exception.Message;
                }
                finally
                {

                }
            }

        }

        private void AuditRunScan()
        {
            Hs.Hypermint.Services.Auditer.ScanForMedia("Amstrad CPC",_gameRepo.GamesList);
            GamesList = new ListCollectionView(_gameRepo.GamesList);
        }
        #endregion
                       
        private void GamesList_CurrentChanged(object sender, EventArgs e)
        {
            Game game = GamesList.CurrentItem as Game;
            if (game != null)
            {                
                this._eventAggregator.GetEvent<GameSelectedEvent>().Publish(game.Description);
            }            
        }
       
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }
    }
}
