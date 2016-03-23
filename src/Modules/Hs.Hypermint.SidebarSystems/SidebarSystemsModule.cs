using Hs.Hypermint.SidebarSystems.Views;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.SidebarSystems
{
    public class SidebarSystemsModule : PrismBaseModule
    {
        private IRegionManager _regionManager;

        public SidebarSystemsModule(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {
            _regionManager = manager;

            RegionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarView));           
           
        }

        public override void Initialize()
        {

        }

    }
}
