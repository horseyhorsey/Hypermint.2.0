using Frontends.Models.Hyperspin;
using Hs.Hypermint.DatabaseDetails.Dialog;
using Hs.Hypermint.DatabaseDetails.Services;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class SaveDatabaseViewModel : ViewModelBase
    {
        #region Properties

        private CustomDialog customDialog;

        private bool saveToDatabase = true;
        public bool SaveToDatabase
        {
            get { return saveToDatabase; }
            set { SetProperty(ref saveToDatabase, value); }
        }

        private bool saveGenres;
        public bool SaveGenres
        {
            get { return saveGenres; }
            set { SetProperty(ref saveGenres, value); }
        }

        private bool saveFavoritesText = true;
        public bool SaveFavoritesText
        {
            get { return saveFavoritesText; }
            set { SetProperty(ref saveFavoritesText, value); }
        }

        private bool saveFavoritesXml;
        public bool SaveFavoritesXml
        {
            get { return saveFavoritesXml; }
            set { SetProperty(ref saveFavoritesXml, value); }
        }

        private bool addToGenre;
        public bool AddToGenre
        {
            get { return addToGenre; }
            set { SetProperty(ref addToGenre, value); }
        }

        private string dbName;
        public string DbName
        {
            get { return dbName; }
            set { SetProperty(ref dbName, value); }
        }

        #endregion

        #region Commands
        public DelegateCommand<string> OpenSaveOptionsCommand { get; set; }
        public DelegateCommand SaveXmlCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        #endregion

        #region Services
        private IDialogCoordinator _dialogService;
        private ISettingsRepo _settingsRepo;
        private ISelectedService _selectedService;
        private IEventAggregator _eventAggregator;
        private IHyperspinXmlService _xmlService;
        private IGameRepo _gameRepo;
        private IFileFolderChecker _fileFolderChecker;
        #endregion

        #region Constructors
        public SaveDatabaseViewModel(ISettingsRepo settingsRepo,
            IHyperspinXmlService xmlService,
            IGameRepo gameRepo,
            IFileFolderChecker fileCheck,
            IDialogCoordinator dialogService,
            ISelectedService selectedService,
            IEventAggregator ea)
        {
            _settingsRepo = settingsRepo;
            _dialogService = dialogService;
            _selectedService = selectedService;
            _eventAggregator = ea;
            _xmlService = xmlService;
            _gameRepo = gameRepo;
            _fileFolderChecker = fileCheck;

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(sysName =>
            {
                DbName = sysName;
            });

            _eventAggregator.GetEvent<SystemDatabaseChanged>().Subscribe(sysName =>
            {
                DbName = sysName;
            });

            OpenSaveOptionsCommand = new DelegateCommand<string>(async x =>
            {
                if (!Directory.Exists(_settingsRepo.HypermintSettings.HsPath)) return;

                await RunCustomDialog();
            });

            SaveXmlCommand = new DelegateCommand(() =>
            {
                SaveDatabaseConfirmAsync(DbName);
            });

            CloseCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, customDialog);
            });
        }
        #endregion

        #region Methods
        private async Task RunCustomDialog()
        {                        
            customDialog = new CustomDialog() { Title = "" };

            customDialog.Content = new SaveDatabaseDialog { DataContext = this };

            await _dialogService.ShowMetroDialogAsync(this, customDialog);

        }

        private async void SaveDatabaseConfirmAsync(string x)
        {
            var mahSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Save",
                NegativeButtonText = "Cancel",
            };

            string saveInfoText = "Save ";
            if (SaveToDatabase) saveInfoText += "|Database";
            if (SaveFavoritesText) saveInfoText += "|Favorites text";
            if (SaveFavoritesXml) saveInfoText += "|Favorites xml";
            if (SaveGenres) saveInfoText += "|Genres";

            var result = await _dialogService.ShowMessageAsync(this, saveInfoText, "Do you want to save? ",
                MessageDialogStyle.AffirmativeAndNegative, mahSettings);

            if (result == MessageDialogResult.Affirmative)
                SaveDatabasesAsync(x);
        }

        private async void SaveDatabasesAsync(string dbName)
        {
            await _dialogService.HideMetroDialogAsync(this, customDialog);

            var progressResult = await _dialogService.ShowProgressAsync(this, "Saving...", "");
            progressResult.SetIndeterminate();

            var hsPath = _settingsRepo.HypermintSettings.HsPath;
            var system = _selectedService.CurrentSystem;

            if (SaveToDatabase)
            {
                progressResult.SetMessage("Saving Database");

                await SerializeXmlAsync(hsPath, system, dbName);

                progressResult.SetMessage(dbName + " Database saved.");                

            }

            if (SaveGenres)
            {
                progressResult.SetMessage("Saving genres..");

                await SaveGenreDatabases();

                progressResult.SetMessage("Genre databases saved..");
            }

            if (SaveFavoritesText || SaveFavoritesXml)
            {
                var faveGames = new Games();
                var games = _gameRepo.GamesList.Where(m => m.IsFavorite == true);
                foreach (var favorite in games)
                {
                    faveGames.Add(favorite);
                }

                if (SaveFavoritesText)
                {
                    progressResult.SetMessage("Saving favorites text..");

                    await Task.Delay(500);
                    //dbName == "Favorites";                
                    var favesTextFile = Path.Combine( _settingsRepo.HypermintSettings.HsPath,
                    Root.Databases, _selectedService.CurrentSystem,
                    "favorites.txt");

                    if (!_fileFolderChecker.FileExists(favesTextFile))
                    {
                        progressResult.SetMessage("Favorites not found, creating..");

                        await Task.Delay(500);

                        using (var fileToCreate = _fileFolderChecker.CreateFile(favesTextFile))
                        {
                            fileToCreate.Close();
                        };
                    }

                    _xmlService.SaveFavoritesText(faveGames, favesTextFile);

                    progressResult.SetMessage("Saved favorites text..");

                    await Task.Delay(500);
                }

                try
                {
                    progressResult.SetMessage("Saving favorites xml");

                    if (SaveFavoritesXml && dbName != "Favorites")
                    {

                        _xmlService.SerializeHyperspinXml(faveGames, _selectedService.CurrentSystem,
                           _settingsRepo.HypermintSettings.HsPath, "Favorites");
                    }

                    progressResult.SetMessage("Saved favorites to database");
                    
                }
                catch (System.IO.IOException ex)
                {
                    progressResult.SetMessage("Saved failed " + ex.InnerException);

                    await Task.Delay(2000);

                }
                catch (Exception ex)
                {
                    progressResult.SetMessage("Saved failed " + ex.InnerException);

                    await Task.Delay(2000);                    
                }


            }

            await progressResult.CloseAsync();

            //_eventAggregator.GetEvent<ErrorMessageEvent>().Publish(e.TargetSite + " : " + e.Message); 

        }

        private async Task SerializeXmlAsync(string hyperSpinPath, string systemName, string dbName)
        {
            var databasePath = _fileFolderChecker.CombinePath(new string[] {
                hyperSpinPath,Root.Databases, systemName
            });

            var dbXmlFileName = dbName + ".xml";
            var finalPath = _fileFolderChecker.CombinePath(new string[] { databasePath, dbXmlFileName });

            if (!_fileFolderChecker.FileExists(finalPath))
            {
                _fileFolderChecker.CreateDirectory(databasePath);

                using (var file = _fileFolderChecker.CreateFile(finalPath))
                {
                    file.Close();
                }
            }

            await Sleep(2000);

            try
            {
                await _xmlService.SerializeHyperspinXmlAsync(_gameRepo.GamesList,
                    finalPath, databasePath);
            }
            catch (Exception)
            {

            }
            finally { }


        }

        private async Task SaveGenreDatabases()
        {
            await Task.Run(() =>
            {
                _xmlService.SerializeGenreXml(_gameRepo.GamesList,
                       _selectedService.CurrentSystem,
                       _settingsRepo.HypermintSettings.HsPath);
            });
        }

        private async Task Sleep(int time)
        {
            await Task.Run(() => Thread.Sleep(time));
        }

        #endregion

    }
}
