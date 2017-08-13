using Hs.HyperSpin.Database;

namespace Hypermint.Base.Interfaces
{
    public interface IMainMenuRepo
    {
        string[] GetMainMenuDatabases(string MainMenuFolder);

        void BuildMainMenuItems(string mainMenuXml, string iconsPath = "");

        Systems Systems { get; set; }
    }
}
