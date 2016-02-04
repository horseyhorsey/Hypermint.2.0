using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hypermint.Base.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Hypermint.Base.Paths.Tests
{
    [TestClass()]
    public class InstallationTests
    {
        [TestMethod()]
        public void SearchForHsInstallTest()
        {
            var findHsInstallationPath = Installation.SearchForInstall("Hyperspin");

            Trace.WriteLine("HyperSpin Path: " + findHsInstallationPath);

            Assert.AreNotSame("", "*","Cannot find Hyperspin path");

            Assert.IsTrue(Directory.Exists(findHsInstallationPath));

        }

        [TestMethod()]
        public void SearchForRlInstallTest()
        {
            var findRlInstallationPath = Installation.SearchForInstall("RocketLauncher");

            Trace.WriteLine("Rocketlauncher Path: " + findRlInstallationPath);
            
            Assert.AreNotSame("", "*", "Cannot find RocketLauncher path");

            Assert.IsTrue(Directory.Exists(findRlInstallationPath));
        }
    }
}