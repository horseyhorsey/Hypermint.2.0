using Hypermint.Base.Base;
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
        }

        public override void Initialize()
        {
            
        }
    }
}