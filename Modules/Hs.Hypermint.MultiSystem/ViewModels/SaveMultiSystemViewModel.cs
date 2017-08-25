using Hypermint.Base;
using System;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using Hypermint.Base.Interfaces;
using Frontends.Models.Hyperspin;
using System.Windows.Input;
using Hypermint.Base.Services;

namespace Hs.Hypermint.MultiSystem.ViewModels
{
    public class SaveMultiSystemViewModel : ViewModelBase
    {
        #region Fields        
        private IDialogCoordinator _dialogService;
        private IEventAggregator _eventAggregator;
        private ISettingsHypermint _settingsService;
        private IFileDialogHelper _fileFolderService;
        private IHyperspinManager _hyperspinManager;
        #endregion

        private CustomDialog _customDialog;

        #region Constructors
        public SaveMultiSystemViewModel(IDialogCoordinator dialogService, CustomDialog customDialog, IEventAggregator ea,
            ISettingsHypermint settingsService, IHyperspinManager hyperspinManager, IFileDialogHelper fileService, ISelectedService selected)
        {
            _dialogService = dialogService;
            _customDialog = customDialog;
            _eventAggregator = ea;
            _settingsService = settingsService;
            _fileFolderService = fileService;
            _hyperspinManager = hyperspinManager;

            CloseCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, _customDialog);
            });

            SelectSettingsCommand = new DelegateCommand(SelectSettings);

            BuildMultiSystemCommand = new DelegateCommand(async () =>
            {
                try
                {
                    if (CheckValid())
                    {
                        await BuildMultiSystemAsync();

                        await _hyperspinManager.SaveCurrentSystemsListToXmlAsync(selected.CurrentMainMenu, true);

                        await _dialogService.HideMetroDialogAsync(this, _customDialog);
                    }
                }
                catch (Exception ex) { await _dialogService.HideMetroDialogAsync(this, _customDialog); }
            }
            );
        }

        #endregion

        #region Properties
        private string multiSystemName;
        public string MultiSystemName
        {
            get { return multiSystemName; }
            set { SetProperty(ref multiSystemName, value); }
        }

        private string settingsTemplate;
        public string SettingsTemplate
        {
            get { return settingsTemplate; }
            set { SetProperty(ref settingsTemplate, value); }
        }

        private bool createGenres;
        public bool CreateGenres
        {
            get { return createGenres; }
            set { SetProperty(ref createGenres, value); }
        }

        private bool createRomMap;
        public bool CreateRomMap
        {
            get { return createRomMap; }
            set { SetProperty(ref createRomMap, value); }
        }

        private bool defaultTheme;
        public bool DefaultTheme
        {
            get { return defaultTheme; }
            set { SetProperty(ref defaultTheme, value); }
        }

        private bool copyMedia;
        public bool CopyMedia
        {
            get { return copyMedia; }
            set { SetProperty(ref copyMedia, value); }
        }

        private string progressMessage;
        public string ProgressMessage
        {
            get { return progressMessage; }
            set { SetProperty(ref progressMessage, value); }
        }

        private bool createSymbolicLinks = true;
        public bool CreateSymbolicLinks
        {
            get { return createSymbolicLinks; }
            set { SetProperty(ref createSymbolicLinks, value); }
        }

        private MultiSystemOptions _multiSystemOptions;
        public MultiSystemOptions MultiSystemOptions
        {
            get { return _multiSystemOptions; }
            set { SetProperty(ref _multiSystemOptions, value); }
        }
        #endregion

        #region Commands

        /// <summary>
        /// Select an existing Hyperspin settings file.
        /// </summary>
        public ICommand SelectSettingsCommand { get; private set; }

        /// <summary>
        /// Closes the save dialog window.
        /// </summary>
        public ICommand CloseCommand { get; private set; }

        public ICommand BuildMultiSystemCommand { get; private set; }

        #endregion

        #region Support Methods

        /// <summary>
        /// Builds the multi system list to hyperspin asynchronously.
        /// </summary>
        /// <returns></returns>
        private Task<bool> BuildMultiSystemAsync()
        {
            MultiSystemOptions = new MultiSystemOptions
            {
                CopyMedia = CopyMedia,
                CreateGenres = CreateGenres,
                CreateRomMap = CreateRomMap,
                CreateSymbolicLinks = CreateSymbolicLinks,
                MultiSystemName = MultiSystemName,
                SettingsTemplateFile = SettingsTemplate
            };

            return _hyperspinManager.CreateMultiSystem(MultiSystemOptions);
        }

        private bool CheckValid()
        {
            if (string.IsNullOrWhiteSpace(MultiSystemName))
                return false;

            if (_hyperspinManager.MultiSystemGamesList?.Count <= 0)
            {
                return false;
            }

            return true;
        }

        private async void SaveDatabasesAsync(string dbName)
        {
            await _dialogService.HideMetroDialogAsync(this, _customDialog);

            var progressResult = await _dialogService.ShowProgressAsync(this, "Saving...", "");
            progressResult.SetIndeterminate();

            progressResult.SetMessage(dbName + " Database saved.");

            await Task.Delay(2000);

            await progressResult.CloseAsync();

            //_eventAggregator.GetEvent<ErrorMessageEvent>().Publish(e.TargetSite + " : " + e.Message); 
        }

        private async void SaveDatabaseConfirmAsync(string x)
        {
            var mahSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Save",
                NegativeButtonText = "Cancel",
            };

            string saveInfoText = "Save ";

            //if (SaveToDatabase) saveInfoText += "|Database";
            //if (SaveFavoritesText) saveInfoText += "|Favorites text";
            //if (SaveFavoritesXml) saveInfoText += "|Favorites xml";
            //if (SaveGenres) saveInfoText += "|Genres";

            var result = await _dialogService.ShowMessageAsync(this, saveInfoText, "Do you want to save? ",
                MessageDialogStyle.AffirmativeAndNegative, mahSettings);

            //if (result == MessageDialogResult.Affirmative)
            //    SaveDatabasesAsync(x);
        }

        private void SelectSettings()
        {
            var hsPath = _settingsService.HypermintSettings.HsPath;
            SettingsTemplate = _fileFolderService.SetFileDialog(hsPath);
        }

        #endregion
    }
}
