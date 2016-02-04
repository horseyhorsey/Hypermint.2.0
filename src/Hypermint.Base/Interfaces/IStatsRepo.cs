using Hs.RocketLauncher.Statistics;


namespace Hypermint.Base.Interfaces
{
    public interface IStatsRepo
    {
        Stats GetStatsForSystem(string rlStatsIni);

        Stat GetSingleGameStats(string statsPath, string systemName, string romName);
    }
}
