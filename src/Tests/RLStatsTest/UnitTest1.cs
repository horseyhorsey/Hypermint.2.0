using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hs.Hypermint.Services;

namespace RLStatsTest
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        public void TestMethod1()
        {
            IStatsRepo statRepo = new StatRepo();
            // Pull all stats from a stats file
            var statsList = statRepo.GetStatsForSystem(@"I:\RocketLauncher\Data\Statistics\Amstrad CPC.ini");

        }

        [TestMethod]
        public void GetSingleGameStats()
        {
            IStatsRepo statRepo = new StatRepo();
            var gameStat = statRepo.GetSingleGameStats(@"I:\RocketLauncher\Data\Statistics",
                "Amstrad CPC", "Robocop (Europe)");
            ;
        }
    }
}
