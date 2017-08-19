using Horsesoft.Frontends.Helper.Paths.Hyperspin;
using Hs.Hypermint.BusinessTests.Fixtures.Real;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Hs.Hypermint.BusinessTests.IntergrationsTests
{
    [Collection("HyperspinXmlDataCollection")]
    public class HyperspinDataProviderTests
    {
        HyperspinXmlDataFixture _fixture;

        public HyperspinDataProviderTests()
        {
            _fixture = new HyperspinXmlDataFixture();
        }

        [Theory]
        [InlineData("Amstrad CPC", 1789)]
        [InlineData("Commodore Vic 20", 8)]
        [InlineData("Nintendo 64", 18)]
        [InlineData("MAME", 9)]
        public async void GetAllHyperspinGames(string systemName, int expectedCount)
        {
            var dir = Environment.CurrentDirectory + "\\TestData\\Hyperspin";

            var games = await _fixture._xmlDataProvider.GetAllGames(dir, systemName);

            Assert.True(games.Count() == expectedCount);
        }

        [Theory]
        [InlineData("\\TestData\\Hype", "Amstrad CPC")]
        [InlineData("\\TestData\\Hype", "Nintendo 64")]
        public async void GetAllHyperspinGames_ThrowsFileNotFoundException(string folder, string systemName)
        {
            var dir = Environment.CurrentDirectory + folder;

            await Assert.ThrowsAsync<FileNotFoundException>(async () =>
            {
                var games = await _fixture._xmlDataProvider.GetAllGames(dir, systemName);
            });
        }

        [Theory]
        [InlineData("Amstrad CPC", "Dizzy", 13)]
        [InlineData("Nintendo 64", "007", 2)]        
        public async void SearchForGamesAsync(string systemName, string searchString, int expectedCount)
        {
            var db = PathHelper.GetSystemDatabasePath(_fixture._frontend.Path, systemName) + "\\" + $"{systemName}.xml";
            var games = await  _fixture._xmlDataProvider.SearchXmlAsync(_fixture._frontend.Path, systemName, db, searchString);

            Assert.True(games.Count() == expectedCount);            
        }
    }
}
