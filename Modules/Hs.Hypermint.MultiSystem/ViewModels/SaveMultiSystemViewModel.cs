using Hypermint.Base.Base;
using System;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.IO;
using Hypermint.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Hs.Hypermint.DatabaseDetails.Services;
using Hs.Hypermint.MultiSystem.Services;
using Hypermint.Base.Services;
using Frontends.Models.Hyperspin;

namespace Hs.Hypermint.MultiSystem.ViewModels
{
    public class SaveMultiSystemViewModel : ViewModelBase
    {
        #region Constructors
        public SaveMultiSystemViewModel(IDialogCoordinator dialogService, CustomDialog customDialog, IEventAggregator ea,
            ISettingsRepo settingsService, IMultiSystemRepo multiSystem, IHyperspinXmlService xmlService, IMainMenuRepo mainMenuRepo, IFileFolderService fileService)
        {
            _dialogService = dialogService;
            _customDialog = customDialog;
            _eventAggregator = ea;
            _settingsService = settingsService;
            _multiSystemRepo = multiSystem;
            _xmlService = xmlService;
            _mainmenuRepo = mainMenuRepo;
            _fileFolderService = fileService;


            CloseCommand = new DelegateCommand(async () =>
            {
                await _dialogService.HideMetroDialogAsync(this, _customDialog);
            });

            SelectSettingsCommand = new DelegateCommand(SelectSettings);

            BuildMultiSystemCommand = new DelegateCommand(BuildMultiSystem);
        }

        #endregion

        #region Fields        

        private IDialogCoordinator _dialogService;
        private CustomDialog _customDialog;
        private IEventAggregator _eventAggregator;
        private ISettingsRepo _settingsService;
        private IMultiSystemRepo _multiSystemRepo;
        private IHyperspinXmlService _xmlService;
        private IMainMenuRepo _mainmenuRepo;
        private IFileFolderService _fileFolderService;

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
        #endregion

        #region Commands

        /// <summary>
        /// Select an existing Hyperspin settings file.
        /// </summary>
        public DelegateCommand SelectSettingsCommand { get; private set; }

        /// <summary>
        /// Closes the save dialog window.
        /// </summary>
        public DelegateCommand CloseCommand { get; private set; }

        public DelegateCommand BuildMultiSystemCommand { get; private set; }

        #endregion

        #region Support Methods

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

        private void SelectSettings()
        {
            var hsPath = _settingsService.HypermintSettings.HsPath;
            SettingsTemplate = _fileFolderService.SetFileDialog(hsPath);
        }

        /// <summary>
        /// Builds a database from the list.
        /// Creates all media folders and options to create symbolic links rather than duplicate media
        /// </summary>
        private async void BuildMultiSystem()
        {
            //See if system already exits in database
            bool nameExists = _mainmenuRepo.Systems.Any(x => x.Name == MultiSystemName);

            //Button settings for mahapps
            var mahSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ok",
                NegativeButtonText = "Cancel",
            };

            MessageDialogResult result;

            //If system is existing ask user to continue
            if (nameExists)
            {
                result = await _dialogService.ShowMessageAsync(this, "System exists", "Do you want to overwrite? ",
                MessageDialogStyle.AffirmativeAndNegative, mahSettings);
                if (result != MessageDialogResult.Affirmative) return;
            }

            //Ask user if wants to save, back out if not.
            mahSettings.AffirmativeButtonText = "Save";
            result = await _dialogService.ShowMessageAsync(this, "Save system", "Do you want to save? ",
                MessageDialogStyle.AffirmativeAndNegative, mahSettings);
            if (result != MessageDialogResult.Affirmative) return;

            var progressResult = await _dialogService.ShowProgressAsync(this, "Saving...", "");
            progressResult.SetIndeterminate();

            if (!CanBuildSystem())
            {
                progressResult.SetMessage("Cancelled");
                return;
            };            

            //Get the Hs directory and combine with the new system name and create the folder.
            var hsPath = _settingsService.HypermintSettings.HsPath;
            var systemDbPath = Path.Combine(hsPath, Root.Databases, MultiSystemName);
            if (!Directory.Exists(systemDbPath)) Directory.CreateDirectory(systemDbPath);

            //Create the multisystem XML
            try
            {
                progressResult.SetMessage("Generating Hyperspin Xml");
                _xmlService
                    .SerializeHyperspinXml(_multiSystemRepo.MultiSystemList, MultiSystemName, hsPath, "", true);
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish("Saving xml: " + e.Message);
                progressResult.SetMessage("Error generating Xml");
                return;
            }

            // Generate the genre database for this new system
            if (CreateGenres)
            {
                progressResult.SetMessage($"Creating genre xmls");
                try
                {
                    _xmlService.SerializeGenreXml(_multiSystemRepo.MultiSystemList, MultiSystemName, hsPath);
                }
                catch (Exception e) { _eventAggregator.GetEvent<ErrorMessageEvent>().Publish("Saving Genres: " + e.Message); };

            }

            progressResult.SetMessage($"Saving settings");
            await Task.Delay(1000);
            // Copy the settings template
            if (File.Exists(SettingsTemplate))
            {
                var settingsIniPath = Path.Combine(hsPath, Root.Settings, MultiSystemName + ".ini");

                if (File.Exists(settingsIniPath))
                    File.Delete(settingsIniPath);

                File.Copy(settingsTemplate, settingsIniPath);
            }

            //Generate all folders needed for hyperspin
            CreateMediaDirectorysForNewSystem(hsPath);

            if (CopyMedia)
                GenerateMediaItems(hsPath);

            if (CreateRomMap)
            {
                var rlPath = _settingsService.HypermintSettings.RlPath;
                if (!Directory.Exists(rlPath)) { return; }
                var iniPath = rlPath + "\\Settings\\" + MultiSystemName;

                progressResult.SetMessage($"Rom mapping at {iniPath}");
                await Task.Delay(1000);

                try
                {
                    if (!Directory.Exists(iniPath))
                        Directory.CreateDirectory(iniPath);

                    RocketlaunchRomMap.CreateGamesIni(
                            _multiSystemRepo.MultiSystemList, iniPath);
                }
                catch (Exception)
                {

                }

            }

            // Add the new system to the main menu if it doesn't already exist and save.            
            MainMenu newMenuItem = new MainMenu(MultiSystemName, 1);
            if (!nameExists)
            {
                progressResult.SetMessage($"Adding system {MultiSystemName}");
                await Task.Delay(1000);
                _mainmenuRepo.Systems.Add(newMenuItem);
                _xmlService.SerializeMainMenuXml(_mainmenuRepo.Systems, hsPath);
            }            

            await progressResult.CloseAsync();

            await _dialogService.HideMetroDialogAsync(this, _customDialog);
        }
        private bool CanBuildSystem()
        {
            if (!string.IsNullOrEmpty(MultiSystemName))
            {
                if (_multiSystemRepo.MultiSystemList.Count > 0)
                {
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }

        }
        private void CreateMediaDirectorysForNewSystem(string hsPath)
        {
            var newSystemMediaPath = Path.Combine(hsPath, Root.Media, MultiSystemName);

            CreateDefaultHyperspinFolders(newSystemMediaPath);
        }
        private static void CreateDefaultHyperspinFolders(string hyperSpinSystemMediaDirectory)
        {
            for (int i = 1; i < 5; i++)
            {
                Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\Images\\Artwork" + i);
            }
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Backgrounds);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.GenreBackgrounds);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.GenreWheel);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Letters);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Pointer);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Special);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Wheels);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Root.Themes);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.BackgroundMusic);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.SystemExit);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.SystemStart);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.WheelSounds);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Root.Video);
        }
        private void GenerateMediaItems(string hsPath)
        {
            foreach (var game in _multiSystemRepo.MultiSystemList)
            {
                ProgressMessage = $"Media for {game.Description}";
                CopyArtworks(ref hsPath, game);
                CopyThemes(ref hsPath, game);
                CopyVideos(ref hsPath, game);
                CopyWheels(ref hsPath, game);
            }
        }
        private void CopyVideos(ref string hsPath, Game game)
        {
            var FileToLink = Path.Combine(hsPath, Root.Media, game.System, Root.Video, game.RomName + ".mp4");
            var tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, Root.Video, game.RomName + ".mp4");

            if (!File.Exists(tempSymlinkFile))
            {
                if (File.Exists(FileToLink))
                {
                    SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
                    return;
                }
            }

            FileToLink = Path.Combine(hsPath, Root.Media, game.System, Root.Video, game.RomName + ".flv");
            tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, Root.Video, game.RomName + ".flv");

            if (File.Exists(FileToLink))
            {
                if (CreateSymbolicLinks)
                    SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
                else
                    File.Copy(FileToLink, tempSymlinkFile, true);
            }
        }
        private void CopyArtworks(ref string hsPath, Game game)
        {
            for (int i = 1; i < 5; i++)
            {
                var FileToLink = Path.Combine(hsPath, Root.Media, game.System, "Images\\Artwork" + i, game.RomName + ".png");
                var tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, "Images\\Artwork" + i, game.RomName + ".png");

                if (!File.Exists(tempSymlinkFile))
                {
                    if (File.Exists(FileToLink))
                    {
                        if (CreateSymbolicLinks)
                            SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
                        else
                            File.Copy(FileToLink, tempSymlinkFile, true);
                    }
                }
            }
        }
        private void CopyWheels(ref string hsPath, Game game)
        {
            var FileToLink = Path.Combine(hsPath, Root.Media, game.System, Images.Wheels, game.RomName + ".png");
            var tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, Images.Wheels, game.RomName + ".png");

            if (File.Exists(FileToLink))
            {
                if (CreateSymbolicLinks)
                    SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
                else
                    File.Copy(FileToLink, tempSymlinkFile, true);
            }
        }
        private void CopyThemes(ref string hsPath, Game game)
        {
            var FileToLink = Path.Combine(hsPath, Root.Media, game.System, Root.Themes, game.RomName + ".zip");
            var tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, Root.Themes, game.RomName + ".zip");

            if (DefaultTheme)
            {
                if (!File.Exists(tempSymlinkFile))
                {
                    if (!File.Exists(FileToLink))
                        FileToLink = Path.Combine(hsPath, Root.Media, game.System, Root.Themes, "default.zip");
                }
            }

            if (CreateSymbolicLinks)
                SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
            else
                File.Copy(FileToLink, tempSymlinkFile, true);
        }

        #endregion


    }
}
