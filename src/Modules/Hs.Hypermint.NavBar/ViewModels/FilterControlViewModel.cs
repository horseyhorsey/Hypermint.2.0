using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Models;
using Prism.Events;
using System.Collections.Generic;

namespace Hs.Hypermint.NavBar.ViewModels
{
    public class FilterControlViewModel : ViewModelBase
    {        
        private IEventAggregator _eventAggregator;

        #region Properties
        private string filterText;
        public string FilterText
        {
            get { return filterText; }
            set
            {
                SetProperty(ref filterText, value);
                OnPropertyChanged(() => FilterText);
            }
        }

        private bool showClones = true;
        public bool ShowClones
        {
            get { return showClones; }
            set
            {
                SetProperty(ref showClones, value);
                OnPropertyChanged(() => ShowClones);
            }
        }

        private bool showFavoritesOnly;
        public bool ShowFavoritesOnly
        {
            get { return showFavoritesOnly; }
            set { SetProperty(ref showFavoritesOnly, value); }
        }
        #endregion

        public FilterControlViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Publish the filter text to the details view
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged(string propertyName)
        {
            //base.OnPropertyChanged(propertyName);
            //Add the options to a dictionary passed to the filter
            var filterOptions = new GameFilter
            {
                FilterText = FilterText,
                ShowClones = ShowClones,
                ShowFavoritesOnly = ShowFavoritesOnly
            };            

            _eventAggregator.GetEvent<GameFilteredEvent>().Publish(filterOptions);            
                        
        }
    }
    
}
