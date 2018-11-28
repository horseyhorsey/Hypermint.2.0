using Hypermint.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.MultiSystem
{
    public class MultiSystemModule : PrismBaseModule
    {

        IRegionManager _regionManager;

        public MultiSystemModule(IUnityContainer container, IRegionManager manager)
            : base(container, manager)
        {
           _regionManager = manager;

        }

        public override void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.MultiSystemView));
            RegionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(Views.MultiSystemOptionsView));
        }
    }
}