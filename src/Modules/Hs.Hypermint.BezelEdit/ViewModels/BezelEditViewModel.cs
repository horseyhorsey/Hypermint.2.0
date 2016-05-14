using Hs.Hypermint.BezelEdit.Views;
using Hypermint.Base;
using Hypermint.Base.Base;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hs.Hypermint.BezelEdit.ViewModels
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

        private async Task RunCustomDialog()
        {
            customDialog = new CustomDialog() { Title = "" };

            customDialog.Content = new BezelEditView { DataContext = this };

            await _dialogService.ShowMetroDialogAsync(this, customDialog);

        }
    }
}
