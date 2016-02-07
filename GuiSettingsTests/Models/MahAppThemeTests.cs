using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hypermint.Shell.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hypermint.Shell.Models.Tests
{
    [TestClass()]
    public class MahAppThemeTests
    {
        [TestMethod()]
        public void MahAppAvailableThemesTest()
        {
            var mahAppTheme = new MahAppTheme();

            Assert.IsNotNull(mahAppTheme.AvailableThemes);
        }
    }
}