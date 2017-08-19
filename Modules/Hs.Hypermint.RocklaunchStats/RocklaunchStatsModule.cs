using Hypermint.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.RocklaunchStats
{
    public class RocklaunchStatsModule : PrismBaseModule
    {        
        IRegionManager _regionManager;

        public RocklaunchStatsModule(IUnityContainer unityContainer, IRegionManager regionManager) : base(unityContainer, regionManager)
        {
            _regionManager = regionManager;            
        }

        public override void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.StatsView));
        }
    }
}