using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hs.Hypermint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.Services.Tests
{
    [TestClass()]
    public class FileFolderServiceTests
    {
        [TestMethod()]
        public void OpenFolderTest()
        {
            var f = new FileFolderService();

            f.OpenFolder(@"C:\");
        }
    }
}