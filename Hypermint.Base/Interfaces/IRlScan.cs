using Frontends.Models.Hyperspin;
using Frontends.Models.RocketLauncher;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hypermint.Base.Interfaces
{
    public interface IRlScan
    {
        string RlPath { get; set; }
        Task ScanBezelsAsync(IEnumerable<Game> games, string rlPath);
        Task ScanPauseAsync(IEnumerable<Game> games, string rlPath);        
        Task<RlAudit> ScanDefaultsAsync(string rlPath, string systemName);
    }
}
