﻿using Frontends.Models.Hyperspin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hs.Hypermint.Business.Hyperspin
{
    public interface IHyperspinXmlDataProvider
    {
        Task<IEnumerable<Game>> GetAllGames(string frontendPath, string systemName, string dbName = "");

        Task<IEnumerable<MainMenu>> GetAllSystems(string frontendPath, string systemName, string dbName = "");

        Task<IEnumerable<Game>> SearchXmlAsync(string frontendPath, string systemName, string xmlPath, string searchText);
    }
}
