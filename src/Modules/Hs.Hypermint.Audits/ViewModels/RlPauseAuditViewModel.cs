using System;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using System.Linq;
using System.Threading.Tasks;
using Prism.Events;
using MahApps.Metro.Controls.Dialogs;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class RlPauseAuditViewModel : RlAuditMediaViewModelBase
    {
        private IRlScan _rlScan;
        private ISettingsHypermint _settings;

        public RlPauseAuditViewModel()
        {
        }

        public RlPauseAuditViewModel(IEventAggregator ea, IHyperspinManager hyperspinManager, IGameLaunch gameLaunch, ISettingsHypermint settings, ISelectedService selected, IRlScan rlScan, IDialogCoordinator dialogService) : base(ea, hyperspinManager, gameLaunch, settings, selected, rlScan, dialogService)
        {
            _rlScan = rlScan;
            _settings = settings;
        }

        /// <summary>
        /// Scans for pause media. **Scans all needs changing
        /// </summary>
        public override async Task ScanForMedia()
        {
            IsBusy = true;

            if (_hyperspinManager.CurrentSystemsGames.Count > 0)
                await _rlScan.ScanPauseAsync(_hyperspinManager.CurrentSystemsGames.Select(x => x.Game), _settings.HypermintSettings.RlPath + "\\Media");            

            IsBusy = false;
        }
    }
}
