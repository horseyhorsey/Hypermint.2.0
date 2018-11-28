using Hs.Hypermint.DatabaseDetails.Views;
using Hs.Hypermint.Services;
using Hypermint.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.DatabaseDetails
{
    public class ModuleInit : PrismBaseModule
    {
        public ModuleInit(IUnityContainer container, IRegionManager manager) : base(container, manager) { }

        public override void Initialize()
        {            
            UnityContainer.RegisterType<IFolderExplore, FolderExplore>();            

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(MainMenuView));
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(DatabaseDetailsView));
            RegionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(DatabaseOptionsView));            
        }
    }
}
