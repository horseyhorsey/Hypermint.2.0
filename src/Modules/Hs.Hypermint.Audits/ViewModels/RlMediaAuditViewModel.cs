using Hs.RocketLauncher.AuditBase;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class RlMediaAuditViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IGameRepo _gameRepo;
        private IAuditerRl _rocketAuditer;

        
        private ISettingsRepo _settingsRepo;

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

        public RlMediaAuditViewModel(IEventAggregator eventAggregator,IGameRepo gameRepo
            ,IAuditerRl rocketAuditer, ISettingsRepo settings)
        {
            _eventAggregator = eventAggregator;
            _gameRepo = gameRepo;
            _rocketAuditer = rocketAuditer;
            _settingsRepo = settings;

            _rocketAuditer.RlAudits = new RocketLauncherAudits();
            _rocketAuditer.RlAuditsDefault = new RocketLauncherAudits();

            _eventAggregator.GetEvent<GamesUpdatedEvent>().Subscribe(GamesUpdated);

        }

        private void GamesUpdated(string systemName)
        {
            if (!systemName.Contains("Main Menu"))
            {
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
