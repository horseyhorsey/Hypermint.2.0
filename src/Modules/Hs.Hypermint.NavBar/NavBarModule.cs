using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.NavBar
{
    public class NavBarModule : PrismBaseModule
    {

        IRegionManager _regionManager;

        public NavBarModule(IUnityContainer container, IRegionManager manager)
            : base(container, manager)
        {
            _regionManager = manager;
            //RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.MediaPaneView));
            
        }

        public override void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.ToolBarRegion, typeof(Views.NavBarView));
        }
    }
}