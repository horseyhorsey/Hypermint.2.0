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
        // Temp paths
        private string systemXml = @"I:\HyperSpin\Databases\Amstrad CPC\Amstrad CPC.xml";
        private string systemName = @"Amstrad CPC";

        #region Constructors
        public DatabaseDetailsViewModel(IGameRepo gameRepo, IEventAggregator eventAggregator)
        {
            if (gameRepo == null) throw new ArgumentNullException("gameRepo");
            //_getGamesCommand = new Commands.GetGamesCommand();
            _gameRepo = gameRepo;
            _gameRepo.GetGames(systemXml, systemName);
                        
            GamesList = new ListCollectionView(_gameRepo.GamesList);
            
            _eventAggregator = eventAggregator;
            GamesList.CurrentChanged += GamesList_CurrentChanged;

            AuditScanStart = new DelegateCommand(AuditRunScan);

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(UpdateGames);
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
        public DelegateCommand SaveDb { get; set; }
        public DelegateCommand AuditScanStart { get; private set; }
        #endregion

        #region Methods
        private void UpdateGames(string obj)
        {
            if (GamesList != null)
                try
                {
                    GamesList.CurrentChanged -= GamesList_CurrentChanged;

                    _gameRepo.GetGames(@"I:\HyperSpin\Databases\" + obj + "\\" + obj + ".xml", obj);

                    GamesList = new ListCollectionView(_gameRepo.GamesList);                    
                }
                catch (Exception exception)
                {
                    exception.GetBaseException();
                    var msg = exception.Message;
                }
                finally
                {
                    GamesList.CurrentChanged += GamesList_CurrentChanged;
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
