using Hs.Hypermint.FilesViewer.Views;
using Hypermint.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.FilesViewer
{
    public class FilesViewModule : PrismBaseModule
    {
        public FilesViewModule(IUnityContainer container, IRegionManager manager) : base(container, manager) { }

        public override void Initialize()
        {            
            RegionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(RlFilesView));
        }
    }
}
