using Hs.Hypermint.Services;
using Hs.Hypermint.SidebarSystems.Views;
using Hypermint.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.SidebarSystems
{
    public class SidebarSystemsModule : PrismBaseModule
    {
        private IRegionManager _regionManager;

        public SidebarSystemsModule(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {
            _regionManager = manager;
            UnityContainer.RegisterType<IFolderExplore, FolderExplore>();
            RegionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarView));
            //RegionManager.RegisterViewWithRegion(RegionNames.SidebarMainMenusRegion, typeof(MainMenuView));
            //RegionManager.RegisterViewWithRegion(RegionNames.SidebarSystemsRegion, typeof(SystemsView));
        }

        public override void Initialize()
        {

        }

    }
}
