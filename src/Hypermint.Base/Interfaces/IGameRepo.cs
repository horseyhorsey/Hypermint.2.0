using Frontends.Models.Hyperspin;
using System.Threading.Tasks;

namespace Hypermint.Base.Interfaces
{
    public interface IGameRepo
    {              
        Games GamesList { get; set; }

        void GetGames(string systemXml, string systemName = "Main Menu");

        Task GetGamesAsync(string systemXml, string systemName = "Main Menu");

        void ScanForRoms(string rlPath, string systemName);
    }
}
