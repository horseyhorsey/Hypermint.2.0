using Hs.HyperSpin.Database;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System;
using Hypermint.Base.Services;

namespace Hs.Hypermint.MultiSystem.ViewModels
{
    public class MultiSystemViewModel : ViewModelBase
    {

        #region Properties
        private string message = "Test Message";
        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }

        private ICollectionView multiSystemList;
        public ICollectionView MultiSystemList
        {
            get { return multiSystemList; }
            set { SetProperty(ref multiSystemList, value); }
        }

        private string multiSystemName;
        public string MultiSystemName
        {
            get { return multiSystemName; }
            set { multiSystemName = value; }
        }

        private string settingsTemplate;
        public string SettingsTemplate
        {
            get { return settingsTemplate; }
            set { SetProperty(ref settingsTemplate, value); }
        }

        private bool createLinks;
        public bool CreateLinks
        {
            get { return createLinks; }
            set { SetProperty(ref createLinks, value); }
        }

        private bool createGenres;
        public bool CreateGenres
        {
            get { return createGenres; }
            set { SetProperty(ref createGenres, value); }
        }

        private bool createRomMap;
        public bool CreateRomMap
        {
            get { return createRomMap; }
            set { SetProperty(ref createRomMap, value); }
        }

        private bool defaultTheme;
        public bool DefaultTheme
        {
            get { return defaultTheme; }
            set { SetProperty(ref defaultTheme, value); }
        }

        private bool createSymbolicLinks;
        public bool CreateSymbolicLinks
        {
            get { return createSymbolicLinks; }
            set { SetProperty(ref createSymbolicLinks, value); }
        }

        #endregion

        #region Constructors
        public MultiSystemViewModel(IEventAggregator ea, IMultiSystemRepo multiSystem, IFileFolderService fileService,
          ISettingsRepo settings)
        {
            _eventAggregator = ea;
            _multiSystemRepo = multiSystem;
            _fileFolderService = fileService;
            _settingsService = settings;

            _eventAggregator.GetEvent<AddToMultiSystemEvent>().Subscribe(AddToMultiSystem);

            RemoveGameCommand = new DelegateCommand<Game>(RemoveFromMultisystemList);

            ClearListCommand = new DelegateCommand(() =>
            {
                if (_multiSystemRepo.MultiSystemList != null)
                    _multiSystemRepo.MultiSystemList.Clear();
            });

            SelectSettingsCommand = new DelegateCommand(SelectSettings);
        }
        #endregion

        #region Commands
        private IEventAggregator _eventAggregator;
        public DelegateCommand<Game> RemoveGameCommand { get; set; }
        public DelegateCommand ClearListCommand { get; private set; }
        public DelegateCommand SelectSettingsCommand { get; private set; }
        #endregion

        #region Services
        private IFileFolderService _fileFolderService;
        private ISettingsRepo _settingsService;
        private IMultiSystemRepo _multiSystemRepo;
        #endregion

        #region Methods
        private void SelectSettings()
        {
            var hsPath = _settingsService.HypermintSettings.HsPath;
            SettingsTemplate = _fileFolderService.setFileDialog(hsPath);
        }
        /// <summary>
        /// Remove a single item when X is clicked for a game
        /// </summary>
        /// <param name="game"></param>
        private void RemoveFromMultisystemList(Game game)
        {
            _multiSystemRepo.MultiSystemList.Remove(game);
        }
        /// <summary>
        /// Add to a multisystem list from the main database menu event
        /// </summary>
        /// <param name="games"></param>
        private void AddToMultiSystem(object games)
        {
            if (_multiSystemRepo.MultiSystemList == null)
            {
                _multiSystemRepo.MultiSystemList = new Games();
                MultiSystemList = new ListCollectionView(_multiSystemRepo.MultiSystemList);
            }

            foreach (var game in (List<Game>)games)
            {
                _multiSystemRepo.MultiSystemList.Add(game);
            }


        }
        #endregion

    }
}
