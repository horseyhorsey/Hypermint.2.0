using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Services;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;

namespace Hs.Hypermint.MediaPane
{
    public class MediaPaneModule : PrismBaseModule
    {
        IRegionManager _regionManager;

        public MediaPaneModule(IUnityContainer container, IRegionManager manager)
            : base(container, manager)
        {
            _regionManager = manager;
            RegionManager.RegisterViewWithRegion(RegionNames.MediaPaneRegion, typeof(Views.MediaPaneView));
        }

        public override void Initialize()
        {
            
        }
    }
}