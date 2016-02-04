using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Hs.Hypermint.Services.Tests
{
    [TestClass()]
    public class SettingsRepoTests
    {
        [TestMethod()]
        public void SaveHsPathToSettingsTest()
        {
            var settingsRepo = new SettingsRepo();

            settingsRepo.HyperSpinPath = "I:\\HyperSpin";

            Assert.IsTrue(settingsRepo.SaveHsPathToSettings(),"HyperSpinPath empty string");

        }

        [TestMethod()]
        public void GetHsPathFromSettings ()
        {
            var settingsRepo = new SettingsRepo();

            Trace.WriteLine(settingsRepo.GetHsPathFromSettings());

            Assert.IsFalse(string.IsNullOrEmpty(settingsRepo.GetHsPathFromSettings()));
        }
    }
}