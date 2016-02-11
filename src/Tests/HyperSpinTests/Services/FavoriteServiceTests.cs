using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hs.Hypermint.DatabaseDetails.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.DatabaseDetails.Services.Tests
{
    [TestClass()]
    public class FavoriteServiceTests
    {
        [TestMethod()]
        public void GetFavoritesForSystemTest()
        {
            var faveService = new FavoriteService();

            var systemName = "Amstrad CPC";

            var hyperSpinPath = @"I:\Hyperspin";

            var favoriteList = faveService.GetFavoritesForSystem(systemName, hyperSpinPath);

            Assert.IsNotNull(favoriteList);

            Assert.IsTrue(favoriteList.Count > 0);

      }
    }
}