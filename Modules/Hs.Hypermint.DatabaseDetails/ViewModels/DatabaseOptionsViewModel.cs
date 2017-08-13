using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabaseOptionsViewModel : ViewModelBase
    {
        #region Constructors
        public DatabaseOptionsViewModel(ISelectedService selectedService,
            IGameRepo gameRepo, IEventAggregator ea)
        {
            _selectedService = selectedService;
            _gameRepo = gameRepo;
            _eventAggregator = ea;

            ApplyToCellsCommand = new DelegateCommand(ApplyToCells);

            ReplaceDescriptionCommand = new DelegateCommand(ReplaceDescriptions);

        }
        #endregion

        #region Properties
        private string applyString;
        public string ApplyString
        {
            get { return applyString; }
            set { SetProperty(ref applyString, value); }
        }

        private ComboBoxItem selectedItem;
#warning Use another way to bind,  not relying on a Comboboxitem in the ViewModel !!!!THis is easy what are you doing??
        public ComboBoxItem SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }

        private string _pattern;
        /// <summary>
        /// Gets or sets the pattern to search in a description.
        /// </summary>
        public string Pattern
        {
            get { return _pattern; }
            set { SetProperty(ref _pattern, value); }
        }

        private string _replacement;
        /// <summary>
        /// Gets or sets the text to replace for a game description
        /// </summary>
        public string Replacement
        {
            get { return _replacement; }
            set { SetProperty(ref _replacement, value); }
        }

        #endregion

        #region Fields
        private ISelectedService _selectedService;
        private IGameRepo _gameRepo;
        private IEventAggregator _eventAggregator;
        #endregion

        #region Commands
        public DelegateCommand ApplyToCellsCommand { get; private set; }
        public DelegateCommand ReplaceDescriptionCommand { get; private set; }        

        #endregion

        #region Support Methods
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

        /// <summary>
        /// Replace all selected games descriptions from view models Pattern and Replacement properties.        
        /// </summary>       
        private void ReplaceDescriptions()
        {
#warning Check that when renaming descriptions it doesn't break anything for frontend or launcher
            if (_selectedService.SelectedGames != null && _selectedService.SelectedGames.Count > 0)
            {
                try
                {
                    _selectedService.SelectedGames.ForEach((game) =>
                    {
                        var index = _gameRepo.GamesList.IndexOf(game);

                        _gameRepo.GamesList.ElementAt(index).Description = game.Description.Replace(Pattern, Replacement);

                    });
                }
                catch (Exception) { }

                //Tell the main viewmodel we're done
                _eventAggregator.GetEvent<MultipleCellsUpdated>().Publish("");
            }                
        }
        #endregion

    }
}
