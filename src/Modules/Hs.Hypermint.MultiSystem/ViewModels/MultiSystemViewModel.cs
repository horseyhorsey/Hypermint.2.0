using Hs.HyperSpin.Database;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System;
using Hs.Hypermint.DatabaseDetails.Services;
using System.IO;
using Hypermint.Base.Constants;
using Hs.Hypermint.MultiSystem.Services;
using System.Runtime.CompilerServices;
using Hypermint.Base.Services;

namespace Hs.Hypermint.MultiSystem.ViewModels
{
    public class MultiSystemViewModel : ViewModelBase
    {

        #region Properties
        private string message = "Test Message";
        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }

        private ICollectionView multiSystemList;
        public ICollectionView MultiSystemList
        {
            get { return multiSystemList; }
            set { SetProperty(ref multiSystemList, value); }
        }

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

        private bool createSymbolicLinks;
        public bool CreateSymbolicLinks
        {
            get { return createSymbolicLinks; }
            set { SetProperty(ref createSymbolicLinks, value); }
        }

        #endregion

        #region Constructors
        public MultiSystemViewModel(IEventAggregator ea, IMultiSystemRepo multiSystem, IFileFolderService fileService,
          ISettingsRepo settings, IHyperspinXmlService xmlService, IMainMenuRepo mainMenuRepo, IFavoriteService favoritesService)
        {
            _eventAggregator = ea;
            _multiSystemRepo = multiSystem;
            _fileFolderService = fileService;
            _settingsService = settings;
            _xmlService = xmlService;
            _mainmenuRepo = mainMenuRepo;
            _favoriteService = favoritesService;

            _eventAggregator.GetEvent<AddToMultiSystemEvent>().Subscribe(AddToMultiSystem);

            RemoveGameCommand = new DelegateCommand<Game>(RemoveFromMultisystemList);

            ClearListCommand = new DelegateCommand(() =>
            {
                if (_multiSystemRepo.MultiSystemList != null)
                    _multiSystemRepo.MultiSystemList.Clear();
            });

            SelectSettingsCommand = new DelegateCommand(SelectSettings);

            BuildMultiSystemCommand = new DelegateCommand(BuildMultiSystem);

            ScanFavoritesCommand = new DelegateCommand(ScanFavorites);
        }

        #endregion

        #region Commands
        private IEventAggregator _eventAggregator;
        public DelegateCommand<Game> RemoveGameCommand { get; set; }
        public DelegateCommand ClearListCommand { get; private set; }
        public DelegateCommand SelectSettingsCommand { get; private set; }
        public DelegateCommand BuildMultiSystemCommand { get; private set; }
        public DelegateCommand ScanFavoritesCommand { get; private set; }

        #endregion

        #region Services
        private IFileFolderService _fileFolderService;
        private ISettingsRepo _settingsService;
        private IMultiSystemRepo _multiSystemRepo;
        private IHyperspinXmlService _xmlService;
        private IMainMenuRepo _mainmenuRepo;
        private IFavoriteService _favoriteService;
        #endregion

        #region Methods

        /// <summary>
        /// Scans all favorites.txt in available systems
        /// Needs to go into each databases xml to pull the info for each game 
        /// ****
        /// Why creating two lists..learn how to find if game.romname exists in collection already
        /// </summary>
        private void ScanFavorites()
        {
            if (_multiSystemRepo.MultiSystemList == null)
                _multiSystemRepo.MultiSystemList = new Games();

            var tempGames = new List<Game>();

            try
            {
                foreach (MainMenu system in _mainmenuRepo.Systems)
                {
                    var favoritesList = new List<string>();

                    var hsPath = _settingsService.HypermintSettings.HsPath;

                    var isMultiSystem = File.Exists(Path.Combine(
                        hsPath, Root.Databases, system.Name, "_multisystem")
                        );

                    // Dont want to be scanning the favorites of a multisystem
                    if (system.Name != "Main Menu" && !isMultiSystem)
                    {
                        favoritesList = _favoriteService.GetFavoritesForSystem(system.Name, hsPath);

                        var games = _xmlService.SearchRomStringsListFromXml(favoritesList, system.Name,
                            hsPath);

                        foreach (Game game in games)
                        {
                            if (!tempGames.Exists(x => x.RomName == game.RomName))
                            {
                                tempGames.Add(game);
                                _multiSystemRepo.MultiSystemList.Add(game);
                            }
                        }
                    }
                }

                
            }
            catch {  }
            

            MultiSystemList = new ListCollectionView(_multiSystemRepo.MultiSystemList);
            
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

        private void SelectSettings()
        {
            var hsPath = _settingsService.HypermintSettings.HsPath;
            SettingsTemplate = _fileFolderService.setFileDialog(hsPath);
        }
        /// <summary>
        /// Remove a single item when X is clicked for a game
        /// </summary>
        /// <param name="game"></param>
        private void RemoveFromMultisystemList(Game game)
        {
            _multiSystemRepo.MultiSystemList.Remove(game);
        }
        /// <summary>
        /// Add to a multisystem list from the main database menu event
        /// </summary>
        /// <param name="games"></param>
        private void AddToMultiSystem(object games)
        {
            if (_multiSystemRepo.MultiSystemList == null)
            {
                _multiSystemRepo.MultiSystemList = new Games();
                MultiSystemList = new ListCollectionView(_multiSystemRepo.MultiSystemList);
            }

            foreach (var game in (List<Game>)games)
            {
                //Only add the game if it doesn't already exits
                if (!_multiSystemRepo.MultiSystemList.Contains(game))
                    _multiSystemRepo.MultiSystemList.Add(game);
            }

        }

        /// <summary>
        /// Builds a database from the list.
        /// Creates all media folders and options to create symbolic links rather than duplicate media
        /// </summary>
        private void BuildMultiSystem()
        {
            if (!CanBuildSystem()) return;

            var hsPath = _settingsService.HypermintSettings.HsPath;

            var systemDbPath = Path.Combine(hsPath, Root.Databases, MultiSystemName);

            if (!Directory.Exists(systemDbPath))
                Directory.CreateDirectory(systemDbPath);
            //Create the multisystem XML
            try
            {
                _xmlService.SerializeHyperspinXml(
                _multiSystemRepo.MultiSystemList, MultiSystemName,
                hsPath,"", true);
            }
            catch (Exception e) {
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish("Saving xml: " + e.Message);
                return; }
            

            // Add the new system to the main menu if it doesn't already exist
            // then serialize.
            bool nameExists = false;
            foreach (var item in _mainmenuRepo.Systems)
            {
                if (item.Name == MultiSystemName)
                    nameExists = true;
            }
            var newMenuItem = new MainMenu(MultiSystemName, 1);
            if (!nameExists)
            {
                _mainmenuRepo.Systems.Add(newMenuItem);
                _xmlService.SerializeMainMenuXml(_mainmenuRepo.Systems, hsPath);
            }
            // Generate the genre database for this new system
            if (CreateGenres)
            {
                try
                {
                    _xmlService.SerializeGenreXml(_multiSystemRepo.MultiSystemList, MultiSystemName, hsPath);
                }
                catch (Exception e) { _eventAggregator.GetEvent<ErrorMessageEvent>().Publish("Saving Genres: " + e.Message); };
                
            }

            // Copy the settings template
            if (File.Exists(SettingsTemplate))
            {
                var settingsIniPath = Path.Combine(hsPath, Root.Settings, MultiSystemName + ".ini");

                if (File.Exists(settingsIniPath))
                    File.Delete(settingsIniPath);

                File.Copy(settingsTemplate, settingsIniPath);
            }

            CreateMediaDirectorysForNewSystem(hsPath);

            if (CreateSymbolicLinks)
                GenerateMediaItems(hsPath);

            if (CreateRomMap)
            {
                try
                {
                    var rlPath = _settingsService.HypermintSettings.RlPath;
                    if (!Directory.Exists(rlPath)) { return; }

                    var iniPath = rlPath + "\\Settings\\" + MultiSystemName;

                    if (!Directory.Exists(iniPath))
                        Directory.CreateDirectory(iniPath);

                    RocketlaunchRomMap.CreateGamesIni(
                            _multiSystemRepo.MultiSystemList, iniPath);
                }
                catch (Exception)
                {

                }

            }
        }

        private void GenerateMediaItems(string hsPath)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var game in _multiSystemRepo.MultiSystemList)
            {
                CreateSymbolicArtworks(ref hsPath, game);
                CreateSymbolicTheme(ref hsPath, game);
                CreateSymbolicVideos(ref hsPath, game);
                CreateSymbolicWheels(ref hsPath, game);
            }
        }

        private void CreateSymbolicVideos(ref string hsPath, Game game)
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
                SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
            }
        }

        private void CreateSymbolicArtworks(ref string hsPath, Game game)
        {
            for (int i = 1; i < 5; i++)
            {
                var FileToLink = Path.Combine(hsPath, Root.Media, game.System, "Images\\Artwork" + i, game.RomName + ".png");
                var tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, "Images\\Artwork" + i, game.RomName + ".png");

                if (!File.Exists(tempSymlinkFile))
                {
                    if (File.Exists(FileToLink))
                        SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
                }
            }
        }

        private void CreateSymbolicWheels(ref string hsPath, Game game)
        {
            var FileToLink = Path.Combine(hsPath, Root.Media, game.System, Images.Wheels, game.RomName + ".png");
            var tempSymlinkFile = Path.Combine(hsPath, Root.Media, MultiSystemName, Images.Wheels, game.RomName + ".png");

            if (!File.Exists(tempSymlinkFile))
            {
                if (File.Exists(FileToLink))

                    SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
            }
        }

        private void CreateSymbolicTheme(ref string hsPath, Game game)
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

            SymbolicLinkService.CheckThenCreate(FileToLink, tempSymlinkFile);
        }

        private void CreateMediaDirectorysForNewSystem(string hsPath)
        {
            var newSystemMediaPath = Path.Combine(hsPath, Root.Media, MultiSystemName);

            for (int i = 1; i < 5; i++)
            {
                Directory.CreateDirectory(newSystemMediaPath + "\\Images\\Artwork" + i);
            }

            Directory.CreateDirectory(newSystemMediaPath + "\\" + Images.Backgrounds);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Images.GenreBackgrounds);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Images.GenreWheel);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Images.Letters);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Images.Pointer);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Images.Special);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Images.Wheels);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Root.Themes);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Sound.BackgroundMusic);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Sound.SystemExit);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Sound.SystemStart);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Sound.WheelSounds);
            Directory.CreateDirectory(newSystemMediaPath + "\\" + Root.Video);


        }
        #endregion

    }
}
