using Hypermint.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Model;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabaseOptionsViewModel : ViewModelBase
    {
        #region Fields
        private ISelectedService _selectedService;
        private IEventAggregator _eventAggregator;
        private IHyperspinManager _hyperspinManager;
        #endregion

        #region Constructors
        /// <summary>
        /// This view model applies the given value to multiple rows
        /// </summary>
        /// <param name="selectedService">The selected service.</param>
        /// <param name="ea">The ea.</param>
        public DatabaseOptionsViewModel(IHyperspinManager hyperspinManager,
            ISelectedService selectedService, IEventAggregator ea)
        {
            _selectedService = selectedService;
            _eventAggregator = ea;
            _hyperspinManager = hyperspinManager;

            ApplyToCellsCommand = new DelegateCommand(ApplyToCells);
            ReplaceDescriptionCommand = new DelegateCommand(ReplaceDescriptions);
        }

        #endregion

        #region Commands
        public ICommand ApplyToCellsCommand { get; private set; }
        public ICommand ReplaceDescriptionCommand { get; private set; }
        #endregion

        #region Properties
        private string applyString;
        public string ApplyString
        {
            get { return applyString; }
            set { SetProperty(ref applyString, value); }
        }

        private string selectedItem;
        public string SelectedItem
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

        #region Support Methods

        /// <summary>
        /// Apply a given text to the selected rows cell
        /// </summary>
        private void ApplyToCells()
        {
            if (_selectedService.SelectedGames != null && _selectedService.SelectedGames.Count > 0)
            {
                try
                {
                    UserRequestRowMessage msg = null;

                    switch (SelectedItem)
                    {
                        case "Description":
                            msg = new UserRequestRowMessage(_selectedService.SelectedGames, RowUpdateType.Description, ApplyString);
                            break;
                        case "Genre":
                            msg = new UserRequestRowMessage(_selectedService.SelectedGames, RowUpdateType.Genre, ApplyString);
                            break;
                        case "Manufacturer":
                            msg = new UserRequestRowMessage(_selectedService.SelectedGames, RowUpdateType.Manufacturer, ApplyString);
                            break;
                        case "Rating":
                            msg = new UserRequestRowMessage(_selectedService.SelectedGames, RowUpdateType.Rating, ApplyString);
                            break;
                        case "Year":
                            msg = new UserRequestRowMessage(_selectedService.SelectedGames, RowUpdateType.Year, ApplyString);
                            break;
                        default:
                            return;
                    }

                    _eventAggregator.GetEvent<UserRequestUpdateSelectedRows>().Publish(msg);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Replace all selected games descriptions from view models Pattern and Replacement properties.        
        /// </summary>              
        private void ReplaceDescriptions()
        {
            //#warning Check that when renaming descriptions it doesn't break anything for frontend or launcher
            if (_selectedService.SelectedGames != null && _selectedService.SelectedGames.Count > 0)
            {
                try
                {
                    _selectedService.SelectedGames.ForEach((game) =>
                    {
                        var index = _hyperspinManager.CurrentSystemsGames.IndexOf(game);

                        _hyperspinManager.CurrentSystemsGames[index].Description = game.Description.Replace(Pattern, Replacement);

                    });
                }
                catch (Exception) { }
            }
        }

        #endregion
    }
}
