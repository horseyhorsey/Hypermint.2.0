using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hs.Hypermint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hypermint.Base.Paths;
using System.IO;
using Hypermint.Base.Constants;

namespace Hs.Hypermint.Services.Tests
{
    [TestClass()]
    public class AuditerTests
    {
        [TestMethod()]
        public void ScanForMediaMainMenuTest()
        {
            var auditer = new Auditer();

            Installation.HsPath = Installation.SearchForInstall("HyperSpin");

            var mainMenuXmlPath = Path.Combine(Installation.HsPath, Root.Databases, "Main Menu\\Main Menu.xml");

            Assert.IsTrue(File.Exists(mainMenuXmlPath));

            var gameRepo = new GameRepo();

            gameRepo.GetGames(Path.Combine(Installation.HsPath, Root.Databases, "Main Menu\\Main Menu.xml"), "Main Menu");


            auditer.ScanForMediaMainMenu(Installation.HsPath, gameRepo.GamesList);
        }
    }
}