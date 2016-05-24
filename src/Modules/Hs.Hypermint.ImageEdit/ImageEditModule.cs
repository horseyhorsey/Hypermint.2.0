using Hs.Hypermint.ImageEdit.Views;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;
using System;

namespace Hs.Hypermint.ImageEdit
{
    public class ImageEditModule : PrismBaseModule
    {
        IRegionManager _regionManager;

        public ImageEditModule(IUnityContainer unityContainer, IRegionManager regionManager)
            : base(unityContainer, regionManager)
        {
            _regionManager = regionManager;

            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(CreateImageView));

            _regionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(ImagePresetView));
            
        }

        public override void Initialize()
        {
            
        }
    }
}