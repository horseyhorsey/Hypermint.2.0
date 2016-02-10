using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class IntroVideosViewModel : BindableBase
    {
        private IRegionManager _regionManager;

        public IntroVideosViewModel(IRegionManager manager)
        {
            _regionManager = manager;

        }
    }
}
