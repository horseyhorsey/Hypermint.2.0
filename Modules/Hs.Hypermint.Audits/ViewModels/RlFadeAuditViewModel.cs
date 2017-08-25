using System;
using System.Threading.Tasks;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Events;
using System.Linq;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class RlFadeAuditViewModel : RlAuditMediaViewModelBase
    {
        private IRlScan _rlScan;
        private ISettingsHypermint _settings;

        public RlFadeAuditViewModel()
        {
        }

        public RlFadeAuditViewModel(IEventAggregator ea, IHyperspinManager hyperspinManager, IGameLaunch gameLaunch, ISettingsHypermint settings, ISelectedService selected,
            IRlScan rlScan) : base(ea, hyperspinManager, gameLaunch, settings, selected, rlScan)
        {
            _rlScan = rlScan;
            _settings = settings;
        }

        public override async Task ScanForMedia()
        {
            IsBusy = true;

            if (_hyperspinManager.CurrentSystemsGames.Count > 0)
                await _rlScan.ScanFadeAsync(_hyperspinManager.CurrentSystemsGames.Select(x => x.Game), 
                    _settings.HypermintSettings.RlPath + "\\Media");

            IsBusy = false;
        }
    }
}
