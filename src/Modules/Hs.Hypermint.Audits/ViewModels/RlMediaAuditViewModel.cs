using Hs.RocketLauncher.AuditBase;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class RlMediaAuditViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IGameRepo _gameRepo;
        private IAuditerRl _rocketAuditer;
        private ISettingsRepo _settingsRepo;
        private IDialogCoordinator _dialogService;
        private IRegionManager _regionManager;
        private ISelectedService _selectedService;

        #region Properties

        private string _selectedSystem;

        private string currentColumnHeader;
        public string CurrentColumnHeader
        {
            get { return currentColumnHeader; }
            set { SetProperty(ref currentColumnHeader, value); }
        }

        private string currentColumnType;
        public string CurrentColumnType
        {
            get { return currentColumnType; }
            set { SetProperty(ref currentColumnType, value); }
        }

        private string auditHeader ;
        public string AuditHeader 
        {
            get { return auditHeader ; }
            set { SetProperty(ref auditHeader , value); }
        }

        private string filterText;
        public string FilterText
        {
            get { return filterText; }
            set { SetProperty(ref filterText, value); }
        }

        private ICollectionView auditList;
        public ICollectionView AuditList
        {
            get { return auditList; }
            set { SetProperty(ref auditList, value); }
        }

        private ICollectionView auditListDefaults;
        public ICollectionView AuditListDefaults
        {
            get { return auditListDefaults; }
            set { SetProperty(ref auditListDefaults, value); }
        }
        #endregion

        private CustomDialog customDialog;
        private IGameLaunch _gameLaunch;
        private IFolderExplore _fExplorer;

        public RocketLaunchAudit SelectedGame { get; set; }

        public DelegateCommand BezelScanCommand { get; }
        public DelegateCommand<object> CurrentCellChanged { get; set; }
        public DelegateCommand PauseMediaScanCommand { get; private set; }
        public DelegateCommand FadeScanCommand { get; private set; }
        public DelegateCommand BezelEditCommand { get; private set; }
        public DelegateCommand<string> LaunchRlMode { get; private set; }
        public DelegateCommand OpenFolderCommand { get; private set; } 

        public RlMediaAuditViewModel(IEventAggregator eventAggregator,
              IRegionManager regionManager, IGameRepo gameRepo,
              IAuditerRl rocketAuditer, ISettingsRepo settings, IGameLaunch gameLaunch,
              ISelectedService selectedService, IDialogCoordinator dialogService)
        {
            _eventAggregator = eventAggregator;
            _gameRepo = gameRepo;
            _rocketAuditer = rocketAuditer;
            _settingsRepo = settings;
            _dialogService = dialogService;
            _regionManager = regionManager;
            _selectedService = selectedService;
            _gameLaunch = gameLaunch;

            AuditHeader = "Rocketlaunch Audit";

            _rocketAuditer.RlAudits = new RocketLauncherAudits();
            _rocketAuditer.RlAuditsDefault = new RocketLauncherAudits();

            CurrentCellChanged = new DelegateCommand<object>(
             selectedGameCell =>
             {
                 CurrenCellChanged(selectedGameCell);
             });

            BezelScanCommand = new DelegateCommand(ScanBezel);
            PauseMediaScanCommand = new DelegateCommand(ScanPauseMedia);
            FadeScanCommand = new DelegateCommand(ScanFadeLayers);

            _eventAggregator.GetEvent<GamesUpdatedEvent>().Subscribe(GamesUpdated);            

            LaunchRlMode = new DelegateCommand<string>((x) =>
            {
                _gameLaunch.RocketLaunchGameWithMode(
                    _settingsRepo.HypermintSettings.RlPath,
                    _selectedSystem, _selectedService.CurrentRomname, x);
            });

            OpenFolderCommand = new DelegateCommand(() =>
            {
                var path = Path.Combine(_settingsRepo.HypermintSettings.RlMediaPath,
                    "Fade", _selectedSystem, _selectedService.CurrentRomname);

                if (Directory.Exists(path))
                    _eventAggregator.GetEvent<RequestOpenFolderEvent>().Publish(path);
            });

            _eventAggregator.GetEvent<RlAuditUpdateEvent>().Subscribe(FilesDroppedEventHandler);

        }

        private void FilesDroppedEventHandler(object x)
        {
            var values = (string[])x;

            RocketLaunchAudit game;
            if (values[0] == "_Default")            
                game = _rocketAuditer.RlAuditsDefault
                    .Where(r => r.RomName == values[0]).Single();            
            else
                game = _rocketAuditer.RlAudits
                .Where(r => r.RomName == values[0]).Single();

            if (game == null) return;

            switch (values[1])
            {
                case "Artwork":
                    game.HaveArtwork = true;
                    break;
                case "Controller":
                    game.HaveController = true;
                    break;
                case "Guides":
                    game.HaveGuide = true;
                    break;
                case "Manuals":
                    game.HaveManual = true;
                    break;
                case "Screenshots":
                    game.HaveScreenshots = true;
                    break;
                case "Music":
                    game.HaveMusic = true;
                    break;
                case "SavedGame":
                    game.HaveSaves = true;
                    break;
                case "Videos":
                    game.HaveVideo = true;
                    break;
                case "Bezel":
                    game.HaveBezels = true;
                    break;
                case "BezelBg":
                    game.HaveBezelBg = true;
                    break;
                case "Cards":
                    game.HaveCards = true;
                    break;
                case "Layer 1":
                    game.HaveFadeLayer1 = true;
                    break;
                case "Layer 2":
                    game.HaveFadeLayer2 = true;
                    break;
                case "Layer 3":
                    game.HaveFadeLayer3 = true;
                    break;
                case "Extra Layer 1":
                    game.HaveExtraLayer1 = true;
                    break;
                default:
                    break;
            }

            AuditList.Refresh();
        }

        private void CurrenCellChanged(object selectedGameCell)
        {
            //_eventAggregator.GetEvent<ClearRlFilesEvent>().Publish("");

            try
            {
                //Using reflection to get the underlying DataGrid               
                var datagridProperties =
                     selectedGameCell.GetType().GetProperty("DataGridOwner",
                     BindingFlags.Instance | BindingFlags.NonPublic).GetValue(selectedGameCell, null);

                var dg = datagridProperties as DataGrid;
                SelectedGame = dg.CurrentItem as RocketLaunchAudit;
                _selectedService.CurrentRomname = SelectedGame.RomName;

                AuditHeader = "Rocketlaunch Audit : " 
                    + _selectedService.CurrentRomname + " Selected ";

            }
            catch (Exception) { return; }

            try
            {
                var column = selectedGameCell as DataGridTextColumn;

                CurrentColumnType = column.SortMemberPath;
                CurrentColumnHeader = column.Header.ToString();

            }
            catch (Exception e) { }

            _eventAggregator.GetEvent<UpdateFilesEvent>()
                .Publish(new string[] { currentColumnHeader, SelectedGame.RomName });
        }

        private void ScanPauseMedia()
        {
            if (AuditList != null)
                FilterText = "";

            if (!string.IsNullOrEmpty(_selectedSystem))
            {

                _rocketAuditer.ScanArtwork(_selectedSystem,
                _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanForController(_selectedSystem,
                _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanForGuides(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanForManuals(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanForMusic(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanSaves(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanForMultiGame(_selectedSystem,
                _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanVideos(_selectedSystem,
                _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanScreenshots(_selectedSystem,
                _settingsRepo.HypermintSettings.RlMediaPath);

                AuditList = new ListCollectionView(_rocketAuditer.RlAudits);
            }
        }

        private void ScanFadeLayers()
        {
            if (AuditList != null)
                FilterText = "";

            if (!string.IsNullOrEmpty(_selectedSystem))
            {

                _rocketAuditer.ScanFadeLayers(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                AuditList = new ListCollectionView(_rocketAuditer.RlAudits);
            }

        }

        /// <summary>
        /// Scan the bezels folder for all cards, bezels & backgrounds.
        /// </summary>
        private void ScanBezel()
        {
            if (AuditList != null)
                FilterText = "";

            if (!string.IsNullOrEmpty(_selectedSystem))
            {
                _rocketAuditer.ScanForCards(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanForBezels(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                AuditList = new ListCollectionView(_rocketAuditer.RlAudits);
            }
        }

        private void GamesUpdated(string systemName)
        {
            if (Directory.Exists(_settingsRepo.HypermintSettings.RlPath))
            {
                if (AuditList != null)
                    FilterText = "";

                if (!systemName.ToLower().Contains("main menu"))
                {
                    _selectedSystem = systemName;

                    if (_gameRepo.GamesList != null)
                    {
                            _rocketAuditer.RlAudits.Clear();
                            _rocketAuditer.RlAuditsDefault.Clear();

                            _rocketAuditer.RlAuditsDefault.Add(new RocketLaunchAudit()
                            {
                                RomName = "_Default",
                                Description = ""
                            });

                            foreach (var game in _gameRepo.GamesList)
                            {
                                _rocketAuditer.RlAudits.Add(new RocketLaunchAudit
                                {
                                    RomName = game.RomName,
                                    Description = game.Description,

                                });
                            }

                            _rocketAuditer.ScanRocketLaunchMedia(systemName, _settingsRepo.HypermintSettings.RlMediaPath);

                            AuditListDefaults = new ListCollectionView(_rocketAuditer.RlAuditsDefault);
                            AuditList = new ListCollectionView(_rocketAuditer.RlAudits);
                        
                    }
                }
            }

        }

        private void SetAuditGameFilter()
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(AuditList);

            cv.Filter = o =>
            {
                var g = o as RocketLaunchAudit;

                if (_selectedService.IsMainMenu())
                    return g.RomName.ToUpper().Contains(FilterText.ToUpper());
                else
                    return g.Description.ToUpper().Contains(FilterText.ToUpper()); ;
            };
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "FilterText")
            {
                SetAuditGameFilter();
            }
        }

    }
}
