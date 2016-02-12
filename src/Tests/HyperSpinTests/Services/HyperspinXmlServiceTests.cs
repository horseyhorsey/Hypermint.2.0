using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hs.Hypermint.DatabaseDetails.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hs.HyperSpin.Database;
using Hs.Hypermint.Services;
using System.IO;
using Hypermint.Base.Paths;
using Hypermint.Base.Constants;

namespace Hs.Hypermint.DatabaseDetails.Services.Tests
{
    [TestClass()]
    public class HyperspinXmlServiceTests
    {
        [TestMethod()]
        public void SerializeHyperspinXmlTest()
        {
            var gameRepo = new GameRepo();

            var hsPath = Installation.SearchForInstall("Hyperspin");

            Assert.IsTrue(!string.IsNullOrEmpty(hsPath));

            var hyperspinPath = Path.Combine(hsPath, Root.Databases, "Amstrad CPC\\Amstrad CPC.xml");

            Assert.IsTrue(File.Exists(hyperspinPath));

            gameRepo.GetGames(hyperspinPath, "Amstrad CPC");

            Assert.IsNotNull(gameRepo.GamesList);

            var xmlService = new Services.HyperspinXmlService();

            xmlService.SerializeHyperspinXml(gameRepo.GamesList, "Amstrad CPC", hsPath, "!TestDb2016");
        }
    }
}