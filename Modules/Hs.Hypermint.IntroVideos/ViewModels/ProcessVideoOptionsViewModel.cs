using Hypermint.Base;
using Hypermint.Base.Events;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class ProcessVideoOptionsViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        #region Commands
        public ICommand RemoveSelectedCommand { get; set; }
        public ICommand ClearListCommand { get; set; }
        public ICommand ScanFormatCommand { get; set; }        
        #endregion

        public ProcessVideoOptionsViewModel(IEventAggregator ea)
        {
            _eventAggregator = ea;

            //Removes from list and adds to available
            RemoveSelectedCommand = new DelegateCommand(() =>
                    _eventAggregator.GetEvent<RemoveSelectedProcessVideosEvent>().Publish());

            //Clears the list
            ClearListCommand = new DelegateCommand(() =>
                    _eventAggregator.GetEvent<ClearProcessVideosEvent>().Publish());

            //Scan the videos in the process list for resolution and framerate
            ScanFormatCommand = new DelegateCommand(() =>
                _eventAggregator.GetEvent<ScanVideoFormatEvent>().Publish());            
        }
    }
}
