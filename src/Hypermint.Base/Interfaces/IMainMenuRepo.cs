using Hs.HyperSpin.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Interfaces
{
    public interface IMainMenuRepo
    {
        string[] GetMainMenuDatabases(string MainMenuFolder);

        Systems BuildMainMenuItems(string mainMenuXml, string iconsPath = "");
    }
}
