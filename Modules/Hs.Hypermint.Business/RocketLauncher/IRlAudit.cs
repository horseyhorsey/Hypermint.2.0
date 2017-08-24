using Frontends.Models.Hyperspin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hs.Hypermint.Business.RocketLauncher
{
    public interface IRlAudit
    {
        Task<bool> ScanAllSystemMedia(IEnumerable<Game> games);
    }
}
