using System.Collections.Generic;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public interface IFavoriteService
    {
        List<string> GetFavoritesForSystem (string system, string hyperSpinPath);
    }
}
