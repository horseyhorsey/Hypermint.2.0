using Hs.RocketLauncher.Statistics;
using System.Collections.Generic;

namespace Hypermint.Base.Interfaces
{
    public interface IStatsRepo
    {
        Stats GetStatsForSystem(string rlStatsIni);

        Stat GetSingleGameStats(string statsPath, string systemName, string romName);

        Stats GetGlobalStats(string globalStatsIni);

        Dictionary<string,Stats> GetAllGlobal(string globalStatsIni);
    }
}
