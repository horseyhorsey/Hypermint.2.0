using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.Audits
{
    public class AuditsModule : PrismBaseModule
    {
        IRegionManager _regionManager;

        public AuditsModule(IUnityContainer container, IRegionManager manager)
                : base(container, manager)
        {

        }

        public override void Initialize()
        {            
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.HsMediaAuditView));
        }
    }
}