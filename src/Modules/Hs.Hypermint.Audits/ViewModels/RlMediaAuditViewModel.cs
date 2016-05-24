using Hs.RocketLauncher.AuditBase;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
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

        private ICollectionView auditList;
        public ICollectionView AuditList
        {
            get { return auditList; }
            set { SetProperty(ref auditList, value); }
        }

        private ICollectionView auditListDefaults;
        private CustomDialog customDialog;

        private IDialogCoordinator _dialogService;
        private IRegionManager _regionManager;

        public ICollectionView AuditListDefaults
        {
            get { return auditListDefaults; }
            set { SetProperty(ref auditListDefaults, value); }
        }

        public RocketLaunchAudit SelectedGame { get; set; }

        public DelegateCommand BezelScanCommand { get; }
        public DelegateCommand<object> CurrentCellChanged { get; set; }
        public DelegateCommand PauseMediaScanCommand { get; private set; }
        public DelegateCommand FadeScanCommand { get; private set; }
        public DelegateCommand BezelEditCommand { get; private set; } 

        public RlMediaAuditViewModel(IEventAggregator eventAggregator,IRegionManager regionManager, IGameRepo gameRepo
            , IAuditerRl rocketAuditer, ISettingsRepo settings, IDialogCoordinator dialogService)
        {
            _eventAggregator = eventAggregator;
            _gameRepo = gameRepo;
            _rocketAuditer = rocketAuditer;
            _settingsRepo = settings;
            _dialogService = dialogService;
            _regionManager = regionManager;

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

            BezelEditCommand = new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.ContentRegion, "BezelEditView");
            });

            _eventAggregator.GetEvent<GamesUpdatedEvent>().Subscribe(GamesUpdated);

        }

        private void CurrenCellChanged(object selectedGameCell)
        {
            try
            {
                //Using reflection to get the underlying DataGrid               
                var datagridProperties =
                     selectedGameCell.GetType().GetProperty("DataGridOwner",
                     BindingFlags.Instance | BindingFlags.NonPublic).GetValue(selectedGameCell, null);

                var dg = datagridProperties as DataGrid;
                SelectedGame = dg.CurrentItem as RocketLaunchAudit;

            }
            catch (Exception) { return; }

            try
            {
                var column = selectedGameCell as DataGridTextColumn;

                CurrentColumnType = column.SortMemberPath;
                CurrentColumnHeader = column.Header.ToString();

            }
            catch (Exception e) { }

            _eventAggregator.GetEvent<UpdateFilesEvent>().Publish(
                new string[] { currentColumnHeader, SelectedGame.RomName });

        }

        private void ScanPauseMedia()
        {
            if (!string.IsNullOrEmpty(_selectedSystem))
            {
                _rocketAuditer.ScanForMultiGame(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanForGuides(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanForManuals(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanForMusic(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanSaves(_selectedSystem,
                    _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanVideos(_selectedSystem,
                _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanScreenshots(_selectedSystem,
                _settingsRepo.HypermintSettings.RlMediaPath);

                _rocketAuditer.ScanArtwork(_selectedSystem,
                _settingsRepo.HypermintSettings.RlMediaPath);

                AuditList = new ListCollectionView(_rocketAuditer.RlAudits);
            }
        }

        private void ScanFadeLayers()
        {
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

    }
}
