using Hypermint.Base.Base;
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

namespace Hs.Hypermint.Search.ViewModels
{
    public class SearchViewModel : HyperMintModelBase
    {
        #region Constructors
        public SearchViewModel(
            IEventAggregator eventAggregator,
            ISettingsRepo settings,            
            ISelectedService selectedSrv,
            IGameLaunch gameLaunch) : base(eventAggregator, selectedSrv, gameLaunch, settings)
        {            

            SelectSystemsCommand = new DelegateCommand<string>(x =>
            {
                try
                {
                    var sysEnabled = 0;

                    if (x == "all")
                        sysEnabled = 1;

                    foreach (MainMenu item in _systems)
                    {
                        item.Enabled = sysEnabled;
                    }

                    Systems.Refresh();

                }
                catch (Exception) { }

            });

            // Command for datagrid selectedItems
            SelectionChanged = new DelegateCommand<IList>(
                items =>
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
                                _selectedService.SelectedGames.Add(game.Game);
                        };
                    }
                    catch (Exception)
                    {


                    }

                });

            DockSystemsCommand = new DelegateCommand(() => { SystemsVisible = !SystemsVisible; });

            _eventAggregator.GetEvent<SystemsGenerated>().Subscribe(x => OnSystemsUpdated(x));
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

        #region Fields               
        private IMainMenuRepo _mainmenuRepo;
        private Systems _systems;

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
            _systems = new Systems();

            try
            {
                if (_mainmenuRepo.Systems != null)
                {
                    foreach (var item in _mainmenuRepo.Systems)
                    {
                        if (!item.Name.Contains("Main Menu"))
                        {
                            _systems.Add(new MainMenu()
                            {
                                Name = item.Name,
                                SysIcon = item.SysIcon,
                                Enabled = 1
                            });
                        }
                    }

                    Systems = new ListCollectionView(_systems);
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

        public string SystemImage { get; set; }
    }

    #endregion   
}

