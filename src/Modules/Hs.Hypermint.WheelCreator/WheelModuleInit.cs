using Hs.Hypermint.WheelCreator.Views;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.WheelCreator
{
    public class WheelModuleInit : PrismBaseModule
    {
        public WheelModuleInit(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {                        
        }

        public override void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(SimpleWheelView));

            RegionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(WheelProcessView));
        }

    }
}
