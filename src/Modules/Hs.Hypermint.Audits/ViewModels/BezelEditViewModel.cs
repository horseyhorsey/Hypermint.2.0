using Hs.Hypermint.BezelEdit.Views;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Threading.Tasks;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class BezelEditViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManage;

        public DelegateCommand CloseBezelEditCommand { get; private set; } 

        public BezelEditViewModel(IEventAggregator eventAggregator, IRegionManager regionManage)
        {
            _eventAggregator = eventAggregator;
            _regionManage = regionManage;

            CloseBezelEditCommand = new DelegateCommand(() =>
            {
                _regionManage.RequestNavigate(RegionNames.ContentRegion, "");
            });

        }

    }
}
