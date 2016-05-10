using Hs.HyperSpin.Database;
using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hs.Hypermint.Search.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        public DelegateCommand<string> SearchGamesCommand { get; private set; }

        public SearchViewModel()
        {

            SearchGamesCommand = new DelegateCommand<string>(async x =>
                {
                    await ScanForGamesAsync();
                });
        }

        /// <summary>
        /// Method to scan from button inside the custom search dialog
        /// </summary>
        /// <returns></returns>
        private async Task ScanForGamesAsync()
        {

            try
            {

            }
            catch (Exception) { }

        }
    }
}
