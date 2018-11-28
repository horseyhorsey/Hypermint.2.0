using Frontends.Models.Hyperspin;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        bool SaveFavoritesText(Games gamesList, string favoritesTextFile);

        Task SerializeHyperspinXmlAsync(Games gamesList, string finalPath, string databasePath, bool isMultiSystem = false);

        IEnumerable<Game> SearchGames(string hsPath, string systemName, string searchTerm,
            bool searchClone = true, bool searchEnabled = false);
    }
}
