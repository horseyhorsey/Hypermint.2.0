using System;
using System.Threading.Tasks;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Events;
using System.Collections.ObjectModel;
using Frontends.Models.RocketLauncher;
using System.Windows.Input;
using Prism.Commands;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class RlDefaultAuditViewModel : RlAuditMediaViewModelBase
    {
        private IRlScan _rlScan;
        private ISettingsHypermint _settings;
        private ISelectedService _selected;

        public RlDefaultAuditViewModel()
        {
        }

        public RlDefaultAuditViewModel(IEventAggregator ea, IHyperspinManager hyperspinManager, IGameLaunch gameLaunch, ISettingsHypermint settings, ISelectedService selected, IRlScan rlScan)
            : base(ea, hyperspinManager, gameLaunch, settings, selected, rlScan)
        {
            _rlScan = rlScan;
            _settings = settings;
            _selected = selected;

            DefaultFolders = new ObservableCollection<RlAudit>();

            ScanDefaultsCommand = new DelegateCommand(async () => await ScanForMedia());
        }

        public ICommand ScanDefaultsCommand { get; set; }

        public override string OnColumnChanged(object selectedGameCell)
        {
            var _currentColumnHeader = base.OnColumnChanged(selectedGameCell);

            PublishCurrentGame(_currentColumnHeader);

            return string.Empty;
        }

        /// <summary>
        /// Publishes the current selected game to change the artwork.
        /// </summary>
        private void PublishCurrentGame(string columHeader)
        {
                //_selectedService.SelectedGames.Clear();
                //_selectedService.SelectedGames.Add(game);
                _eventAggregator.GetEvent<UpdateFilesEvent>().Publish(new string[] { columHeader, "_Default" });
                //_eventAggregator.GetEvent<GameSelectedEvent>().Publish(new string[] { game.Name, _currentColumnHeader });
        }

        public async override Task ScanForMedia()
        {
            try
            {
                DefaultFolders.Clear();

                var audit = await _rlScan.ScanDefaultsAsync(_settings.HypermintSettings.RlMediaPath, _selected.CurrentSystem);

                DefaultFolders.Add(audit);
                //DefaultFolders[0].HaveFade
            }
            catch (Exception ex) { }

        }

        private ObservableCollection<RlAudit> defaultFolders;
        public ObservableCollection<RlAudit> DefaultFolders
        {
            get { return defaultFolders; }
            set { SetProperty(ref defaultFolders, value); }
        }
    }
}
