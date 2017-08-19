using Hs.Hypermint.WheelCreator.Views;
using Hypermint.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.WheelCreator
{
    public class WheelModuleInit : PrismBaseModule
    {
        public WheelModuleInit(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {            
            manager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(WheelProcessView));

            manager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(SimpleWheelView));
        }

        public override void Initialize()
        {

        }
    }
}
