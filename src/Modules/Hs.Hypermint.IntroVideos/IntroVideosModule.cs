using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Services;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.IntroVideos
{
    public class IntroVideosModule : PrismBaseModule
    {

        IRegionManager _regionManager;

        public IntroVideosModule(IUnityContainer container, IRegionManager manager)
            : base(container, manager)
        {
            _regionManager = manager;            
        }

        public override void Initialize()
        {
            UnityContainer.RegisterType<IAviSynthScripter, AviSynthScripter>();

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.IntroVideosView));
            
        }
    }
}