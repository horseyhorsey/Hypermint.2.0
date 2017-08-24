using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class AddSystemDialogViewModel : ViewModelBase
    {
        #region Fields
        private IDialogCoordinator _dialogService;
        private IHyperspinManager _hyperspinManager;
        private CustomDialog _callingDialog;
        private IEventAggregator _eventAggregator;
        private IFileDialogHelper _fileFolderServic;
        private ISettingsHypermint _settings;
        private ISelectedService _service;
        #endregion

        public AddSystemDialogViewModel(IDialogCoordinator dialogService, CustomDialog callingDialog, IHyperspinManager manager,
            IEventAggregator ea, IFileDialogHelper fileFolderServic, ISettingsHypermint settings, ISelectedService service)
        {
            _dialogService = dialogService;
            _hyperspinManager = manager;
            _callingDialog = callingDialog;
            _eventAggregator = ea;
            _fileFolderServic = fileFolderServic;
            _settings = settings;
            _service = service;

            CloseDialogCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, callingDialog);
            });

            SelectDatabaseCommand = new DelegateCommand(() =>
            {
                OnSelectDatabase();
            });

            SaveNewSystemCommand = new DelegateCommand(() =>
            {
                OnSaveNewSystem();
            });
        }        

        #region Commands
        public ICommand CloseDialogCommand { get; private set; }        
        public ICommand SelectDatabaseCommand { get; private set; }
        public ICommand SaveNewSystemCommand { get; private set; }
        #endregion

        #region Properties

        private string pickedDatabaseXml;
        /// <summary>
        /// Gets or sets the picked database XML.
        /// </summary>
        public string PickedDatabaseXml
        {
            get { return pickedDatabaseXml; }
            set { SetProperty(ref pickedDatabaseXml, value); }
        }

        private string shortDbName;
        /// <summary>
        /// Gets or sets the short name of the database.
        /// </summary>
        public string ShortDbName
        {
            get { return shortDbName; }
            set { SetProperty(ref shortDbName, value); }
        }

        private string newSystemName;
        /// <summary>
        /// Gets or sets the new name of the system.
        /// </summary>
        public string NewSystemName
        {
            get { return newSystemName; }
            set { SetProperty(ref newSystemName, value); }
        }

        #endregion

        #region Support Methods

        /// <summary>
        /// Called when [saving new system].
        /// </summary>
        private void OnSaveNewSystem()
        {
            if (string.IsNullOrWhiteSpace(NewSystemName))
            {
                return;
            }

            SaveNewSystem(NewSystemName);
        }

        /// <summary>
        /// Called when [selecting a database file].
        /// </summary>
        private void OnSelectDatabase()
        {
            if (!Directory.Exists(_settings.HypermintSettings.HsPath)) return;

            PickedDatabaseXml = _fileFolderServic.SetFileDialog(_settings.HypermintSettings.HsPath + "\\Databases");

            ShortDbName = Path.GetFileNameWithoutExtension(PickedDatabaseXml);
        }

        /// <summary>
        /// Saves a new system to hyperspin
        /// </summary>
        /// <param name="createFromExistingDb"></param>
        private void SaveNewSystem(string systemName)
        {
            if (systemName.Contains("Main Menu"))
            {
                System.Windows.MessageBox.Show("Can't create systems that contain Main Menu");
                return;
            }

            if (_hyperspinManager.Systems.Any(x => x.Name == systemName))
            {
                //System exists ask to overwrite.
                if (System.Windows.MessageBox.Show("Do you want to overwrite the system?",
                    "System already exists",
                    System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            _hyperspinManager.CreateSystem(systemName, PickedDatabaseXml, _service.CurrentMainMenu);

            _dialogService.HideMetroDialogAsync(this, _callingDialog);
        }

        #endregion
    }
}
