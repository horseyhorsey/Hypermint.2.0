using Frontends.Models;
using Frontends.Models.Hyperspin;
using Frontends.Models.Interfaces;
using Hypermint.Base.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Hypermint.Base
{
    public interface IHyperspinManager
    {
        #region Collections
        /// <summary>
        /// Gets or sets the systems.
        /// </summary>
        ObservableCollection<MenuItemViewModel> Systems { get; set; }

        /// <summary>
        /// Gets or sets the current systems games.
        /// </summary>
        ObservableCollection<GameItemViewModel> CurrentSystemsGames { get; set; }

        /// <summary>
        /// Gets or sets the current systems games.
        /// </summary>
        ObservableCollection<IFile> DatabasesCurrentSystem { get; set; }

        /// <summary>
        /// Gets or sets the databases current genres.
        /// </summary>
        ObservableCollection<IFile> DatabasesCurrentGenres { get; set; }

        /// <summary>
        /// Gets or sets the multi system games list.
        /// </summary>
        ObservableCollection<GameItemViewModel> MultiSystemGamesList { get; set; }

        #endregion

        IFrontend _hyperspinFrontEnd { get; set; }

        Task AuditMedia(string systemName);

        Task<bool> CreateMultiSystem(MultiSystemOptions options);

        Task<bool> SaveCurrentSystemsListToXmlAsync(string currentMainMenu, bool isMultisystem);

        /// <summary>
        /// Creates a system and all of its needed directories and settings for hyperspin to work.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <param name="existingDb">hyperspin database file</param>
        /// <returns></returns>
        Task<bool> CreateSystem(string systemName, string existingDb = "", string mainmenuName = "");

        /// <summary>
        /// Gets the frontend.
        /// </summary>
        IHyperspinFrontend GetFrontend();

        /// <summary>
        /// Gets the games from all favorites text files found in system database folders.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Game>> GetGamesFromAllFavorites();

        /// <summary>
        /// Gets the genre databases.
        /// </summary>
        /// <param name="system">The system.</param>
        Task<IEnumerable<HyperspinFile>> GetGenreDatabases(string system);

        /// <summary>
        /// Gets the system databases.
        /// </summary>
        /// <param name="system">The system.</param>
        Task GetSystemDatabases(string system);

        /// <summary>
        /// Populates the main menu systems.
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        /// <returns></returns>
        Task PopulateMainMenuSystems(string dbName = "");

        /// <summary>
        /// Sets the games for system.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <param name="dbName">Name of the database.</param>
        Task<IEnumerable<Game>> SetGamesForSystem(string systemName, string dbName = "");

        /// <summary>
        /// Saves the current games list to XML asynchronous.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="dbName">Name of the database.</param>
        Task<bool> SaveCurrentGamesListToXmlAsync(string systemName, string dbName);

        /// <summary>
        /// Saves the current games list to genre XMLS asynchronous.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <returns></returns>
        Task<bool> SaveCurrentGamesListToGenreXmlsAsync(string systemName);

        /// <summary>
        /// Gets the hyperspin media files.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="fileFilter">The file filter.</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetHyperspinMediaFiles(string systemName, string folder, string fileFilter = "*.*");

        /// <summary>
        /// Scans for roms.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <param name="rlPath">The rl path.</param>
        /// <returns></returns>
        Task ScanForRoms(string systemName, string rlPath);

        /// <summary>
        /// Gets the favorites for system.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <returns></returns>
        Task GetFavoritesForSystem(string systemName);

        /// <summary>
        /// Saves the favorites.
        /// </summary>
        /// <param name="system">The system.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <returns></returns>
        Task<bool> SaveFavorites(string system);
    }
}
