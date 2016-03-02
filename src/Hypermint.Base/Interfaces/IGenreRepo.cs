using System.Collections.Generic;

namespace Hypermint.Base.Interfaces
{
    public interface IGenreRepo
    {
        List<string> GenreList { get; set; }

        void PopulateGenres(string systemName);

    }
}
