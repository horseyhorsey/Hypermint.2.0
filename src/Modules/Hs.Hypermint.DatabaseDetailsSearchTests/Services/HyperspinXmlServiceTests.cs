using NUnit.Framework;
using Hs.Hypermint.DatabaseDetails.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.DatabaseDetails.Services.Tests
{
    [TestFixture()]
    public class HyperspinXmlServiceTests
    {
        [Test()]
        public void SearchGamesTest()
        {
            IHyperspinXmlService hs = new HyperspinXmlService();

            var hsPath = @"I:\Hyperspin";

            var games = hs.SearchGames(hsPath, "Amstrad CPC", "Dizzy");

            Assert.IsNotNull(games);
        }
    }
}