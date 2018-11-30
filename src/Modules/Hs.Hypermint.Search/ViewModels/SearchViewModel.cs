using Hypermint.Base.Interfaces;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Windows.Data;
using Hypermint.Base.Services;
using Prism.Events;
using Hypermint.Base;
using System.Collections;
using System.Windows.Input;
using Frontends.Models.Hyperspin;
using Hypermint.Base.Model;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace Hs.Hypermint.Search.ViewModels
{
    public class SearchViewModel : HyperMintModelBase
    {
        #region Fields               
        private IHyperspinManager _hyperspinManager;
        private ObservableCollection<MenuItemViewModel> _systemVms;
        #endregion

        #region Constructors
        public SearchViewModel(
            IEventAggregator eventAggregator,
            ISettingsHypermint settings,
            ISelectedService selectedSrv, IHyperspinManager hyperspinManager,
            IGameLaunch gameLaunch) : base(eventAggregator, selectedSrv, gameLaunch, settings)
        {
            _hyperspinManager = hyperspinManager;
            Systems = new ListCollectionView(_hyperspinManager.Systems);                        
            
            DockSystemsCommand = new DelegateCommand(() => { SystemsVisible = !SystemsVisible; });
            SelectSystemsCommand = new DelegateCommand<string>(OnSystemsSelected);
        }

        #endregion

        #region Properties

        private ICollectionView systems;
        public ICollectionView Systems
        {
            get { return systems; }
            set { SetProperty(ref systems, value); }
        }

        private bool systemsVisible = true;
        public bool SystemsVisible
        {
            get { return systemsVisible; }
            set { SetProperty(ref systemsVisible, value); }
        }

        #endregion

        #region Commands

        public ICommand SelectSystemsCommand { get; }        
        public ICommand ListBoxChanged { get; private set; }
        public ICommand DockSystemsCommand { get; private set; }

        #endregion

        #region Support Methods   
        
        private void OnSystemsSelected(string x)
        {
            try
            {
                var sysEnabled = 0;
                if (x == "all")
                    sysEnabled = 1;

                foreach (MenuItemViewModel item in _hyperspinManager.Systems)
                {
                    item.Enabled = sysEnabled;
                }
            }
            catch (Exception) { }
        }

        #endregion
    }

    #region Support classes
    public class GameSearch
    {
        public Game Game { get; set; }
        public string WheelImage { get; set; }

        public Uri SystemImage { get; set; }
    }

    #endregion   
}

