using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _regionManager.RequestNavigate("ContentRegion", uri);
        }
    }
}
