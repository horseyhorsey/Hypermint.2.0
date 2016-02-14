using Hs.HyperSpin.Database;
using System.Collections.Generic;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public interface IHyperspinXmlService
    {
        bool SerializeHyperspinXml(Games gamesList, string systemName,
            string hyperspinPath, string dbName = "");
        
    }
}
