using Hs.Hypermint.DatabaseDetails.Dialog;
using Hypermint.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Services;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabaseControlsViewModel : ViewModelBase
    {
        #region Fields
        private IHyperspinManager _hyperspinManager;
        private IDialogCoordinator _dialogService;
        private IUnityContainer _container;
        private IEventAggregator _eventAgg;
        private ISelectedService _selectedService;
        private CustomDialog customDialog;
        #endregion

        #region Commands        
        public ICommand EnableFaveItemsCommand { get; set; }        
        public ICommand EnableDbItemsCommand { get; set; }
        public ICommand OpenSaveDialogCommand { get; set; }
        #endregion

        #region Constructors

        public DatabaseControlsViewModel(IEventAggregator eventAgg, IUnityContainer container,
            IHyperspinManager hsManager, ISelectedService selectedService, IDialogCoordinator dialogService)
        {
            _hyperspinManager = hsManager;
            _eventAgg = eventAgg;
            _selectedService = selectedService;
            _dialogService = dialogService;
            _container = container;

            EnableFaveItemsCommand = new DelegateCommand<string>(x => UpdateRows(x, RowUpdateType.Favorite));
            EnableDbItemsCommand = new DelegateCommand<string>(x => UpdateRows(x, RowUpdateType.Enabled));

            OpenSaveDialogCommand = new DelegateCommand<string>(async x =>
            {
                await RunCustomDialog();
            });
        } 
        #endregion

        #region Support Methods

        /// <summary>
        /// Runs the custom dialog. (SaveDatabaseDialog <para /> This could be more generic by using an IDialog and passing in.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Couldnt get view model SaveDatabase from contaienr</exception>
        private async Task RunCustomDialog()
        {
            customDialog = new CustomDialog() { Title = "" };

            var ctx = _container.Resolve<SaveDatabaseDialogViewModel>();

            if (ctx == null)
                throw new NullReferenceException("Couldnt get view model - SaveDatabase from container");

            //Give the Vm the customDialog so it can close it.
            ctx.SetCustomDialog(customDialog);
            customDialog.Content = new SaveDatabaseDialogView() { DataContext = ctx };

            await _dialogService.ShowMetroDialogAsync(this, customDialog);

        }

        /// <summary>
        /// Updates the rows.
        /// </summary>
        /// <param name="enabled">The enabled.</param>
        /// <param name="enabled">The enabled.</param>
        private void UpdateRows(string enabled, RowUpdateType rowType)
        {
            object value = null;
            if (rowType == RowUpdateType.Enabled || rowType == RowUpdateType.Favorite)
                value = (object)Convert.ToInt32(enabled);

            try
            {
                var messgae = new UserRequestRowMessage(_selectedService.SelectedGames, rowType, value);

                //Publish after the gameslist is updated here
                _eventAgg.GetEvent<UserRequestUpdateSelectedRows>().Publish(messgae);

            }
            catch (Exception) { throw; }
        } 

        #endregion

    }
}
