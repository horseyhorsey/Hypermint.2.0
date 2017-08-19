using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Hypermint.Base.Events;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using System.Linq;
using System.Collections.Generic;
using Frontends.Models.Hyperspin;
using System.Windows.Input;
using Hypermint.Base.Model;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class HsMediaAuditViewModel : HyperMintModelBase
    {
        #region Fields                
        private IGameRepo _gameRepo;
        private ISearchYoutube _youtube;
        private IDialogCoordinator _dialogService;
        private IHyperspinManager _hsManager;
        private IAuditer _auditer;
        #endregion

        //IAuditer auditer
        #region Constructor
        public HsMediaAuditViewModel(ISettingsHypermint settings, IHyperspinManager hsManager, IGameRepo gameRepo,
            IEventAggregator eventAggregator, IAuditer auditer,
            IDialogCoordinator dialogService,
            ISelectedService selectedService, IGameLaunch gameLaunch,
            ISearchYoutube youtube) : base(eventAggregator, selectedService, gameLaunch, settings)
        {
#warning 69 HS AUDITER
            //_auditer = auditer;
            //_auditer.AuditsGameList = new AuditsGame();

            _gameRepo = gameRepo;
            _youtube = youtube;
            _dialogService = dialogService;
            _hsManager = hsManager;
            _auditer = auditer;

            CurrentCellChanged = new DelegateCommand<object>(
                selectedGameCell =>
                {
                    SelectedGameCellChanged(selectedGameCell);
                });            

            SearchYoutubeCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<GetVideosEvent>().Publish(new object());
                _eventAggregator.GetEvent<NavigateRequestEvent>().Publish("YoutubeView");
            });

            //Remove not needed??
            //_eventAggregator.GetEvent<GamesUpdatedEvent>().Subscribe(GamesUpdated);
            //_eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(GamesUpdated);

            // Runs the auditer for hyperspin
            RunAuditCommand = new DelegateCommand( async () => await RunScan());

            _eventAggregator.GetEvent<HsAuditUpdateEvent>().Subscribe(a =>
            {
                OnHsAuditUpdateEvent(a);
            });

            _selectedGame = new Game();
            AuditList = new ListCollectionView(_hsManager.CurrentSystemsGames);
            //GamesUpdated("Main Menu");
        }

        #endregion

        #region Commands
        public DelegateCommand<object> SelectionChanged { get; set; }
        public DelegateCommand<object> CurrentCellChanged { get; set; }
        public ICommand SearchYoutubeCommand { get; private set; }
        public ICommand RunAuditCommand { get; private set; }
        #endregion

        #region Properties

        private ICollectionView _auditList;
        public ICollectionView AuditList
        {
            get { return _auditList; }
            set { SetProperty(ref _auditList, value); }
        }

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

        private string mediaAuditHeaderInfo = "Hyperspin Media Audit";
        public string MediaAuditHeaderInfo
        {
            get { return mediaAuditHeaderInfo; }
            set { SetProperty(ref mediaAuditHeaderInfo, value); }
        }

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

        private string filterText;
        public string FilterText
        {
            get { return filterText; }
            set { SetProperty(ref filterText, value); }
        }

        private bool isMainMenu = false;
        public bool IsMainMenu
        {
            get { return isMainMenu; }
            set
            {
                SetProperty(ref isMainMenu, value);

            }
        }

        private bool isntMainMenu = true;
        public bool IsntMainMenu
        {
            get { return isntMainMenu; }
            set
            {
                SetProperty(ref isntMainMenu, value);
            }
        }

        private Game _selectedGame;

        public AuditGame SelectedGame { get; set; }
        public AuditMenu SelectedMenu { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a game to the multisystem via the event aggregator.
        /// </summary>
        public override void AddToMultiSystem()
        {
            _eventAggregator.GetEvent<AddToMultiSystemEvent>().Publish(new List<GameItemViewModel>() { new GameItemViewModel(_selectedGame) });
        }

        #endregion

        #region Support Methods

        private void UpdateAuditValuesAfterDrop(string[] romAndType)
        {
            var game = _auditer.AuditsGameList
                .Where(x => x.RomName == romAndType[0]).Single();

            if (game == null) return;

            switch (romAndType[1])
            {
                case "Artwork1":
                    game.HaveArt1 = true;
                    break;
                case "Artwork2":
                    game.HaveArt2 = true;
                    break;
                case "Artwork3":
                    game.HaveArt3 = true;
                    break;
                case "Artwork4":
                    game.HaveArt4 = true;
                    break;
                case "Wheel":
                    game.HaveWheel = true;
                    break;
                case "Videos":
                    game.HaveVideo = true;
                    break;
                case "Theme":
                    game.HaveTheme = true;
                    break;
                case "Backgrounds":
                    game.HaveBackground = true;
                    break;
                case "MusicBg":
                    game.HaveBGMusic = true;
                    break;
                case "SoundStart":
                    game.HaveS_Start = true;
                    break;
                default:
                    break;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == "FilterText")
            {
                try
                {
                    SetAuditGameFilter();
                }
#warning handle this horse
                catch { }

            }
        }

        private void SetAuditGameFilter()
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(AuditList);

            cv.Filter = o =>
            {
                var g = o as AuditGame;

                if (_selectedService.IsMainMenu())
                    return g.RomName.ToUpper().Contains(FilterText.ToUpper());
                else
                    return g.Description.ToUpper().Contains(FilterText.ToUpper()); ;
            };
        }

        [Obsolete]
        private Task RunScan(string option = "")
        {
            return null;
            //var hsPath = _settingsRepo.HypermintSettings.HsPath;

            //FilterText = "";

            //try
            //{
            //    if (AuditList != null && Directory.Exists(hsPath))
            //    {
            //        var progressResult = await _dialogService.ShowProgressAsync(this, "Scanning Hs media...", "");
            //        progressResult.SetIndeterminate();

            //        var systemName = _selectedService.CurrentSystem;

            //        if (systemName.ToLower().Contains("main menu"))
            //        {
            //            _auditer.ScanForMediaMainMenu(
            //                hsPath, _hsManager.CurrentSystemsGames);
            //        }
            //        else
            //        {

            //            await _auditer.ScanForMediaAsync(
            //                    hsPath, systemName, _hsManager.CurrentSystemsGames);
            //        }

            //        progressResult.SetMessage("Scan complete");

            //        if (systemName.ToLower().Contains("main menu"))
            //            AuditList = new ListCollectionView(_auditer.AuditsMenuList);
            //        else
            //            AuditList = new ListCollectionView(_auditer.AuditsGameList);

            //        await progressResult.CloseAsync();

            //    }

            //    _eventAggregator.GetEvent<AuditHyperSpinEndEvent>().Publish("");
            //}
            //catch (Exception) { throw; }

        }

        private void GamesUpdated(string systemName)
        {
            if (AuditList != null)
                FilterText = "";

            if (systemName.ToLower().Contains("main menu"))
            {
                IsMainMenu = true;
                IsntMainMenu = false;
            }
            else
            {
                IsMainMenu = false;
                IsntMainMenu = true;
            }
        }

        private void SetMessage(string obj)
        {
            Message = obj;
        }

        /// <summary>
        /// Called when [hs audit update event].
        /// </summary>
        /// <param name="a">a.</param>
        private void OnHsAuditUpdateEvent(object a)
        {
            var nameAndType = (string[])a;

            UpdateAuditValuesAfterDrop(nameAndType);

            AuditList.Refresh();
        }

        /// <summary>
        /// On the game cell changed.
        /// </summary>
        /// <param name="selectedGameCell">The selected game cell.</param>
        [Obsolete("This is very bad, needs changing")]
        private void SelectedGameCellChanged(object selectedGameCell)
        {
            string romName = "", description = "";

            try
            {
                //Using reflection to get the underlying DataGrid
                //Current item from the AudiList is one behind on a Cell click
                var datagridProperties =
                selectedGameCell.GetType().GetProperty("DataGridOwner",
                BindingFlags.Instance | BindingFlags.NonPublic).GetValue(selectedGameCell, null);

                var dg = datagridProperties as DataGrid;
                SelectedGame = dg.CurrentItem as AuditGame;

                if (SelectedGame == null)
                {
                    SelectedMenu = dg.CurrentItem as AuditMenu;
                    romName = SelectedMenu.RomName;
                    _selectedService.CurrentRomname = romName;
                }
                else
                {
                    romName = SelectedGame.RomName;
                    description = SelectedGame.Description;
                    _selectedService.CurrentRomname = romName;
                    _selectedService.CurrentDescription = description;
                }

                _selectedGame = new Game
                {
                    RomName = SelectedGame.RomName,
                    Description = SelectedGame.Description,
                };

            }
            catch (Exception) { }

            try
            {
                var column = selectedGameCell as DataGridTextColumn;                
                CurrentColumnType = column.SortMemberPath;
                CurrentColumnHeader = column.Header.ToString();
                MediaAuditHeaderInfo = "Hyperspin Media Audit : " +
                romName + " : " +
                CurrentColumnHeader;

                _eventAggregator.GetEvent<GameSelectedEvent>().Publish(
                new string[] { romName, CurrentColumnHeader }
                );

            }
            catch (Exception) { throw; }
        }
        #endregion        

    }
}
