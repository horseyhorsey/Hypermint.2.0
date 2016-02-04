﻿using Hs.HyperSpin.Database;

namespace Hypermint.Base.Interfaces
{
    public interface IMainMenuRepo
    {
        string[] GetMainMenuDatabases(string MainMenuFolder);

        Systems BuildMainMenuItems(string mainMenuXml, string iconsPath = "");
    }
}
