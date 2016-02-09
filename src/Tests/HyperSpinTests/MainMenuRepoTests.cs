using Hs.Hypermint.Services;
using Hypermint.Base.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using Hypermint.Shell;
using Moq;

namespace Hs.Hypermint.Services.Tests
{
    [TestClass()]
    public class MainMenuRepoTests
    {

        [TestMethod()]
        public void BuildMainMenuItemsTest()
        {
            var mainMenuRepo = new MainMenuRepo();
            var settingsRepo = new SettingsRepo();

            //settingsRepo.HypermintSettings.HsPath = MockData.path;


            //var hsPath = settingsRepo.HypermintSettings.HsPath;
            //hsPath += @"\Databases\Main Menu\Main Menu.xml";
            //Assert.IsFalse(string.IsNullOrEmpty(hsPath));

            //var systems = mainMenuRepo.BuildMainMenuItems(hsPath, "");

            //foreach (var item in systems)
            //{
            //    Trace.WriteLine(item.Name);
            //}
        }

        [TestMethod()]
        public void GetMainMenuDatabasesTest()
        {
            //var settingsRepo = setting;
            //var mainMenuRepo = new MainMenuRepo();

            //settingsRepo.LoadHypermintSettings();

            //var hsPath = settingsRepo.HypermintSettings.HsPath;
            //var xmlName = "Main Menu.xml";
            //var mainMenuXml = Path.Combine(hsPath, @"Databases\Main Menu", xmlName);

            //var rlLaunchMediaPath = settingsRepo.HypermintSettings.RlMediaPath;
            //mainMenuRepo.BuildMainMenuItems(mainMenuXml, rlLaunchMediaPath + @"\Icons");
            //var systemItems = mainMenuRepo.Systems;

            //foreach (var item in systemItems)
            //{
            //    Trace.WriteLine(string.Format("SystemName: {0} Icon: {1}", item.Name, item.SysIcon));
            //}

            var mock =  Mock.Of<ISettingsRepo>();

            mock.LoadHypermintSettings();


            var neh = mock.HypermintSettings;
            //var t = mock.HypermintSettings.HsPath;
    }

    }
}