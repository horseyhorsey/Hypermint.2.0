using Frontends.Models.Hyperspin;
using Frontends.Models.RocketLauncher.Stats;
using Hs.Hypermint.Business.RocketLauncher;
using Hypermint.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Prism.Events;
using System.Collections.ObjectModel;
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
        private ISettingsHypermint _settingsRepo;
        private IHyperspinManager _hyperspinManager;
        private IRocketLaunchStatProvider _statsRepo;
        #endregion

        #region Constructor
        public StatsViewModel(IRocketLaunchStatProvider statsRepo, IEventAggregator eventAggregator, ISettingsHypermint settingsRepo, IHyperspinManager hyperspinManager)
        {
            _statsRepo = statsRepo;
            _eventAggregator = eventAggregator;
            _settingsRepo = settingsRepo;
            _hyperspinManager = hyperspinManager;

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

        private ObservableCollection<GlobalStats> _globalStats;
        public ObservableCollection<GlobalStats> GlobalStats
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
                    _statsRepo.SetUp(_settingsRepo.HypermintSettings.RlPath);
                }

                if (systemName.ToLower().Contains("main menu"))
                {
                    await _statsRepo.GetGlobalStatsAsync();

                    var gStats = _statsRepo.GlobalStats;                    
                    GlobalStats.Clear();
                    GlobalStats.Add(gStats);
                }
                else
                {                    
                    await _statsRepo.PopulateSystemStatsAsync(new MainMenu { Name = systemName });

                    if (_statsRepo.SystemGameStats?.Count() > 0)
                    {
                        Stats = new ListCollectionView(_statsRepo.SystemGameStats?.ToList());
                        var stats = Stats.CurrentItem as GameStat;
                        

                    }
                }
            }
            catch { }
        }
    }
}
