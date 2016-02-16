using Hs.HyperSpin.Database;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Events;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace Hs.Hypermint.MultiSystem.ViewModels
{
    public class MultiSystemViewModel : ViewModelBase
    {
        private string message = "Test Message";
        private IEventAggregator _eventAggregator;
        private IMultiSystemRepo _multiSystemRepo;

        //private IEventAggregator _eventAggregator;

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

        public MultiSystemViewModel(IEventAggregator ea,IMultiSystemRepo multiSystem)
        {
            _eventAggregator = ea;
            _multiSystemRepo = multiSystem;

            _eventAggregator.GetEvent<AddToMultiSystemEvent>().Subscribe(AddToMultiSystem);
            //this._eventAggregator.GetEvent<GameSelectedEvent>().Publish(game.Description);
        }

        private void AddToMultiSystem(object games)
        {            
            if (_multiSystemRepo.MultiSystemList == null)
            {
                _multiSystemRepo.MultiSystemList = new Games();
                MultiSystemList = new ListCollectionView(_multiSystemRepo.MultiSystemList);
            }                                    

            foreach (var game in (List<Game>)games)
            {
                _multiSystemRepo.MultiSystemList.Add(game);                
            }
            

         }
    }
}
