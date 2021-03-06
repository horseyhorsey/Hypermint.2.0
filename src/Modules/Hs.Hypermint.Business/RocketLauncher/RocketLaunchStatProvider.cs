﻿using Frontends.Models.Hyperspin;
using Frontends.Models.Interfaces;
using Frontends.Models.RocketLauncher.Stats;
using Horsesoft.Frontends.Helper.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hs.Hypermint.Business.RocketLauncher
{
    public class RocketLaunchStatProvider : IRocketLaunchStatProvider
    {
        private IRocketStats _rocketStats;

        private IFrontend _frontend;

        #region Constructor
        public RocketLaunchStatProvider()
        {
            SystemGameStats = new List<GameStat>();
            GlobalStats = new GlobalStats();
        }
        #endregion

        #region Properties
        public IEnumerable<GameStat> SystemGameStats { get; set; }

        /// <summary>
        /// Gets or sets the global stats.
        /// </summary>
        public GlobalStats GlobalStats { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets up the provider from a frontend interface
        /// </summary>
        /// <param name="frontend">The frontend.</param>
        public void SetUp(string rlPath)
        {
            _rocketStats = new RocketStats(rlPath);
        }

        /// <summary>
        /// Determines whether [is provider set up].
        /// </summary>
        public bool IsProviderSetUp()
        {
            return _rocketStats == null ? false : true;
        }

        /// <summary>
        /// Gets the game stat asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns></returns>
        public GameStat GetRlGameStatAsync(Game game)
        {
            return _rocketStats.GetRlStats(game);
        }

        /// <summary>
        /// Populates the system stats list.
        /// </summary>
        /// <param name="mainMenuItem">The main menu item.</param>
        public Task PopulateSystemStatsAsync(MainMenu mainMenuItem)
        {
            return Task.Run(async () =>
            {
                SystemGameStats = await _rocketStats.GetRlStatsAsync(mainMenuItem);                
            });            
        }

        /// <summary>
        /// Gets the global stats asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task GetGlobalStatsAsync()
        {
            GlobalStats = await _rocketStats.GetGlobalStatsAsync();
        }

        #endregion

    }
}