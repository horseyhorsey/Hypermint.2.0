using Hypermint.Base;
using Hypermint.Base.Base;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.WheelCreator
{
    public class WheelModuleInit : PrismBaseModule
    {
        public WheelModuleInit(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {
            // UnityContainer.RegisterType<ILotteryRepo, Hs.Hypermint.WheelCreator.Models.LotteryRepo>();
            //RegionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarView));
            //RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(SelectedFolderBrowserView));
            //RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(MediaViewer));
        }

        public override void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(SimpleWheelView));
            //UnityContainer.RegisterType<SidebarView>();

        }

    }
}
