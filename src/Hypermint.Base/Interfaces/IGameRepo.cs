using Hs.HyperSpin.Database;
using System.ComponentModel;

namespace Hypermint.Base.Interfaces
{
    public interface IGameRepo
    {
        //Games BuildSystemGamesList(string systemXml, string systemName = "Main Menu");        
        Games GamesList { get; set; }

        void GetGames(string systemXml, string systemName = "Main Menu");
    }
}
