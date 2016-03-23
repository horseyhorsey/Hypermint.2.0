using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabaseOptionsViewModel : ViewModelBase
    {
        #region Services
        private ISelectedService _selectedService;
        private IGameRepo _gameRepo;
        #endregion

        #region Commands
        public DelegateCommand ApplyToCellsCommand { get; private set; }

        #endregion

        private string applyString;
        public string ApplyString
        {
            get { return applyString; }
            set { SetProperty(ref applyString, value); }
        }

        private IEventAggregator _eventAggregator;

        private ComboBoxItem selectedItem;
        public ComboBoxItem SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }

        public DatabaseOptionsViewModel(ISelectedService selectedService,
            IGameRepo gameRepo, IEventAggregator ea)
        {
            _selectedService = selectedService;
            _gameRepo = gameRepo;
            _eventAggregator = ea;

            ApplyToCellsCommand = new DelegateCommand(ApplyToCells);
        }

        /// <summary>
        /// Apply a given text to the selected cells column
        /// </summary>
        private void ApplyToCells()
        {

            if (_selectedService.SelectedGames != null && _selectedService.SelectedGames.Count > 0)
            {
                foreach (var item in _selectedService.SelectedGames)
                {
                    var index = _gameRepo.GamesList.IndexOf(item);

                    switch (SelectedItem.Content.ToString())
                    {
                        case "Genre":
                            _gameRepo.GamesList.ElementAt(index).Genre = ApplyString;
                            break;
                        case "Manufacturer":
                            _gameRepo.GamesList.ElementAt(index).Manufacturer = ApplyString;
                            break;
                        case "Rating":
                            _gameRepo.GamesList.ElementAt(index).Rating = ApplyString;
                            break;
                        case "Year":
                            try { _gameRepo.GamesList.ElementAt(index).Year = Convert.ToInt32(ApplyString); }
                            catch (Exception) { }
                            break;
                        default:
                            break;
                    }

                }
            }

            //Tell the main viewmodel we're done
            _eventAggregator.GetEvent<MultipleCellsUpdated>().Publish("");

        }
    }
}
