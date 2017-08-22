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

namespace Hs.Hypermint.Search.ViewModels
{
    public class SearchViewModel : HyperMintModelBase
    {
        #region Fields               
        private IHyperspinManager _hyperspinManager;
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

            SelectSystemsCommand = new DelegateCommand<string>(x =>
            {
                OnSystemsSelected(x);
            });

            // Command for datagrid selectedItems
            SelectionChanged = new DelegateCommand<IList>( items => {
                    OnSelectionChanged(items); });

            DockSystemsCommand = new DelegateCommand(() => { SystemsVisible = !SystemsVisible; });

            //_eventAggregator.GetEvent<SystemsGenerated>().Subscribe(x => OnSystemsUpdated(x));
        }

        private void OnSystemsSelected(string x)
        {
            try
            {
                var sysEnabled = 0;

                if (x == "all")
                    sysEnabled = 1;

                foreach (MainMenu item in _hyperspinManager.Systems)
                {
                    item.Enabled = sysEnabled;
                }

                Systems.Refresh();
            }
            catch (Exception) { }
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
        public ICommand SelectionChanged { get; set; }
        public ICommand ListBoxChanged { get; private set; }
        public ICommand DockSystemsCommand { get; private set; }

        #endregion

        #region Support Methods        

        /// <summary>
        /// Called when [systems are updated].
        /// </summary>
        /// <param name="system">The system.</param>
        private void OnSystemsUpdated(string system)
        {
            //try
            //{
            //    if (_hyperspinManager.Systems != null)
            //    {
            //        foreach (var systemss in _hyperspinManager.Systems)
            //        {
            //            if (!systemss.Name.Contains("Main Menu"))
            //            {
            //                 .Add(new MainMenu()
            //                {
            //                    Name = systemss.Name,
            //                    SysIcon = systemss.SysIcon,
            //                    Enabled = 1
            //                });
            //            }
            //        }                    
            //    }
            //}
            //catch (Exception) { }

        }

        private void OnSelectionChanged(IList items)
        {
            if (items == null)
            {
                _selectedService.SelectedGames.Clear();
                return;
            }

            try
            {
                _selectedService.SelectedGames.Clear();
                foreach (var item in items)
                {
                    var game = item as GameSearch;

                    if (game.Game.RomName != null)
                        _selectedService.SelectedGames.Add(new GameItemViewModel(game.Game));
                };
            }
            catch (Exception)
            {


            }
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

