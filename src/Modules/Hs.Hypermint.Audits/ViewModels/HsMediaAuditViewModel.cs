﻿using Hs.HyperSpin.Database.Audit;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Hypermint.Base.Events;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class HsMediaAuditViewModel : ViewModelBase
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ISettingsRepo _settings;
        private IGameRepo _gameRepo;
        private IAuditer _auditer;
        private ISearchYoutube _youtube;

        public DelegateCommand<object> SelectionChanged { get; set; }
        public DelegateCommand<object> CurrentCellChanged { get; set; }        
        private ISelectedService _selectedService;
        public DelegateCommand SearchYoutubeCommand { get; private set; }
        public DelegateCommand RunAuditCommand { get; private set; }
    
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

        public AuditGame SelectedGame { get; set; }
        public AuditMenu SelectedMenu { get; set; }

        private ICollectionView _auditList;
        public ICollectionView AuditList
        {
            get { return _auditList; }
            set { SetProperty(ref _auditList, value); }
        }
        #endregion

        #region ctors
        public HsMediaAuditViewModel(ISettingsRepo settings, IGameRepo gameRepo,
            IEventAggregator eventAggregator, IAuditer auditer, 
            ISelectedService selectedService,
            ISearchYoutube youtube)
        {
            _eventAggregator = eventAggregator;
            _settings = settings;
            _auditer = auditer;
            _auditer.AuditsGameList = new AuditsGame();
            _gameRepo = gameRepo;
            _selectedService = selectedService;
            _youtube = youtube;

            _eventAggregator.GetEvent<GamesUpdatedEvent>().Subscribe(GamesUpdated);
            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(GamesUpdated);

            CurrentCellChanged = new DelegateCommand<object>(
                selectedGameCell =>
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
                        else {
                            romName = SelectedGame.RomName;
                            description = SelectedGame.Description;
                            _selectedService.CurrentRomname = romName;
                            _selectedService.CurrentDescription = description;
                        }

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
                    catch (Exception e) { }

                });            

            GamesUpdated("Main Menu");

            SearchYoutubeCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<GetVideosEvent>().Publish(new object());
                _eventAggregator.GetEvent<NavigateRequestEvent>().Publish("YoutubeView");
            });

            // Run the auditer for hyperspin
            RunAuditCommand = new DelegateCommand(() => RunScan());
        }

        #endregion

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "FilterText")
            {
                SetAuditGameFilter();
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

        private void RunScan(string option="")
        {
            var hsPath = _settings.HypermintSettings.HsPath;

            FilterText = "";

            try
            {
                if (AuditList != null && Directory.Exists(hsPath))
                {
                    var systemName = _selectedService.CurrentSystem;

                    if (systemName.ToLower().Contains("main menu"))
                    {
                        _auditer.ScanForMediaMainMenu(
                            hsPath, _gameRepo.GamesList);

                        AuditList = new ListCollectionView(_auditer.AuditsMenuList);
                    }
                    else
                    {
                        _auditer.ScanForMedia(
                            hsPath, systemName, _gameRepo.GamesList
                             );

                        AuditList = new ListCollectionView(_auditer.AuditsGameList);
                    }

                }

                _eventAggregator.GetEvent<AuditHyperSpinEndEvent>().Publish("");

            }
            catch (Exception) { }

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

            if (_gameRepo.GamesList != null)
            {
                _auditer.AuditsGameList.Clear();
            
                    foreach (var item in _gameRepo.GamesList)
                    {
                        _auditer.AuditsGameList.Add(new AuditGame
                        {
                            RomName = item.RomName,
                            Description = item.Description
                        });
                    }

                    AuditList = new ListCollectionView(_auditer.AuditsGameList);
                
            }
        }

        private void SetMessage(string obj)
        {
            Message = obj;
        }

    }
}
