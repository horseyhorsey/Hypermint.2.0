using Hs.HyperSpin.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public interface IHyperspinXmlService
    {
        bool SerializeHyperspinXml(Games gamesList, string systemName,
            string hyperspinPath, string dbName = "");
    }
}
