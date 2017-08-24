using Frontends.Models;
using Frontends.Models.Hyperspin;
using Frontends.Models.Interfaces;
using Horsesoft.Frontends.Helper.Auditing;
using Horsesoft.Frontends.Helper.Common;
using Horsesoft.Frontends.Helper.Media;
using Horsesoft.Frontends.Helper.Paths.Hyperspin;
using Horsesoft.Frontends.Helper.Serialization;
using Horsesoft.Frontends.Helper.Systems;
using Horsesoft.Frontends.Helper.Tools;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hs.Hypermint.Business.Hyperspin
{
    /// <summary>
    /// Manage games and systems
    /// </summary>
    /// <seealso cref="Prism.Mvvm.BindableBase" />
    /// <seealso cref="Hypermint.Base.IHyperspinManager" />
    public class HyperspinManager : BindableBase, IHyperspinManager
    {
        private ISystemCreator _systemCreator;
        private IHyperspinXmlDataProvider _hsDataProvider;
        private ISettingsHypermint _settingsRepo;
        public IFrontend _hyperspinFrontEnd { get; set; }
        private IHyperspinSerializer _hsSerializer;
        private IHyperspinAudit _auditer;

        public HyperspinManager(ISettingsHypermint settingsRepo, IHyperspinXmlDataProvider hsDataProvider)
        {
            _hsDataProvider = hsDataProvider;
            _settingsRepo = settingsRepo;
            _hyperspinFrontEnd = new HyperspinFrontend
            {
                Path = _settingsRepo.HypermintSettings.HsPath
            };

            //init system stuff
            _systemCreator = new SystemCreator(_hyperspinFrontEnd);
            Systems = new ObservableCollection<MainMenu>();

            //init games lists
            CurrentSystemsGames = new ObservableCollection<GameItemViewModel>();
            MultiSystemGamesList = new ObservableCollection<GameItemViewModel>();

            DatabasesCurrentSystem = new ObservableCollection<IFile>();
            DatabasesCurrentGenres = new ObservableCollection<IFile>();
        }

        #region Properties        

        /// <summary>
        /// Gets or sets the systems collection for the view
        /// </summary>
        public ICollection<MainMenu> SystemsCollection { get; set; }

        private ObservableCollection<MainMenu> systems;
        /// <summary>
        /// Gets or sets the hyperspin systems.
        /// </summary>
        public ObservableCollection<MainMenu> Systems
        {
            get { return systems; }
            set { SetProperty(ref systems, value); }
        }

        private ObservableCollection<GameItemViewModel> _currentSystemsGames;
        public ObservableCollection<GameItemViewModel> CurrentSystemsGames
        {
            get { return _currentSystemsGames; }
            set { SetProperty(ref _currentSystemsGames, value); }
        }

        public ObservableCollection<GameItemViewModel> MultiSystemGamesList { get; set; }

        public ObservableCollection<IFile> DatabasesCurrentSystem { get; set; }
        public ObservableCollection<IFile> DatabasesCurrentGenres { get; set; }

        #endregion

        #region Public Methods

        public async Task AuditMedia(string systemName)
        {
            _auditer = new HyperspinAudit(_hyperspinFrontEnd, new MediaHelperHs(_hyperspinFrontEnd.Path, systemName));

            if (systemName.ToLower().Contains("main menu"))
            {
                await _auditer.ScanMainMenuMediaAsync(CurrentSystemsGames.Select(x => x.Game));
            }
            else
            {
                await _auditer.ScanForMediaAsync(CurrentSystemsGames.Select(x => x.Game));
            }
        }

        /// <summary>
        /// Creates the multi system.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public Task<bool> CreateMultiSystem(MultiSystemOptions options)
        {
            IMediaCopier mc = new MediaCopier(_hyperspinFrontEnd);
            _hsSerializer = new HyperspinSerializer(_hyperspinFrontEnd.Path, options.MultiSystemName, "");
            IMultiSystem ms = new MultiSystem(_hsSerializer, _systemCreator, mc, options);
            var games = MultiSystemGamesList.Select(x => x.Game);

            if (!Systems.Any(x => x.Name == options.MultiSystemName))
                Systems.Add(new MainMenu(options.MultiSystemName, 1));

            return ms.CreateMultiSystem(games, _hyperspinFrontEnd.Path, _settingsRepo.HypermintSettings.RlPath);
        }

        /// <summary>
        /// Creates a system and all of its needed directories and settings for hyperspin to work.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <param name="existingDb">if set to <c>true</c> [existing database].</param>
        /// <returns></returns>
        public async Task<bool> CreateSystem(string systemName, string existingDb = "", string mainmenuName = "")
        {
            if (!await _systemCreator.CreateSystem(systemName, existingDb))
                return false;

            Systems.Add(new MainMenu(systemName, 1));

            _hsSerializer = new HyperspinSerializer(_hyperspinFrontEnd.Path, systemName, mainmenuName);
            await SaveCurrentSystemsListToXmlAsync(mainmenuName, false);

            return true;
        }

        /// <summary>
        /// Gets the frontend interface associated with the manager
        /// </summary>
        public IHyperspinFrontend GetFrontend() => (IHyperspinFrontend)_hyperspinFrontEnd;

        /// <summary>
        /// Gets the main menu databases for hyperspin
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetDatabaseFilesForSystemAsync(string systemName)
        {
            return await _hyperspinFrontEnd.GetDatabaseFilesForSystemAsync(systemName);
        }

        /// <summary>
        /// Gets the games from all favorites text files found in system database folders.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Game>> GetGamesFromAllFavorites()
        {            
            if (_hsSerializer == null)
                _hsSerializer = new HyperspinSerializer(_hyperspinFrontEnd.Path, "Main Menu", "");

            _hsSerializer.ChangeSystemAndDatabase("Main Menu");

            return await _hsSerializer.CreateGamesListFromAllFavoriteTexts(_hyperspinFrontEnd.Path, Systems.AsEnumerable());
        }

        /// <summary>
        /// Gets the genre databases.
        /// </summary>
        /// <param name="system">The system.</param>
        /// <returns></returns>
        public async Task GetGenreDatabases(string system)
        {
            DatabasesCurrentGenres.Clear();

            var genres = await _hsDataProvider.GetAllGenreDatabases(_hyperspinFrontEnd.Path, system);

            foreach (var genre in genres)
            {
                DatabasesCurrentGenres.Add(new HyperspinFile($"{genre}.xml") { });
            }
        }

        /// <summary>
        /// Gets the system databases.
        /// </summary>
        /// <param name="system">The system.</param>
        /// <returns></returns>
        public async Task GetSystemDatabases(string system)
        {
            DatabasesCurrentSystem.Clear();

            var files = await _hyperspinFrontEnd.GetDatabaseFilesForSystemAsync(system);

            foreach (var file in files)
            {
                DatabasesCurrentSystem.Add(new HyperspinFile(file));
            }
        }

        /// <summary>
        /// Populates the main menu entries from a main menu xml
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        /// <returns></returns>
        public async Task PopulateMainMenuSystems(string dbName = "")
        {
            IEnumerable<MainMenu> _systems = null;
            Systems.Clear();
            if (CurrentSystemsGames.Count > 0) CurrentSystemsGames.Clear();

            await Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(dbName))
                {
                    dbName = "Main Menu";
                }

                //Get systems from main menu xml
                _systems = await _hsDataProvider.GetAllSystems(_settingsRepo.HypermintSettings.HsPath, "Main Menu", dbName);
            });

            //Add to system list
            if (_systems?.Count() > 0)
            {
                foreach (var system in _systems)
                {
                    Systems.Add(system);
                }

                await GetSystemIconsAsync();
            }
        }

        /// <summary>
        /// Saves the current games list to XML asynchronous.
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="dbName">Name of the database.</param>
        /// <returns></returns>
        public async Task<bool> SaveCurrentGamesListToXmlAsync(string systemName, string dbName)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    _hsSerializer = new HyperspinSerializer(_hyperspinFrontEnd.Path, systemName, dbName);

                    var games = CurrentSystemsGames.Select(x => x.Game);
                    await _hsSerializer.SerializeAsync(games, false);
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Saves the current games list to genre XMLS asynchronous.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <returns></returns>
        public async Task<bool> SaveCurrentGamesListToGenreXmlsAsync(string systemName)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var games = CurrentSystemsGames.Select(x => x.Game);

                    _hsSerializer = new HyperspinSerializer(_hyperspinFrontEnd.Path, systemName);
                    await _hsSerializer.SerializeGenresAsync(games);

                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Sets the games to the given system.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Game>> SetGamesForSystem(string systemName, string dbName = "")
        {
            CurrentSystemsGames.Clear();

            var games = await _hsDataProvider.GetAllGames(_hyperspinFrontEnd.Path, systemName, dbName);

            //Map the original systems from a games.ini
            if (games != null && games.Count() > 0)
            {                
                var dbPath = Path.Combine(_hyperspinFrontEnd.Path, "Databases", systemName);
                if (Directory.Exists(dbPath + "\\" + "MultiSystem"))
                {
                    var romMapper = new RomMapperRl();
                    var mappedGames = await romMapper.GetGamesFromRocketLaunchGamesIniAsync(dbPath + "\\MultiSystem");
                    foreach (var game in mappedGames)
                    {
                        try
                        {
                            games.FirstOrDefault(x => x.RomName == game.RomName).OriginalSystem = game.System;
                        }
                        catch { }
                    }
                }
            }

            return games;
        }

        public async Task ScanForRoms(string systemName, string rlPath)
        {
            if (!Directory.Exists(rlPath)) return;

            IRomScan romScan = new RomScanner();
            await romScan.ScanRlRomPathsAsync(CurrentSystemsGames.Select(x => x.Game), rlPath, systemName);
        }

        public async Task<IEnumerable<string>> GetHyperspinMediaFiles(string systemName, string folder, string fileFilter ="*.*")
        {
            return await Task.Run(() => PathHelper.GetMediaFilesForGame(_hyperspinFrontEnd.Path, systemName, folder, fileFilter));
        }

        #endregion

        /// <summary>
        /// Gets the system icons asynchronous.
        /// </summary>
        /// <returns></returns>
        private async Task GetSystemIconsAsync()
        {
            await Task.Run(() =>
            {
                var icons = _settingsRepo.HypermintSettings.Icons;

                //Get an icon for each system if exisiting and user has set the folder.
                if (!string.IsNullOrWhiteSpace(icons))
                {
                    if (Directory.Exists(icons))
                    {
                        foreach (var system in Systems)
                        {
                            system.SysIcon = new Uri(Path.Combine(icons, system.Name + ".png"));
                        }
                    }
                }
            });
        }

        public async Task<bool> SaveCurrentSystemsListToXmlAsync(string currentMainMenu, bool isMultiSystem)
        {
            _hsSerializer = new HyperspinSerializer(_hyperspinFrontEnd.Path, "Main Menu", currentMainMenu);

            return await _hsSerializer.SerializeAsync(Systems, isMultiSystem);
        }

        /// <summary>
        /// Gets the favorites for system.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <returns></returns>
        public async Task GetFavoritesForSystem(string systemName)
        {
            try
            {
                if (CurrentSystemsGames.Count > 0)
                {
                    _hsSerializer = new HyperspinSerializer(_hyperspinFrontEnd.Path, systemName);

                    var faves = await _hsSerializer.DeserializeFavoritesAsync();

                    if (faves != null)
                    {
                        foreach (var favorite in faves)
                        {
                            CurrentSystemsGames.FirstOrDefault(x => x.RomName == favorite.RomName).IsFavorite = true;
                        }
                    }
                }
            }
            catch(Exception ex) { }
        }
    }
}
