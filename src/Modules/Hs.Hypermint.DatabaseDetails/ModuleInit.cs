using Hs.Hypermint.Services;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.DatabaseDetails
{
    public class ModuleInit : PrismBaseModule
    {
        public ModuleInit(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {
            
        }

        public override void Initialize()
        {
            UnityContainer.RegisterType<IGameRepo, GameRepo>();

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(DatabaseDetailsView));
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(HsMediaAuditView));
            
            //RegionManager.RegisterViewWithRegion(RegionNames.ToolBarRegion, typeof(Controls.FilterControl));
            
        }

    }
}
