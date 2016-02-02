using Hypermint.Base;
using Hypermint.Base.Base;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hypermint.Modules.HyperSpin
{
    public class SidebarModule : PrismBaseModule
    {
        public SidebarModule(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {
           // UnityContainer.RegisterType<ILotteryRepo, Hypermint.Modules.HyperSpin.Models.LotteryRepo>();
            RegionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarView));
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(SelectedFolderBrowserView));
            //RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(MediaViewer));
        }

        public override void Initialize()
        { 
            //RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(SidebarView));
            //UnityContainer.RegisterType<SidebarView>();

        }

    }
}
