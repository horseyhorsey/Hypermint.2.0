using Frontends.Models.Hyperspin;
using Frontends.Models.Interfaces;
using Horsesoft.Frontends.Helper.Paths.Hyperspin;
using Horsesoft.Frontends.Helper.Serialization;
using Horsesoft.Frontends.Helper.Tools.Search;
using Hypermint.Base;
using System;
using System.Collections.Generic;
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

        [Obsolete("Not implemented")]
        public async Task<IEnumerable<Genre>> GetAllGenreDatabases(string frontendPath, string systemName)
        {
            //_hsSerializer = new HyperspinSerializer(frontendPath, systemName);
            var path = PathHelper.GetSystemDatabasePath(frontendPath, systemName);

            return await _hsSerializer.GetGenresAsync(path + "\\genre.xml");
        }

        public async Task<IEnumerable<MainMenu>> GetAllSystems(string frontendPath, string systemName, string dbName = "")
        {
            _hsSerializer = new 
                HyperspinSerializer(frontendPath, systemName, dbName);

            return await _hsSerializer.DeserializeMenusAsync();
        }

        public async Task<IEnumerable<Game>> SearchXmlAsync(string frontendPath, string systemName, string xmlPath, string searchText)
        {
            _hsSerializer = new HyperspinSerializer(frontendPath, systemName);

            return await _searchGameXml.Search(systemName, xmlPath, new string[] { searchText });
        }
    }
}
