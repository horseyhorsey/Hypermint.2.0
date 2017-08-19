using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.GameLaunch
{

    public class GameLaunchModule : PrismBaseModule
    {
        public GameLaunchModule(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {
                                 
        }

        public override void Initialize()
        {
            UnityContainer.RegisterType<IGameLaunch, GameLaunch>(new ContainerControlledLifetimeManager());

        }

    }
}