using Hs.Hypermint.VideoEdit.Views;
using Hypermint.Base.Constants;
using Prism.Modularity;
using Prism.Regions;

namespace Hs.Hypermint.VideoEdit
{
    public class VideoEditModule : IModule
    {
        IRegionManager _regionManager;

        public VideoEditModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(VideoEditView));
            _regionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(VideoProcessView));
        }

        public void Initialize()
        {            
        }
    }
}