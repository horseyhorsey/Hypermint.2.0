using Hs.Hypermint.Search.ViewModels;
using Hs.Hypermint.Search.Views;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

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
            RegionManager.RegisterViewWithRegion(RegionNames.SearchViewRegion, typeof(Views.SearchOptionsView));
            RegionManager.RegisterViewWithRegion(RegionNames.SearchResultsViewRegion, typeof(Views.SearchResultsView));
            
        }
    }
}