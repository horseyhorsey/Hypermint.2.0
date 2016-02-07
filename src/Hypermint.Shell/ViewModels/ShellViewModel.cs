using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace Hypermint.Shell.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        public DelegateCommand<string> NavigateCommand { get; set; }

        private readonly IRegionManager _regionManager;        

        public ShellViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(Navigate);            
        }

        private void Navigate(string uri)
        {            
            
        }
    }
}
