using Hs.Hypermint.BezelEdit.Views;
using Hypermint.Base;
using Hypermint.Base.Base;
using MahApps.Metro.Controls.Dialogs;
using Prism.Events;
using System.Threading.Tasks;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class BezelEditViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private Task _dialogService;
        private CustomDialog customDialog;

        public BezelEditViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<OpenBezelEditEvent>().Subscribe(x => ShowBezelEdit(x));

        }

        private void ShowBezelEdit(string fileName)
        {
            
        }


    }
}
