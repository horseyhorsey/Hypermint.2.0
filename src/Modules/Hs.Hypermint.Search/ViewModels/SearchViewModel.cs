using Hs.HyperSpin.Database;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Hypermint.Base.Services;
using Prism.Events;
using Hypermint.Base;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Threading;

namespace Hs.Hypermint.Search.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        public DelegateCommand<string> SearchGamesCommand { get; }
        public DelegateCommand<string> SelectSystemsCommand { get; }
        public DelegateCommand CancelCommand { get;} 

        private IMainMenuRepo _mainmenuRepo;

        #region Properties

        CancellationTokenSource tokenSource = new CancellationTokenSource();

        private Systems _systems = new Systems();

        private ICollectionView systems;
        private IEventAggregator _eventAggregator;
        private ISettingsRepo _settings;

        public ICollectionView Systems
        {
            get { return systems; }
            set { SetProperty(ref systems, value); }
        }

        private List<object> searchedGames;
        private ICollectionView foundGmes;
        private bool canSearch = true;

        public ICollectionView FoundGmes
        {
            get { return foundGmes; }
            set { SetProperty(ref foundGmes, value); }
        }
        #endregion

        public SearchViewModel(IMainMenuRepo mainMenu, IEventAggregator eventAggregator, ISettingsRepo settings)
        {
            _mainmenuRepo = mainMenu;
            _eventAggregator = eventAggregator;
            _settings = settings;

            SearchGamesCommand = new DelegateCommand<string>(async x =>
            {
                await ScanForGamesAsync(x);
            }, o => canSearch);            

            CancelCommand = new DelegateCommand(() =>
            {
                tokenSource.Cancel();
                canSearch = true;
                SearchGamesCommand.RaiseCanExecuteChanged();
            });

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

            _eventAggregator.GetEvent<SystemsGenerated>().Subscribe(x => SystemsUpdated(x));
        }

        private void SystemsUpdated(string system)
        {
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
                                Enabled = 0
                            });
                        }
                    }

                    Systems = new ListCollectionView(_systems);
                }
            }
            catch (Exception)
            {


            }

        }

        /// <summary>
        /// Method to scan from button inside the custom search dialog
        /// </summary>
        /// <returns></returns>
        private async Task ScanForGamesAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return;            

            tokenSource = new CancellationTokenSource();

            XDocument xdoc = null;

            canSearch = false;
            SearchGamesCommand.RaiseCanExecuteChanged();

            searchedGames = new List<object>();

            await Task.Run(() =>
            {

                foreach (var system in _systems)
                {
                    try
                    {
                        if (system.Enabled == 1)
                        {
                            var xmlPath = Path.Combine(_settings.HypermintSettings.HsPath, "Databases", system.Name, system.Name + ".xml");

                            using (var xr = XmlReader.Create(xmlPath))
                            {
                                xdoc = XDocument.Load(xr);

                                var query =
                                    from g in xdoc.Descendants("game")
                                    where g.Value.ToLower().Contains(searchTerm.ToLower())
                                    select new
                                    {
                                        RomName = g.Attribute("name").Value,
                                        Description = g.Element("description").Value,
                                        Year = g.Element("year").Value,
                                        System = system.Name,
                                        Genre = g.Element("genre").Value
                                    };

                                foreach (var item in query)
                                {
                                    searchedGames.Add(item);
                                }
                            }

                        }
                        
                    }
                    catch (Exception) { }
                    
                }

                FoundGmes = new ListCollectionView(searchedGames);

            }, tokenSource.Token);


            canSearch = true;
            SearchGamesCommand.RaiseCanExecuteChanged();

        }
    }
}
