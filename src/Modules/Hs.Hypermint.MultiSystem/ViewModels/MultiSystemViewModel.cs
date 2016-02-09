using Hypermint.Base.Base;

namespace Hs.Hypermint.MultiSystem.ViewModels
{
    public class MultiSystemViewModel : ViewModelBase
    {
        private string message = "Test Message";
        //private IEventAggregator _eventAggregator;

        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }

        public MultiSystemViewModel()
        {
            //_eventAggregator = eventAggregator;

            //_eventAggregator.GetEvent<GameSelectedEvent>().Subscribe(SetMessage);
            //this._eventAggregator.GetEvent<GameSelectedEvent>().Publish(game.Description);
        }

        private void SetMessage(string obj)
        {
            Message = obj;
        }
    }
}
