using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class HsMediaAuditViewModel : ViewModelBase
    {        
        #region Fields
        private IEventAggregator _eventAggregator;
        private ISettingsRepo _settings;
        private IGameRepo _gameRepo;
        private IAuditer _auditer;
        public DelegateCommand RunScanCommand { get; private set; }
        private ICollectionView _auditList;
        private ISelectedService _selectedService;
        #endregion

        #region Properties
        private bool runningScan;
        public bool RunningScan
        {
            get { return runningScan; }
            set { SetProperty(ref runningScan, value); }
        }
        private string message = "Test Message";
        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }

        public ICollectionView AuditList
        {
            get { return _auditList; }
            set { SetProperty(ref _auditList, value); }
        }
        #endregion

        #region ctors
        public HsMediaAuditViewModel(ISettingsRepo settings, IGameRepo gameRepo,
            IEventAggregator eventAggregator,IAuditer auditer, ISelectedService selectedService)
        {
            _eventAggregator = eventAggregator;
            _settings = settings;
            _auditer = auditer;
            _auditer.AuditsGameList = new HyperSpin.Database.Audit.AuditsGame();
            _gameRepo = gameRepo;
            _selectedService = selectedService;

            //This event is called after main database view is updated
            _eventAggregator.GetEvent<GamesUpdatedEvent>().Subscribe(gamesUpdated);
            
            RunScanCommand = new DelegateCommand(RunScan);            
        }

        private void gamesUpdated(string systemName)
        {
            if (_gameRepo.GamesList != null)
            {
                _auditer.AuditsGameList.Clear();

                foreach (var item in _gameRepo.GamesList)
                {
                    _auditer.AuditsGameList.Add(new HyperSpin.Database.Audit.AuditGame
                    {
                        RomName = item.RomName
                    });
                }

                AuditList = new ListCollectionView(_auditer.AuditsGameList);
            }
        }
        #endregion

        private void GamesList_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void RunScan()
        {
            var hsPath = _settings.HypermintSettings.HsPath;

            try
            {
                if (AuditList != null && Directory.Exists(hsPath))
                {
                    var systemName = _selectedService.CurrentSystem;

                    if (systemName.Contains("Main Menu"))
                    {
                        _auditer.ScanForMediaMainMenu(
                            hsPath, _gameRepo.GamesList);

                        AuditList = new ListCollectionView(_auditer.AuditsMenuList);
                    }
                    else
                    {
                        _auditer.ScanForMedia(
                            hsPath, systemName, _gameRepo.GamesList
                             );

                        AuditList = new ListCollectionView(_auditer.AuditsGameList);
                    }
                }
            }
            catch (Exception) { }

        }
        
        private void SetMessage(string obj)
        {
            Message = obj;
        }
    }
}
