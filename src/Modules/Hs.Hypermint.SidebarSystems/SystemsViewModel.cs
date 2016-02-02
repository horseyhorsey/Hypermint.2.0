using Hs.HyperSpin.Database;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Hs.Hypermint.SidebarSystems
{
    public class SystemsViewModel
    {
        private ICollectionView _systemItems;
        public ICollectionView SystemItems { get; private set; }

        IMainMenuRepo _mainMenuRepo;

        private IEventAggregator _eventAggregator;

        public SystemsViewModel(IMainMenuRepo main, IEventAggregator eventAggregator)
        {
            _mainMenuRepo = main;
            _eventAggregator = eventAggregator;

            SystemItems = new ListCollectionView(
        _mainMenuRepo.BuildMainMenuItems(@"I:\HyperSpin\Databases\Main Menu\Main Menu.xml",
        @"I:\RocketLauncher\Media\Icons\"));

            SystemItems.CurrentChanged += SystemItems_CurrentChanged;

        }

        private void SystemItems_CurrentChanged(object sender, System.EventArgs e)
        {
            MainMenu system = SystemItems.CurrentItem as MainMenu;
            if (system != null)
            {
                this._eventAggregator.GetEvent<SystemSelectedEvent>().Publish(system.Name);
            }
        }
    }
}
