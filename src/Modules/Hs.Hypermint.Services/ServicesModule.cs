using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.Services
{
    public class ServicesModule : PrismBaseModule
    {
        public ServicesModule(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {            
            //RegionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarSystemsView));            
        }

        public override void Initialize()
        {                        

            UnityContainer.RegisterType<ISettingsRepo, SettingsRepo>(new ContainerControlledLifetimeManager());

            UnityContainer.RegisterType<IGameRepo, GameRepo>(new ContainerControlledLifetimeManager());
            
        }

    }
}
