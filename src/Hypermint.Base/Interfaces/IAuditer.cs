using Hs.HyperSpin.Database;
using Hs.HyperSpin.Database.Audit;
using System.Collections.Generic;

namespace Hypermint.Base.Interfaces
{
    public interface IAuditer
    {
        AuditsMenu AuditsMenuList { get; set; }

        AuditsGame AuditsGameList { get; set; }

        void ScanForMedia(string hyperspinPath, string systemName, Games databaseGameList);

        void ScanForMediaMainMenu(string hyperspinPath, List<MainMenu> mainMenuList);

    }
}
