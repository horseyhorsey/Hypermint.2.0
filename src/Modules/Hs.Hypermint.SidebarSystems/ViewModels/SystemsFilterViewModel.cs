using Hypermint.Base;
using Prism.Events;
using System.ComponentModel;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    /// <summary>
    /// View model to send a message when filtering games
    /// </summary>
    /// <seealso cref="Hypermint.Base.Base.ViewModelBase" />
    public class SystemsFilterViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;

        public SystemsFilterViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        private string systemTextFilter;

        /// <summary>
        /// Gets or sets the system text filter.
        /// </summary>
        public string SystemTextFilter
        {
            get { return systemTextFilter; }
            set { SetProperty(ref systemTextFilter, value);}
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            //base.OnPropertyChanged(propertyName);
            if (args.PropertyName == "SystemTextFilter")
            {
                //Publish to SystemsViewModel with SystemFilteredEvent
                _eventAggregator.GetEvent<SystemFilteredEvent>().Publish(SystemTextFilter);
            }
        }
    }
}
