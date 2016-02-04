﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

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

            var hsPath = settingsRepo.GetHsPathFromSettings();
            hsPath += @"\Databases\Main Menu\Main Menu.xml";
            Assert.IsFalse(string.IsNullOrEmpty(hsPath));

            var systems = mainMenuRepo.BuildMainMenuItems(hsPath, "");

            foreach (var item in systems)
            {
                Trace.WriteLine(item.Name);
            }
        }
    }
}