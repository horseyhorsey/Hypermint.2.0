using Hypermint.Base;
using Hypermint.Base.Base;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.FilesViewer
{
    public class FilesViewModule : PrismBaseModule
    {
        public FilesViewModule(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {            
            RegionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(FilesView));            
        }

        public override void Initialize()
        {
            //UnityContainer.RegisterType<IMainMenuRepo>();
        }

    }
}
