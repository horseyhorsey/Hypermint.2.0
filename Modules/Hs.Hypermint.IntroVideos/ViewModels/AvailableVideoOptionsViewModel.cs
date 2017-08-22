using Hypermint.Base;
using Hypermint.Base.Events;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class AvailableVideoOptionsViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;

        #region Commands
        public ICommand RandomVideoCommand { get; set; }
        public ICommand AddSelectedCommand { get; set; }
        #endregion

        #region Constructors
        public AvailableVideoOptionsViewModel(IEventAggregator eventAggregator)         
        {
            _eventAggregator = eventAggregator;

            RandomVideoCommand = new DelegateCommand(() => 
            {
                _eventAggregator.GetEvent<RandomVideosEvent>().Publish(RandomCount);
            });

            AddSelectedCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<AddSelectedVideosEvent>().Publish();
            });
        }
        #endregion

        #region Properties

        private int _randomCount;
        /// <summary>
        /// Gets or sets the random video count.
        /// </summary>
        public int RandomCount
        {
            get { return _randomCount; }
            set { SetProperty(ref _randomCount, value); }
        }
        #endregion
    }
}
