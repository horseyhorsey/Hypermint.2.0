using Frontend.Helper.Model.Hyperspin;
using Frontend.Helper.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hs.Hypermint.Business.Hyperspin
{
    public class HyperspinDataProvider : IHyperspinXmlDataProvider
    {
        private IHyperspinSerializer _hsSerializer;

        public HyperspinDataProvider()
        {
        }

        public async Task<IEnumerable<Game>> GetAllGames(string frontendPath, string systemName, string dbName = "")
        {
            _hsSerializer = new HyperspinSerializer(frontendPath, systemName, dbName);

            return await _hsSerializer.DeserializeAsync();
        }

        public async Task<IEnumerable<MainMenu>> GetAllSystems(string frontendPath, string systemName, string dbName = "")
        {
            _hsSerializer = new HyperspinSerializer(frontendPath, systemName, dbName);

            return await _hsSerializer.DeserializeMenusAsync();
        }
    }
}
