using Hs.HyperSpin.Database;
using System.Collections.Generic;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public interface IHyperspinXmlService
    {
        bool SerializeHyperspinXml(Games gamesList, string systemName,
            string hyperspinPath, string dbName = "", bool isMultiSystem = false);

        bool SerializeGenreXml(Games gamesList, string systemName, string hyperspinPath);

        bool SerializeMainMenuXml(Systems systemList, string hyperspinPath,
            string mainMenuName = "Main Menu");

        Game SearchGameFromXml(string romName, string system, string hyperspinPath);

        List<Game> SearchRomStringsListFromXml(List<string> romsNamesList, string system, string hyperspinPath);

    }
}
