using System;
using System.Collections.Generic;
using System.IO;
using Hypermint.Base.Constants;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public class FavoriteService : IFavoriteService
    {
        public List<string> GetFavoritesForSystem(string system, string hyperSpinPath)
        {
            var favoriteTextFile = Path.Combine(hyperSpinPath,
                Root.Databases,
                system,
                "favorites.txt"
                );

            var favoritesList = new List<string>();

            if (!File.Exists(favoriteTextFile))
                return favoritesList;

            using (StreamReader reader = new StreamReader(favoriteTextFile))
            {
                var line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    favoritesList.Add(line);
                }
            }

            return favoritesList;
        }
    }
}
