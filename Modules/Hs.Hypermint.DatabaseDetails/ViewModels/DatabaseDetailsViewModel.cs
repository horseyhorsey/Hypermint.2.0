﻿using Hypermint.Base.Interfaces;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Windows.Data;
using Prism.Events;
using Hypermint.Base;
using System.Collections.Generic;
using Hypermint.Base.Services;
using System.Collections;
using Hypermint.Base.Models;
using MahApps.Metro.Controls.Dialogs;
using Hypermint.Base.Events;
using System.Threading.Tasks;
using System.Windows.Input;
using Frontends.Models.Hyperspin;
using Hypermint.Base.Model;
using System.Linq;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabaseDetailsViewModel : ViewModelBase
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private IDialogCoordinator _dialogService;
        private IHyperspinManager _hyperspinManager;
        private ISelectedService _selectedService;
        #endregion

        #region Constructors
        public DatabaseDetailsViewModel()
        {

        }

        public DatabaseDetailsViewModel(ISelectedService selectedService, IEventAggregator eventAggregator,
            IDialogCoordinator dialogService, IHyperspinManager hyperspinManager)
        {
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _hyperspinManager = hyperspinManager;
            _selectedService = selectedService;

            ScanRomsCommand = new DelegateCommand(ScanRoms);            

            //Events                                   
            _eventAggregator.GetEvent<SaveMainMenuEvent>().Subscribe(SaveCurrentMainMenuItems);            
        }

        #endregion

        #region Commands        
        public ICommand AuditScanStart { get; private set; }        
        public ICommand ScanRomsCommand { get; private set; }
        #endregion

        #region Support Methods

        [Obsolete]
        private Task updateFavoritesForGamesList()
        {
            return null;
            //if (_selectedService != null)
            //{
            //    await Task.Run(() =>
            //    {
            //        try
            //        {
            //            var selectedSystemName = _selectedService.CurrentSystem;

            //            //var favesList = _favouriteService.GetFavoritesForSystem
            //            //    (selectedSystemName, _settingsRepo.HypermintSettings.HsPath);

            //            foreach (var game in _hyperspinManager.CurrentSystemGames)
            //            {
            //                if (favesList.Contains(game.RomName))
            //                    game.IsFavorite = true;
            //            }
            //        }
            //        catch (Exception) { throw; }
            //    });

            //}

        }

        [Obsolete]
        private void ScanRoms()
        {
            //if (!Directory.Exists(_hyperspinManager._hyperspinFrontEnd.Path)) return;

            //try
            //{
            //    if (!_selectedService.CurrentSystem.ToLower().Contains("main menu"))
            //    {
            //        //_gameRepo.ScanForRoms(
            //        //    _settingsRepo.HypermintSettings.RlPath,
            //        //    _selectedService.CurrentSystem);
            //    }

            //    if (GamesList != null)
            //        GamesList.Refresh();
            //}
            //catch (Exception ex)
            //{

            //}

        }

        [Obsolete("Shouldnt be savin main menu items here")]
        private void SaveCurrentMainMenuItems(string xml)
        {
            //if (_menuRepo.Systems != null || _menuRepo.Systems.Count > 0)
            //    _xmlService.SerializeMainMenuXml(
            //        _menuRepo.Systems, _settingsRepo.HypermintSettings.HsPath, xml);
        }

        #endregion
    }
}
