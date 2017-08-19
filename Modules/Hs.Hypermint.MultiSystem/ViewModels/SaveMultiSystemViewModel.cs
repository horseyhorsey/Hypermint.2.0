using Hypermint.Base;
using System;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using Hypermint.Base.Interfaces;
using Hs.Hypermint.DatabaseDetails.Services;
using Frontends.Models.Hyperspin;
using System.Windows.Input;

namespace Hs.Hypermint.MultiSystem.ViewModels
{
    public class SaveMultiSystemViewModel : ViewModelBase
    {
        #region Fields        
        private IDialogCoordinator _dialogService;
        private IEventAggregator _eventAggregator;
        private ISettingsHypermint _settingsService;
        private IMultiSystemRepo _multiSystemRepo;
        private IHyperspinXmlService _xmlService;
        private IMainMenuRepo _mainmenuRepo;
        private IFileDialogHelper _fileFolderService;
        private IHyperspinManager _hyperspinManager;
        #endregion

        private CustomDialog _customDialog;

        #region Constructors
        public SaveMultiSystemViewModel(IDialogCoordinator dialogService, CustomDialog customDialog, IEventAggregator ea,
            ISettingsHypermint settingsService, IMultiSystemRepo multiSystem, IHyperspinManager hyperspinManager,
            IHyperspinXmlService xmlService, IMainMenuRepo mainMenuRepo, IFileDialogHelper fileService)
        {
            _dialogService = dialogService;
            _customDialog = customDialog;
            _eventAggregator = ea;
            _settingsService = settingsService;
            _multiSystemRepo = multiSystem;
            _xmlService = xmlService;
            _mainmenuRepo = mainMenuRepo;
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
                    await BuildMultiSystemAsync();

                    // Add the new system to the main menu if it doesn't already exist and save.            
                    //MainMenu newMenuItem = new MainMenu(MultiSystemName, 1);
                    //if (!nameExists)
                    //{
                    //    progressResult.SetMessage($"Adding system {MultiSystemName}");
                    //    await Task.Delay(1000);
                    //    _mainmenuRepo.Systems.Add(newMenuItem);
                    //    _xmlService.SerializeMainMenuXml(_mainmenuRepo.Systems, hsPath);
                    //}

                    //await progressResult.CloseAsync();                    
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    await _dialogService.HideMetroDialogAsync(this, _customDialog);
                }
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

        /// <summary>
        /// Determines whether this instance [can build system].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can build system]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanBuildSystem()
        {
            if (!string.IsNullOrEmpty(MultiSystemOptions.MultiSystemName))
            {
                if (_hyperspinManager.MultiSystemGamesList.Count > 0)
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

        //private void GenerateMediaItems(string hsPath)
        //{
        //    foreach (var game in _multiSystemRepo.MultiSystemList)
        //    {
        //        ProgressMessage = $"Media for {game.Description}";
        //        CopyArtworks(ref hsPath, game);
        //        CopyThemes(ref hsPath, game);
        //        CopyVideos(ref hsPath, game);
        //        CopyWheels(ref hsPath, game);
        //    }
        //}
        //private void CopyVideos(ref string hsPath, Game game)
        //{
        //    var FileToLink = Path.Combine(hsPath, Root.Media, game.System, Root.Video, game.RomName + ".mp4");
        //    var tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, Root.Video, game.RomName + ".mp4");

        //    if (!File.Exists(tempSymlinkFile))
        //    {
        //        if (File.Exists(FileToLink))
        //        {
        //            SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
        //            return;
        //        }
        //    }

        //    FileToLink = Path.Combine(hsPath, Root.Media, game.System, Root.Video, game.RomName + ".flv");
        //    tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, Root.Video, game.RomName + ".flv");

        //    if (File.Exists(FileToLink))
        //    {
        //        if (CreateSymbolicLinks)
        //            SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
        //        else
        //            File.Copy(FileToLink, tempSymlinkFile, true);
        //    }
        //}
        //private void CopyArtworks(ref string hsPath, Game game)
        //{
        //    for (int i = 1; i < 5; i++)
        //    {
        //        var FileToLink = Path.Combine(hsPath, Root.Media, game.System, "Images\\Artwork" + i, game.RomName + ".png");
        //        var tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, "Images\\Artwork" + i, game.RomName + ".png");

        //        if (!File.Exists(tempSymlinkFile))
        //        {
        //            if (File.Exists(FileToLink))
        //            {
        //                if (CreateSymbolicLinks)
        //                    SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
        //                else
        //                    File.Copy(FileToLink, tempSymlinkFile, true);
        //            }
        //        }
        //    }
        //}
        //private void CopyWheels(ref string hsPath, Game game)
        //{
        //    var FileToLink = Path.Combine(hsPath, Root.Media, game.System, Images.Wheels, game.RomName + ".png");
        //    var tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, Images.Wheels, game.RomName + ".png");

        //    if (File.Exists(FileToLink))
        //    {
        //        if (CreateSymbolicLinks)
        //            SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
        //        else
        //            File.Copy(FileToLink, tempSymlinkFile, true);
        //    }
        //}
        //private void CopyThemes(ref string hsPath, Game game)
        //{
        //    var FileToLink = Path.Combine(hsPath, Root.Media, game.System, Root.Themes, game.RomName + ".zip");
        //    var tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, Root.Themes, game.RomName + ".zip");

        //    if (DefaultTheme)
        //    {
        //        if (!File.Exists(tempSymlinkFile))
        //        {
        //            if (!File.Exists(FileToLink))
        //                FileToLink = Path.Combine(hsPath, Root.Media, game.System, Root.Themes, "default.zip");
        //        }
        //    }

        //    if (CreateSymbolicLinks)
        //        SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
        //    else
        //        File.Copy(FileToLink, tempSymlinkFile, true);
        //}

        #endregion
    }
}
