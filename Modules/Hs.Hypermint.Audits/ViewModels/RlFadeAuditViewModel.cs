using System;
using System.Threading.Tasks;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Events;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class RlFadeAuditViewModel : RlAuditMediaViewModelBase
    {
        public RlFadeAuditViewModel()
        {
        }

        public RlFadeAuditViewModel(IEventAggregator ea, IHyperspinManager hyperspinManager, IGameLaunch gameLaunch, ISettingsHypermint settings, ISelectedService selected, IRlScan rlScan) : base(ea, hyperspinManager, gameLaunch, settings, selected, rlScan)
        {
        }

        public override Task ScanForMedia()
        {
            throw new NotImplementedException();
        }
    }
}
