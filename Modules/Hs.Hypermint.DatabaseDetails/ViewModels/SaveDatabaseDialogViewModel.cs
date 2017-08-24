using Hypermint.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class SaveDatabaseDialogViewModel : ViewModelBase
    {
        #region Commands        
        public ICommand SaveXmlCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        #endregion

        #region Fields        
        private ISettingsHypermint _settingsRepo;
        private ISelectedService _selectedService;
        private IEventAggregator _eventAggregator;
        private IDialogCoordinator _dialogService;
        private IHyperspinManager _hyperspinManager;
        #endregion

        private CustomDialog customDialog;

        #region Constructors
        public SaveDatabaseDialogViewModel(ISettingsHypermint settingsRepo, IHyperspinManager hyperspinManager,
            IHyperspinXmlDataProvider dataProvider, ISelectedService selectedService, IDialogCoordinator dialogService,
            IEventAggregator ea)
        {
            _settingsRepo = settingsRepo;
            _selectedService = selectedService;
            _eventAggregator = ea;
            _dialogService = dialogService;
            _hyperspinManager = hyperspinManager;

            SaveOptions = new SaveOptions();

            //Commands
            SaveXmlCommand = new DelegateCommand(async () =>
            {
                await SaveDatabaseConfirmAsync(SaveOptions.DbName);
            });

            CloseCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, customDialog);
            });

            //Events
            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(sysName =>
            {
                SaveOptions.DbName = sysName;
            });

            _eventAggregator.GetEvent<SystemDatabaseChanged>().Subscribe(sysName =>
            {
                SaveOptions.DbName = sysName;
            });
        }

        public SaveDatabaseDialogViewModel()
        {

        }
        #endregion

        #region Properties

        private SaveOptions _saveOptions;
        public SaveOptions SaveOptions
        {
            get { return _saveOptions; }
            set { SetProperty(ref _saveOptions, value); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the custom dialog.
        /// </summary>
        /// <param name="cDialog">The c dialog.</param>
        public void SetCustomDialog(CustomDialog cDialog) => customDialog = cDialog;

        #endregion

        #region Support Methods

        /// <summary>
        /// Asks to save database asynchronously.
        /// </summary>
        /// <param name="x">The x.</param>
        private async Task SaveDatabaseConfirmAsync(string x)
        {
            var mahSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Save",
                NegativeButtonText = "Cancel",
            };

            string saveInfoText = "Save ";
            if (SaveOptions.SaveToDatabase) saveInfoText += "|Database";
            if (SaveOptions.SaveFavoritesText) saveInfoText += "|Favorites text";
            if (SaveOptions.SaveFavoritesXml) saveInfoText += "|Favorites xml";
            if (SaveOptions.SaveGenres) saveInfoText += "|Genres";

            var result = await _dialogService
                .ShowMessageAsync(this, saveInfoText, "Do you want to save? ", MessageDialogStyle.AffirmativeAndNegative, mahSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                try
                {
                    await SaveDatabasesAndFavoritesAsync(x);
                }
                catch (Exception ex) { }
            }
                
        }

        /// <summary>
        /// Saves the databases and favorites asynchronous.
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Failed saving database</exception>
        private async Task SaveDatabasesAndFavoritesAsync(string dbName)
        {
            //Create a dialog that shows the progress of this saving...
            //await _dialogService.HideMetroDialogAsync(this, customDialog);
            var progressResult = await _dialogService.ShowProgressAsync(this, "Saving...", "");
            progressResult.SetIndeterminate();

            //Set the path and current system
            var hsPath = _settingsRepo.HypermintSettings.HsPath;
            var system = _selectedService.CurrentSystem;

            //Save the games to xml
            if (SaveOptions.SaveToDatabase)
            {
                await SaveXmlAsync(dbName, progressResult, system);
            }

            //Save genres
            if (SaveOptions.SaveGenres)
            {
                await SaveGenreXmls(progressResult, system);
            }            

            if (SaveOptions.SaveFavoritesText || SaveOptions.SaveFavoritesXml)
            {
                await SaveFavoritesAsync(progressResult, system);
            }

            //Close all and return
            await progressResult.CloseAsync();
            await _dialogService.HideMetroDialogAsync(this, customDialog);

            //_eventAggregator.GetEvent<ErrorMessageEvent>().Publish(e.TargetSite + " : " + e.Message); 

        }

        /// <summary>
        /// Saves the genre XMLS.
        /// </summary>
        /// <param name="progressResult">The progress result.</param>
        /// <param name="system">The system.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Failed saving genre databases</exception>
        private async Task SaveGenreXmls(ProgressDialogController progressResult, string system)
        {
            progressResult.SetMessage("Saving genres..");

            if (!await _hyperspinManager.SaveCurrentGamesListToGenreXmlsAsync(system))
                throw new Exception("Failed saving genre databases");

            progressResult.SetMessage("Genre databases saved..");
        }

        /// <summary>
        /// Saves the XML asynchronous.
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="progressResult">The progress result.</param>
        /// <param name="system">The system.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Failed saving database</exception>
        private async Task SaveXmlAsync(string dbName, ProgressDialogController progressResult, string system)
        {
            progressResult.SetMessage("Saving Database");

            if (!await _hyperspinManager.SaveCurrentGamesListToXmlAsync(system, dbName))
                throw new Exception("Failed saving database");

            progressResult.SetMessage(dbName + " Database saved.");
        }

        /// <summary>
        /// Saves the favorites to text asynchronous.
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="progressResult">The progress result.</param>
        /// <param name="system">The system.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Failed saving database</exception>
        private async Task SaveFavoritesAsync(ProgressDialogController progressResult, string system)
        {
            progressResult.SetMessage("Saving Favorites");

            if (!await _hyperspinManager.SaveFavorites(system))
                ;

            progressResult.SetMessage(system + " favorites saved.");
        }

        #endregion

    }
}
