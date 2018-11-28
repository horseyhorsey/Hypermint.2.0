using Prism.Mvvm;
using System.Threading;

namespace Hypermint.Base.Model
{
    public class SearchOptions : BindableBase
    {
        public CancellationTokenSource tokenSource = new CancellationTokenSource();

        private string _searchString;
        /// <summary>
        /// Gets or sets the search string. Updates the <seealso cref="CanSearch"/> if chars lengeth > 3
        /// </summary>
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                SetProperty(ref _searchString, value);                
            }
        }

        private bool onlyEnabledRomsSearch = true;
        /// <summary>
        /// Gets or sets a value indicating whether [only enabled roms search].
        /// </summary>
        public bool OnlyEnabledRomsSearch
        {
            get { return onlyEnabledRomsSearch; }
            set { SetProperty(ref onlyEnabledRomsSearch, value); }
        }

        private bool cloneSearchOn = true;
        /// <summary>
        /// Gets or sets a value indicating whether [clone search on].
        /// </summary>
        public bool CloneSearchOn
        {
            get { return cloneSearchOn; }
            set { SetProperty(ref cloneSearchOn, value); }
        }
    }
}
