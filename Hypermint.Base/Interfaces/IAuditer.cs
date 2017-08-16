using Frontends.Models.Hyperspin;
using System.Threading.Tasks;

namespace Hypermint.Base.Interfaces
{
    public interface IAuditer
    {
        AuditsMenu AuditsMenuList { get; set; }

        AuditsGame AuditsGameList { get; set; }

        Task ScanForMediaAsync(string hyperspinPath, string systemName, Games databaseGameList);        

        void ScanForMediaMainMenu(string hyperspinPath, Games mainMenuList);

    }
}
