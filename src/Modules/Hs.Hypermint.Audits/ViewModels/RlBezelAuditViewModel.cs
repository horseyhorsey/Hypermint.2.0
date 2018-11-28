using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using System.Windows.Input;
using Prism.Events;
using MahApps.Metro.Controls.Dialogs;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class RlBezelAuditViewModel : RlAuditMediaViewModelBase
    {
        private IRlScan _rlScan;
        private ISettingsHypermint _settings;

        public RlBezelAuditViewModel()
        {
        }

        public RlBezelAuditViewModel(IEventAggregator ea, IHyperspinManager hyperspinManager, IGameLaunch gameLaunch, ISettingsHypermint settings, ISelectedService selected, IRlScan rlScan, IDialogCoordinator dialogService) : base(ea, hyperspinManager, gameLaunch, settings, selected, rlScan, dialogService)
        {
            _rlScan = rlScan;
            _settings = settings;
        }

        public ICommand BezelEditCommand { get; set; }

        public override async Task ScanForMedia()
        {
            IsBusy = true;

            if (_hyperspinManager.CurrentSystemsGames.Count > 0)
                   await _rlScan.ScanBezelsAsync(_hyperspinManager.CurrentSystemsGames.Select(x => x.Game), _settings.HypermintSettings.RlPath + "\\Media");

            IsBusy = false;
        }
    }
}
