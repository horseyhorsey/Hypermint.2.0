using Hs.Hypermint.Browser.Views;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;

namespace Hs.Hypermint.Browser
{
    public class BrowserModule : PrismBaseModule
    {
        IRegionManager _regionManager;

        public BrowserModule(IUnityContainer unityContainer, IRegionManager regionManager) : base(unityContainer, regionManager)
        {
            _regionManager = regionManager;
        }

        public override void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(WebBrowseView));
        }
    }
}