using Frontends.Models.Hyperspin;
using Frontends.Models.RocketLauncher.Stats;
using Horsesoft.Frontends.Helper.Hyperspin;
using Hs.Hypermint.Business.RocketLauncher;
using Hypermint.Base.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Prism.Events;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Hs.Hypermint.RocklaunchStats.ViewModels
{
    public class StatsViewModel : ViewModelBase
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ISettingsRepo _settingsRepo;
        private IRocketLaunchStatProvider _statsRepo;
        #endregion

        #region Constructor
        public StatsViewModel(IRocketLaunchStatProvider statsRepo, IEventAggregator eventAggregator, ISettingsRepo settingsRepo)
        {
            _statsRepo = statsRepo;
            _eventAggregator = eventAggregator;
            _settingsRepo = settingsRepo;

            //Updates the stats when system changed.
            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(async (x) => await UpdateStatsOnSystemChanged(x));
        } 
        #endregion

        #region Properties
        private ICollectionView stats;
        public ICollectionView Stats
        {
            get { return stats; }
            set { SetProperty(ref stats, value); }
        }

        private GlobalStats _globalStats;
        public GlobalStats GlobalStats
        {
            get { return _globalStats; }
            set { SetProperty(ref _globalStats, value); }
        }

        #endregion

        /// <summary>
        /// Updates the stats on system changed.
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        public async Task UpdateStatsOnSystemChanged(string systemName)
        {
            if (!Directory.Exists(_settingsRepo.HypermintSettings.RlPath))
                return;

            try
            {
#warning Needs to be moved out of the view model, set it up in the bootstrapper maybe
                if (!_statsRepo.IsProviderSetUp())
                {
                    _statsRepo.SetUp(new HyperspinFrontend { Path = _settingsRepo.HypermintSettings.RlPath });
                }

                if (systemName.ToLower().Contains("main menu"))
                {
                    await _statsRepo.GetGlobalStatsAsync();

                    GlobalStats = _statsRepo.GlobalStats;
                }
                else
                {
                    await _statsRepo.PopulateSystemStatsAsync(new MainMenu { Name = systemName });

                    if (_statsRepo.SystemGameStats?.Count() > 0)
                    {
                        Stats = new ListCollectionView(_statsRepo.SystemGameStats?.ToList());
                    }
                }
            }
            catch { }
        }
    }
}
