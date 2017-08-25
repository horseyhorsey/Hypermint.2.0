using System.Collections.Generic;
using System.Threading.Tasks;
using Frontends.Models.Hyperspin;
using Frontends.Models.RocketLauncher;
using Hypermint.Base.Interfaces;
using Horsesoft.Frontends.Helper.Auditing;

namespace Hs.Hypermint.Business.RocketLauncher
{
    public class RlScan : IRlScan
    {
        private IRocketLaunchAudit _audit;

        public RlScan()
        {
            _audit = new RocketLaunchAudit(null);
        }

        public string RlPath { get; set; }

        public async Task ScanBezelsAsync(IEnumerable<Game> games, string rlPath)
        {
            await _audit.ScanSystemMediaAsync(RlMediaType.Bezels, games, rlPath);
            await _audit.ScanSystemMediaAsync(RlMediaType.Cards, games, rlPath);
        }

        public async Task<RlAudit> ScanDefaultsAsync(string rlPath, string systemName)
        {
            return await _audit.ScanDefaultsForSystem(rlPath, systemName);
        }

        public async Task ScanPauseAsync(IEnumerable<Game> games, string rlPath)
        {
            await _audit.ScanSystemMediaAsync(RlMediaType.Artwork, games, rlPath);
            await _audit.ScanSystemMediaAsync(RlMediaType.Backgrounds, games, rlPath);
            await _audit.ScanSystemMediaAsync(RlMediaType.Controller, games, rlPath);
            await _audit.ScanSystemMediaAsync(RlMediaType.Guides, games, rlPath);
            await _audit.ScanSystemMediaAsync(RlMediaType.Manuals, games, rlPath);
            await _audit.ScanSystemMediaAsync(RlMediaType.MultiGame, games, rlPath);
            await _audit.ScanSystemMediaAsync(RlMediaType.Music, games, rlPath);
            await _audit.ScanSystemMediaAsync(RlMediaType.SavedGames, games, rlPath);            
            await _audit.ScanSystemMediaAsync(RlMediaType.Videos, games, rlPath);
        }

    }
}
