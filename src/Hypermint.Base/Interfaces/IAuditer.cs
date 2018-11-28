using Frontends.Models.Hyperspin;
using Hypermint.Base.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hypermint.Base.Interfaces
{
    public interface IAuditer
    {
        AuditsMenu AuditsMenuList { get; set; }

        AuditsGame AuditsGameList { get; set; }

        Task ScanForMediaAsync(string hyperspinPath, string systemName, IEnumerable<GameItemViewModel> databaseGameList);        

        void ScanForMediaMainMenu(string hyperspinPath, IEnumerable<GameItemViewModel> mainMenuList);

    }
}
