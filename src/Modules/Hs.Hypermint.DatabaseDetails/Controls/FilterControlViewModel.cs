using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hs.Hypermint.DatabaseDetails.Controls
{
    public class FilterControlViewModel : ViewModelBase
    {        
        private IEventAggregator _eventAggregator;

        private string filterText;
        public string FilterText
        {
            get { return filterText; }
            set {
                SetProperty(ref filterText, value);
                OnPropertyChanged(() => FilterText);
                }
        }

        private bool showClones = true;
        public bool ShowClones
        {
            get { return showClones; }
            set {
                SetProperty(ref showClones, value);
                OnPropertyChanged(() => ShowClones);
            }
        }

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
            var filterOptions = new Dictionary<string, bool>();
            filterOptions.Add(FilterText, ShowClones);

            _eventAggregator.GetEvent<GameFilteredEvent>().Publish(filterOptions);            
                        
        }
    }
}
