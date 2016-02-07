using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hypermint.Base.Paths;
using System.IO;
using HsServices = Hs.Hypermint.Services;
using Hypermint.Base.Constants;

namespace AuditerTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ScanGameListForMedia()
        {
            // Set the Hs directory by searching
            Installation.HsPath = Installation.SearchForInstall("HyperSpin");

            var mainMenuXmlPath = Path.Combine(Installation.HsPath, Root.Databases, "Main Menu\\Main Menu.xml");

            Assert.IsTrue(File.Exists(mainMenuXmlPath));

            Hypermint.Base.Interfaces.IGameRepo gameRepo = new HsServices.GameRepo();
           
            gameRepo.GetGames(Path.Combine(Installation.HsPath, Root.Databases, "Amstrad CPC\\Amstrad CPC.xml"), "Amstrad CPC");

            HsServices.Auditer.ScanForMedia("Amstrad CPC", gameRepo.GamesList);
                        
        }
    }
}
