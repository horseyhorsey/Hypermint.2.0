﻿using Hs.Hypermint.Services;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.MultiSystem
{
    public class MultiSystemModule : PrismBaseModule
    {

        IRegionManager _regionManager;

        public MultiSystemModule(IUnityContainer container, IRegionManager manager)
            : base(container, manager)
        {
           _regionManager = manager;

        }

        public override void Initialize()
        {
            UnityContainer.RegisterType<IMultiSystemRepo, MultiSystemRepo>();
            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.MultiSystemView));            
        }
    }
}