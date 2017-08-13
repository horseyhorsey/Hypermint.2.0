using Hs.RocketLauncher.Statistics;
using Hypermint.Base.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Hs.Hypermint.RocklaunchStats.ViewModels
{
    public class StatsViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private ISettingsRepo _settingsRepo;
        private IStatsRepo _statsRepo;

        private ICollectionView stats;
        public ICollectionView Stats
        {
            get { return stats; }
            set { SetProperty(ref stats, value); }
        }

        private Dictionary<string, RocketLauncher.Statistics.Stats> globalStats;
        public Dictionary<string, RocketLauncher.Statistics.Stats> GlobalStats
        {
            get { return globalStats; }
            set { SetProperty(ref globalStats, value); }
        }

        private List<RocketLauncher.Statistics.Stat> topTen;
        public List<RocketLauncher.Statistics.Stat> TopTen
        {
            get { return topTen; }
            set { SetProperty(ref topTen, value); }
        }

        private List<RocketLauncher.Statistics.Stat> topTentimePlayed;
        public List<RocketLauncher.Statistics.Stat> TopTentimePlayed
        {
            get { return topTentimePlayed; }
            set { SetProperty(ref topTentimePlayed, value); }
        }

        public StatsViewModel(IStatsRepo statsRepo, IEventAggregator eventAggregator, ISettingsRepo settingsRepo)
        {
            _statsRepo = statsRepo;
            _eventAggregator = eventAggregator;
            _settingsRepo = settingsRepo;

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe((x) => UpdateStatsForSystem(x));
        }

        private void UpdateStatsForSystem(string systemName)
        {
            RocketLauncher.Statistics.Stats stats = null;

            if (systemName.ToLower().Contains("main menu"))
                GlobalStats = _statsRepo.GetAllGlobal(_settingsRepo.HypermintSettings.RlPath +
                            @"\Data\Statistics\Global Statistics.ini");
            else
                stats = _statsRepo.GetStatsForSystem(_settingsRepo.HypermintSettings.RlPath +
                "\\Data\\Statistics\\" + systemName + ".ini");

            if (stats != null)
            {
                Stats = new ListCollectionView(stats.OrderByDescending(x => x.TimesPlayed).ToList());                

                TopTen = stats
                    .OrderByDescending(x => x.TimesPlayed)
                    .Take(10)
                    .ToList();

                TopTentimePlayed = stats
                    .OrderByDescending(x => x.TotalTimePlayed.Ticks)
                    .Take(10).ToList();                    
            }
        }

        private ListCollectionView GetListCollectionView()
        {
            return (ListCollectionView)CollectionViewSource
                            .GetDefaultView(Stats);
        }
    }

    public class SortStatsByPlayed : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x as Stat == null && y as Stat == null)
            {
                throw new ArgumentException("stats can only sort stat object." );
            }
            if (((Stat)x).TimesPlayed > ((Stat)y).TimesPlayed)
            {
                return 1;
            }
            return -1;
        }
    }
}
