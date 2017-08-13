using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

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

            RegionManager.RegisterViewWithRegion(RegionNames.MediaPaneRegion, typeof(Views.BezelEditView));

        }

        public override void Initialize()
        {
            
        }
    }
}