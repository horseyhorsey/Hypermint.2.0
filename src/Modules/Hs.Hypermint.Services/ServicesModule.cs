﻿using Hypermint.Base;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.Services
{
    public class ServicesModule : PrismBaseModule
    {
        public ServicesModule(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {            
            //RegionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarSystemsView));            
        }

        public override void Initialize()
        {                                    

            
            
        }

    }
}
