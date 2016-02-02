using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.DatabaseDetails
{
    public class DbToolbarViewModel : ViewModelBase
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

        public DbToolbarViewModel(IEventAggregator eventAggregator)
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
