using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Models;
using Prism.Events;
using System.Collections.Generic;
using System;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class FilterControlViewModel : ViewModelBase
    {        
        private IEventAggregator _eventAggregator;

        private bool systemChanging;
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

        private bool showEnabledOnly;
        public bool ShowEnabledOnly
        {
            get { return showEnabledOnly; }
            set { SetProperty(ref showEnabledOnly, value); }
        }
        #endregion

        public FilterControlViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(RemoveFilters);
        }

        private void RemoveFilters(string obj)
        {
            systemChanging = true;
            FilterText = "";
            ShowClones = true;
            ShowFavoritesOnly = false;
            ShowEnabledOnly = false;
            systemChanging = false;
        }

        /// <summary>
        /// Publish the filter text to the details view
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (!systemChanging)
            {
                //Add the options to a dictionary passed to the filter
                var filterOptions = new GameFilter
                {
                    FilterText = FilterText,
                    ShowClones = ShowClones,
                    ShowFavoritesOnly = ShowFavoritesOnly,
                    ShowEnabledOnly = ShowEnabledOnly
                };

                _eventAggregator.GetEvent<GameFilteredEvent>().Publish(filterOptions);
            }         
                        
        }
    }
    
}
