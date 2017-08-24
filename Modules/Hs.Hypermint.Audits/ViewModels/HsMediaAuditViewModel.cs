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
    public class HsMediaAuditViewModel : HyperMintModelBase, IGamesList
    {
        #region Fields                
        private IGameRepo _gameRepo;
        private ISearchYoutube _youtube;
        private IDialogCoordinator _dialogService;
        private IHyperspinManager _hyperspinManager;
        #endregion

        //IAuditer auditer
        #region Constructor
        public HsMediaAuditViewModel(ISettingsHypermint settings, IHyperspinManager hsManager, IGameRepo gameRepo,
            IEventAggregator eventAggregator, IAuditer auditer,
            IDialogCoordinator dialogService,
            ISelectedService selectedService, IGameLaunch gameLaunch,
            ISearchYoutube youtube) : base(eventAggregator, selectedService, gameLaunch, settings)
        {

            _gameRepo = gameRepo;
            _youtube = youtube;
            _dialogService = dialogService;
            _hyperspinManager = hsManager;

            CurrentCellChanged = 
                new DelegateCommand<object>( selectedGameCell => { SelectedGameCellChanged(selectedGameCell);});            

            SearchYoutubeCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<GetVideosEvent>().Publish(new object());
                _eventAggregator.GetEvent<NavigateRequestEvent>().Publish("YoutubeView");
            });

            // Runs the auditer for hyperspin
            RunAuditCommand = new DelegateCommand( async () => await RunScan());

            _eventAggregator.GetEvent<HsAuditUpdateEvent>().Subscribe(a =>
            {
                OnHsAuditUpdateEvent(a);
            });

            //Set the observable game to a collection for the view.
            GamesList = new ListCollectionView(_hyperspinManager.CurrentSystemsGames);
            GamesList.CurrentChanged += GamesList_CurrentChanged;
            //GamesList.GroupDescriptions.Add(new PropertyGroupDescription("RomName"));

            _eventAggregator.GetEvent<GamesUpdatedEvent>().Subscribe(GamesUpdated);
        }

        #endregion

        #region Commands
        public DelegateCommand<object> SelectionChanged { get; set; }
        public DelegateCommand<object> CurrentCellChanged { get; set; }
        public ICommand SearchYoutubeCommand { get; private set; }
        public ICommand RunAuditCommand { get; private set; }
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

        private GameItemViewModel selectedGame;
        public GameItemViewModel SelectedGame
        {
            get { return selectedGame; }
            set { SetProperty(ref selectedGame, value); }
        }

        public ICollectionView GamesList { get; set; }
        #endregion

        #region Public Methods

        ///// <summary>
        ///// Adds a game to the multisystem via the event aggregator.
        ///// </summary>
        //public override void AddToMultiSystem()
        //{
        //    _eventAggregator.GetEvent<AddToMultiSystemEvent>().Publish(new List<GameItemViewModel>() { new GameItemViewModel(_selectedGame) });
        //}

        #endregion

        #region Support Methods        

        private void GamesUpdated(string systemName)
        {
            if (GamesList != null)
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

        /// <summary>
        /// Handles the CurrentChanged event ICollectionView
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void GamesList_CurrentChanged(object sender, EventArgs e)
        {            
            PublishCurrentGame();            
        }

        /// <summary>
        /// Called when [hs audit update event].
        /// </summary>
        /// <param name="a">a.</param>
        private void OnHsAuditUpdateEvent(object a)
        {
            var nameAndType = (string[])a;

            UpdateAuditValuesAfterDrop(nameAndType);

            GamesList.Refresh();
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
#warning handle this
                catch { }

            }
        }

        /// <summary>
        /// Publishes the current selected game to change the artwork.
        /// </summary>
        private void PublishCurrentGame()
        {
            if (_hyperspinManager.CurrentSystemsGames.Count > 0 && GamesList != null)
            {
                var game = GamesList.CurrentItem as GameItemViewModel;

                if (game != null)
                {
                    _selectedService.SelectedGames.Clear();
                    _selectedService.SelectedGames.Add(game);
                    _eventAggregator.GetEvent<GameSelectedEvent>().Publish(new string[] { game.Name, CurrentColumnHeader });
                }
            }
        }

        private async Task RunScan(string option = "")
        {
            try
            {
                if (_selectedService.CurrentSystem.ToLower().Contains("main menu"))
                {
                    await _hyperspinManager.AuditMedia(_selectedService.CurrentSystem);

                    GamesList.Refresh();
                }
                else
                {

                    await _hyperspinManager.AuditMedia(_selectedService.CurrentSystem);

                    GamesList.Refresh();
                }
            }
            catch (Exception ex)
            {

            }

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

        private void SetAuditGameFilter()
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(GamesList);

            cv.Filter = o =>
            {
                var g = o as GameItemViewModel;

                if (_selectedService.IsMainMenu())
                    return g.RomName.ToUpper().Contains(FilterText.ToUpper());
                else
                    return g.Description.ToUpper().Contains(FilterText.ToUpper()); ;
            };
        }

        private void SetMessage(string obj)
        {
            Message = obj;
        }

        /// <summary>
        /// Selecteds the game cell. Need this to tell which column we're in
        /// </summary>
        /// <param name="selectedGameCell">The selected game cell.</param>
        private void SelectedGameCellChanged(object selectedGameCell)
        {
            string romName = "";

            try
            {
                var column = selectedGameCell as DataGridTextColumn;
                if (column != null)
                {
                    CurrentColumnType = column.SortMemberPath;
                    CurrentColumnHeader = column.Header.ToString();
                    MediaAuditHeaderInfo = "Hyperspin Media Audit : " + romName + " : " + CurrentColumnHeader;

                    PublishCurrentGame();
                }

            }
            catch (Exception ex) { }
        }

        private void UpdateAuditValuesAfterDrop(string[] romAndType)
        {
            var game = _hyperspinManager.CurrentSystemsGames
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

        #endregion

    }
}
