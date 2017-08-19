using Hypermint.Base;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Windows.Input;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class AddSystemDialogViewModel : ViewModelBase
    {
        private IDialogCoordinator _dialogService;
        private IHyperspinManager _manager;

        public AddSystemDialogViewModel(IDialogCoordinator dialogService, CustomDialog callingDialog, IHyperspinManager manager)
        {
            _dialogService = dialogService;
            _manager = manager;

            SaveMainMenuCommand = new DelegateCommand(SaveMainMenu);

            CloseDialogCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, callingDialog);
            });

            SelectDatabaseCommand = new DelegateCommand(() =>
            {
                //OnSelectDatabase(_fileFolderService);
            });
        }        

        #region Commands

        public ICommand SaveMainMenuCommand { get; private set; }
        public ICommand CloseDialogCommand { get; private set; }
        [Obsolete("Need enabling")]
        public ICommand SelectDatabaseCommand { get; private set; }

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

        [Obsolete("Need implementing")]
        private void OnSelectDatabase()
        {
            //if (!Directory.Exists(_settingsRepo.HypermintSettings.HsPath)) return;

            //PickedDatabaseXml = _fileFolderService
            //    .SetFileDialog(_settingsRepo.HypermintSettings.HsPath + "\\Databases");

            //ShortDbName = Path.GetFileNameWithoutExtension(PickedDatabaseXml);
        }

        [Obsolete("Need implementing")]
        /// <summary>
        /// Saves the main menu.
        /// </summary>
        private void SaveMainMenu()
        {
            //if (_selectedService.CurrentMainMenu == null) return;

            //try
            //{
            //    _eventAggregator.GetEvent<SaveMainMenuEvent>().Publish(_selectedService.CurrentMainMenu);
            //}
            //catch (Exception) { }
        } 

        #endregion
    }
}
