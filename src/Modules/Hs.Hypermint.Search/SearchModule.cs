using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;

namespace Hs.Hypermint.Search
{
    public class SearchModule : PrismBaseModule
    {

        IRegionManager _regionManager;

        public SearchModule(IUnityContainer container, IRegionManager manager)
            : base(container, manager)
        {
            _regionManager = manager;

        }

        public override void Initialize()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.SearchView));
 
        }
    }
}