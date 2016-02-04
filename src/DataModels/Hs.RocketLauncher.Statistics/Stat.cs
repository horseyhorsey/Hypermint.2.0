using System;
using System.Collections.Generic;

namespace Hs.RocketLauncher.Statistics
{
    public class Stat
    {
        public static List<string> gameStatsList = new List<string>();
        public static string StatsPath { get; set; }
        public int TimesPlayed { get; set; }
        public DateTime LastTimePlayed { get; set; }
        public TimeSpan AvgTimePlayed { get; set; }
        public TimeSpan TotalTimePlayed { get; set; }
        public string _systemName { get; set; }
        public string Rom { get; set; }
        public TimeSpan TotalOverallTime;
    }
}
