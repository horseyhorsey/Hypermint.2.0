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

        private ICollectionView auditList;
        public ICollectionView AuditList
        {
            get { return auditList; }
            set { SetProperty(ref auditList, value); }
        }

        public RlMediaAuditViewModel(IEventAggregator eventAggregator,IGameRepo gameRepo
            ,IAuditerRl rocketAuditer)
        {
            _eventAggregator = eventAggregator;
            _gameRepo = gameRepo;
            _rocketAuditer = rocketAuditer;

            _rocketAuditer.RlAudits = new RocketLauncherAudits();

            _eventAggregator.GetEvent<GamesUpdatedEvent>().Subscribe(GamesUpdated);

        }

        private void GamesUpdated(string systemName)
        {
            if (!systemName.Contains("Main Menu"))
            {
                if (_gameRepo.GamesList != null)
                {
                    _rocketAuditer.RlAudits.Clear();

                    foreach (var game in _gameRepo.GamesList)
                    {
                        _rocketAuditer.RlAudits.Add(new RocketLaunchAudit
                        {
                            RomName = game.RomName,
                            Description = game.Description,
                            
                        });
                    }

                    AuditList = new ListCollectionView(_rocketAuditer.RlAudits);
                }
            }

        }
    }
}
