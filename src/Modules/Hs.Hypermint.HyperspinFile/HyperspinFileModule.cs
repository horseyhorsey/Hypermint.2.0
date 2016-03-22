using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;

namespace Hs.Hypermint.HyperspinFile
{
    public class HyperspinFileModule : PrismBaseModule
    {
        IRegionManager _regionManager;

        public HyperspinFileModule(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {
            _regionManager = manager;
            RegionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(Views.HyperspinFilesView));
        }

        public override void Initialize()
        {

            //UnityContainer.RegisterType<IMainMenuRepo, MainMenuRepo>(new ContainerControlledLifetimeManager());

            //UnityContainer.RegisterType<ISettingsRepo, SettingsRepo>(new ContainerControlledLifetimeManager());

            //UnityContainer.RegisterType<IGameRepo, GameRepo>(new ContainerControlledLifetimeManager());

        }

    }
}