using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.SidebarSystems
{
    public class SidebarSystemsModule : PrismBaseModule
    {
        public SidebarSystemsModule(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {            
            
        }

        public override void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarSystemsView));
            RegionManager.RegisterViewWithRegion(RegionNames.SystemsRegion, typeof(SystemsView));
        }

    }
}
