using Hs.HyperSpin.Database;

namespace Hypermint.Base.Interfaces
{
    public interface IGameRepo
    {              
        Games GamesList { get; set; }

        void GetGames(string systemXml, string systemName = "Main Menu");

        void ScanForRoms(string rlPath, string systemName);
    }
}
