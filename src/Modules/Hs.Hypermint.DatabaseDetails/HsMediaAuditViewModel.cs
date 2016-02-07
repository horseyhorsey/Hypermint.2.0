using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Events;
using System;
using System.ComponentModel;

namespace Hs.Hypermint.DatabaseDetails
{
    public class HsMediaAuditViewModel : ViewModelBase
    {
        private string message = "Test Message";
        private IEventAggregator _eventAggregator;
        private IGameRepo _gameRepo;       

        public string Message
        {
            get { return message; }
            set {
                 SetProperty(ref message, value);
            }
        }

        private ICollectionView _gamesList;
        public ICollectionView GamesList
        {
            get { return _gamesList; }
            set { _gamesList = value; }
        }


        public HsMediaAuditViewModel( IGameRepo gameRepo, IEventAggregator eventAggregator)
        {           
            _eventAggregator = eventAggregator;
            _gameRepo = gameRepo;
       
            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(UpdateGames);

        }

        private void GamesList_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void AuditRunScan()
        {
            throw new NotImplementedException();
        }



        private void UpdateGames(string obj)
        {
          //  if (_gameRepo.GamesList != null && GamesList !=null)
           //     GamesList = new ListCollectionView(_gameRepo.GamesList);

        }

        private void SetMessage(string obj)
        {
            Message = obj;
        }
    }
}
