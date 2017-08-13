using System.Collections.Generic;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public interface IFavoriteService
    {
        /// <summary>
        /// Get favorites from Hyperspin favorites.txt
        /// </summary>
        /// <param name="system"></param>
        /// <param name="hyperSpinPath"></param>
        /// <returns></returns>
        List<string> GetFavoritesForSystem (string system, string hyperSpinPath);
    }
}
