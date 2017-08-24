using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class SidebarOptionsViewModel : ViewModelBase
    {
        private IHyperspinManager _hyperSpinManager;
        private IHyperspinXmlDataProvider _dataProvider;
        private IDialogCoordinator _dialogService;
        private IEventAggregator _eventAggregator;
        private ISelectedService _selected;
        private IFileDialogHelper _fileFolderServic;
        private ISettingsHypermint _settings;

        public SidebarOptionsViewModel(IHyperspinManager hyperSpinManager,
            IHyperspinXmlDataProvider dataProvider, ISelectedService selected, ISettingsHypermint settings,
            IEventAggregator ea, IDialogCoordinator dialogService, IFileDialogHelper fileFolderServic)
        {
            _hyperSpinManager = hyperSpinManager;
            _dataProvider = dataProvider;
            _dialogService = dialogService;
            _eventAggregator = ea;
            _selected = selected;
            _fileFolderServic = fileFolderServic;
            _settings = settings;

            AddSystemCommand = new DelegateCommand<string>(async x =>
            {
                await OnAddSystem();
            });

            SaveMainMenuCommand = new DelegateCommand(async () => await SaveMainMenu());

        }

        private async Task SaveMainMenu()
        {
            if (_selected.CurrentMainMenu == null) return;

            try
            {
                if (_hyperSpinManager.Systems != null || _hyperSpinManager.Systems.Count > 0)
                    await _hyperSpinManager.SaveCurrentSystemsListToXmlAsync(_selected.CurrentMainMenu, false);
            }
            catch (Exception ex) { }
        }

        #region Commands
        public ICommand AddSystemCommand { get; private set; }        
        public ICommand SaveMainMenuCommand { get; private set; }
        #endregion

        CustomDialog customDialog;

        #region Properties

        private bool reOrderSystems;
        /// <summary>
        /// Gets or sets a value indicating whether [re order systems].
        /// </summary>
        public bool ReOrderSystems
        {
            get { return reOrderSystems; }
            set
            {
                SetProperty(ref reOrderSystems, value);

                _eventAggregator.GetEvent<ReorderingSystemsEvent>().Publish(ReOrderSystems);
            }
        }

        #endregion

        #region Support Methods

        /// <summary>
        /// Opens a custom dialog and adds the system asynchronous 
        /// </summary>
        /// <returns></returns>
        private async Task AddSystemAsync()
        {
            try
            {
                customDialog = new CustomDialog() { Title = "Add new system" };

                customDialog.Content = new Dialog.AddSystemDialog { DataContext = new AddSystemDialogViewModel(_dialogService, customDialog, _hyperSpinManager, _eventAggregator, _fileFolderServic,_settings, _selected) };

                await _dialogService.ShowMetroDialogAsync(this, customDialog);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnAddSystem()
        {
            await AddSystemAsync();
        }

        #endregion
    }
}
