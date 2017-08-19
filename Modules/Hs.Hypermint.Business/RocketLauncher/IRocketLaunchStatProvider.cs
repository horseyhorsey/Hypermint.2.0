using Frontends.Models.Hyperspin;
using Frontends.Models.Interfaces;
using Frontends.Models.RocketLauncher.Stats;
using Horsesoft.Frontends.Helper.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hs.Hypermint.Business.RocketLauncher
{
    public interface IRocketLaunchStatProvider
    {
        /// <summary>
        /// Gets or sets the systems game stats.
        /// </summary>
        IEnumerable<GameStat> SystemGameStats { get; set; }

        /// <summary>
        /// Gets or sets the global stats.
        /// </summary>
        GlobalStats GlobalStats { get; set; }

        /// <summary>
        /// Sets up the stats provider from a frontend
        /// </summary>
        /// <param name="frontend">The frontend.</param>
        void SetUp(IFrontend frontend);

        /// <summary>
        /// Gets the rl game stats asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        GameStat GetRlGameStatAsync(Game game);

        /// <summary>
        /// Populates the system stats asynchronous.
        /// </summary>
        /// <param name="mainMenuItem">The main menu item.</param>
        Task PopulateSystemStatsAsync(MainMenu mainMenuItem);

        /// <summary>
        /// Gets the global stats asynchronous.
        /// </summary>
        Task GetGlobalStatsAsync();

        /// <summary>
        /// Determines whether [is provider set up].
        /// </summary>
        bool IsProviderSetUp();
    }
}
