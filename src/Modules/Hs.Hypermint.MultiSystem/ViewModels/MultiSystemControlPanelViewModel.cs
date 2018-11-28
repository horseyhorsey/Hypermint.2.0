using Hypermint.Base;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace Hs.Hypermint.MultiSystem.ViewModels
{
    public class MultiSystemControlPanelViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        #region Fields
        private IHyperspinManager _hyperspinManager; 
        #endregion

        #region Commands
        public ICommand ClearListCommand { get; private set; }
        public ICommand BuildMultiSystemCommand { get; private set; }
        public ICommand ScanFavoritesCommand { get; private set; }
        #endregion

        public MultiSystemControlPanelViewModel(IEventAggregator ea, IHyperspinManager hyperspinManager)
        {
            _eventAggregator = ea;
            _hyperspinManager = hyperspinManager;

            ClearListCommand = new DelegateCommand(() =>
            {
                if (_hyperspinManager.MultiSystemGamesList != null)
                    _hyperspinManager.MultiSystemGamesList.Clear();
            });

            BuildMultiSystemCommand = new DelegateCommand(() => _eventAggregator.GetEvent<BuildMultiSystemEvent>().Publish(true));

            ScanFavoritesCommand = new DelegateCommand(() => _eventAggregator.GetEvent<ScanMultiSystemFavoritesEvent>().Publish(true));
        }

        public MultiSystemControlPanelViewModel()
        {

        }
    }
}
