using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Events;

namespace Hs.Hypermint.DatabaseDetails
{
    public class NavBarViewModel : ViewModelBase
    {
        private string message = "Test Message";
        private IEventAggregator _eventAggregator;

        public string Message
        {
            get { return message; }
            set {
                 SetProperty(ref message, value);
            }
        }

        public NavBarViewModel(IEventAggregator eventAggregator)
        {           
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<GameSelectedEvent>().Subscribe(SetMessage);
            //this._eventAggregator.GetEvent<GameSelectedEvent>().Publish(game.Description);
        }

        private void SetMessage(string obj)
        {
            Message = obj;
        }
    }
}
