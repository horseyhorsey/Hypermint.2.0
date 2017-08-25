using Hypermint.Base.Interfaces;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Prism.Commands;
using Hypermint.Base;
using System.Windows.Data;
using Hypermint.Base.Services;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Controls;
using Hypermint.Base.Model;
using Prism.Events;
using MahApps.Metro.Controls.Dialogs;
using Hs.Hypermint.Audits.Views;
using System.Linq;

namespace Hs.Hypermint.Audits.ViewModels
{
    public abstract class RlAuditMediaViewModelBase : ViewModelBase, IGamesList
    {
        protected readonly IEventAggregator _eventAggregator;
        protected readonly IHyperspinManager _hyperspinManager;
        private IHyperspinManager hyperspinManager;
        private IGameLaunch gameLaunch;
        private ISettingsHypermint settings;
        private ISelectedService _selectedService;
        private ISettingsHypermint _settings;
        private IDialogCoordinator _dialogService;

        #region Commands
        public ICommand ScanRlMediaCommand { get; set; }
        public ICommand LaunchRlMode { get; set; }
        public ICommand CurrentCellChanged { get; set; }
        public ICommand OpenScanRocketMediaFolderCommand { get; set; }
        #endregion

        #region Constructors

        public RlAuditMediaViewModelBase()
        {

        }

        public RlAuditMediaViewModelBase(IEventAggregator ea, IHyperspinManager hyperspinManager, IGameLaunch gameLaunch,
            ISettingsHypermint settings, 
            ISelectedService selected, IRlScan rlScan, IDialogCoordinator dialogService)
        {
            _eventAggregator = ea;
            _hyperspinManager = hyperspinManager;
            _selectedService = selected;
            _settings = settings;
            _dialogService = dialogService;

            //Setup the games list.
            GamesList = new ListCollectionView(_hyperspinManager.CurrentSystemsGames);
            GamesList.CurrentChanged += GamesList_CurrentChanged;

            CurrentCellChanged = new DelegateCommand<object>( selectedGameCell => {OnCurrentCellChanged(selectedGameCell);});

            ScanRlMediaCommand = new DelegateCommand(async () => await ScanForMedia());

            LaunchRlMode = new DelegateCommand<string>((x) =>
            {
                gameLaunch.RocketLaunchGameWithMode(
                    settings.HypermintSettings.RlPath,
                    selected.CurrentSystem, selected.CurrentRomname, x);
            });

            OpenScanRocketMediaFolderCommand = new DelegateCommand<string>(OpenScanRocketMediaFolderAsync);
        }

        private void OnCurrentCellChanged(object selectedGameCell)
        {
            OnColumnChanged(selectedGameCell);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the games list.
        /// </summary>
        public ICollectionView GamesList { get; set; }

        private bool _isBusy;
        private string _currentColumnType;
        private string _currentColumnHeader;
        private CustomDialog customDialog;
        private IEventAggregator ea;
        private ISelectedService selected;
        private IRlScan rlScan;

        public string MediaAuditHeaderInfo { get; private set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        #endregion

        /// <summary>
        /// Scans for media.
        /// </summary>
        public abstract Task ScanForMedia();

        public virtual void GamesList_CurrentChanged(object sender, EventArgs e)
        {
            PublishCurrentGame();
        }

        public virtual string OnColumnChanged(object selectedGameCell)
        {
            //_eventAggregator.GetEvent<ClearRlFilesEvent>().Publish("");

            //try
            //{
            //    //Using reflection to get the underlying DataGrid               
            //    var datagridProperties =
            //         selectedGameCell.GetType().GetProperty("DataGridOwner",
            //         BindingFlags.Instance | BindingFlags.NonPublic).GetValue(selectedGameCell, null);

            //    var dg = datagridProperties as DataGrid;
            //    /////SelectedGame = dg.CurrentItem as RocketLaunchAudit;
            //    _selectedService.CurrentRomname = SelectedGame.RomName;

            //    AuditHeader = "Rocketlaunch Audit : "
            //        + _selectedService.CurrentRomname + " Selected ";

            //    _selectedGame = new Game
            //    {
            //        RomName = SelectedGame.RomName,
            //        Description = SelectedGame.Description,
            //    };

            //}
            //catch (Exception) { return; }

            //try
            //{
            //    var column = selectedGameCell as DataGridTextColumn;

            //    CurrentColumnType = column.SortMemberPath;
            //    CurrentColumnHeader = column.Header.ToString();

            //}
            //catch (Exception e) { }

            //_eventAggregator.GetEvent<UpdateFilesEvent>()
            //    .Publish(new string[] { currentColumnHeader, SelectedGame.RomName });

            string romName = "";

            try
            {
                var column = selectedGameCell as DataGridTextColumn;
                if (column != null)
                {
                    _currentColumnType = column.SortMemberPath;
                    _currentColumnHeader = column.Header.ToString();
                    MediaAuditHeaderInfo = "Rocketlauncher Media Audit : " + romName + " : " + _currentColumnHeader;

                    //PublishCurrentGame();
                }

            }
            catch (Exception ex) { }

            return _currentColumnHeader;
        }

        /// <summary>
        /// Opens the scan rocket media folder asynchronous. Creates a customDialog from RLScanMediaFolder view/viewmodel
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void OpenScanRocketMediaFolderAsync(string obj)
        {
            customDialog = new CustomDialog() { Title = obj };

            customDialog.Content = new RlScanMediaFolderView();

            var rlScanFolderVm = new RlScanMediaFolderViewModel(
                    _settings.HypermintSettings.RlMediaPath,
                    _settings.HypermintSettings.HsPath, obj, _selectedService.CurrentSystem, _dialogService, customDialog,
                    _hyperspinManager.CurrentSystemsGames.Select(x => x.Game));

            customDialog.DataContext = rlScanFolderVm;            

            await _dialogService.ShowMetroDialogAsync(this, customDialog);
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
                    _eventAggregator.GetEvent<UpdateFilesEvent>().Publish(new string[] { _currentColumnHeader, game.RomName });
                    //_eventAggregator.GetEvent<GameSelectedEvent>().Publish(new string[] { game.Name, _currentColumnHeader });
                }
            }
        }
    }
}
