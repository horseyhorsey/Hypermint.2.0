using Hs.Hypermint.BusinessTests.Fixtures.Real;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        [InlineData("Amstrad CPC", 2)]
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
    }
}
