using Hs.RocketLauncher.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hypermint.Base.Interfaces
{
    public interface IStatsRepo
    {
        Stats GetStatsForSystem(string rlStatsIni);

        Stat GetSingleGameStats(string statsPath, string systemName, string romName);
    }
}
