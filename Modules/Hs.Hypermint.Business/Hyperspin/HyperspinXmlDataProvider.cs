using Frontends.Models.Hyperspin;
using Horsesoft.Frontends.Helper.Paths.Hyperspin;
using Horsesoft.Frontends.Helper.Serialization;
using Horsesoft.Frontends.Helper.Tools;
using Horsesoft.Frontends.Helper.Tools.Search;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hs.Hypermint.Business.Hyperspin
{
    public class HyperspinDataProvider : IHyperspinXmlDataProvider
    {
        private IHyperspinSerializer _hsSerializer;
        private ISearch<Game> _searchGameXml;

        public HyperspinDataProvider()
        {
            _searchGameXml = new XmlSearch<Game>();            
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

        public async Task<IEnumerable<Game>> SearchXmlAsync(string frontendPath, string systemName, string xmlPath, string searchText)
        {
            _hsSerializer = new HyperspinSerializer(frontendPath, systemName);            

            return await _searchGameXml.Search(systemName, xmlPath, new string[] { searchText });
        }
    }
}
