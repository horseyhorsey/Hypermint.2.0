﻿using Hs.Hypermint.Services;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.Audits
{
    public class AuditsModule : PrismBaseModule
    {
        IRegionManager _regionManager;

        public AuditsModule(IUnityContainer container, IRegionManager manager)
                : base(container, manager)
        {
            _regionManager = manager;
        }

        public override void Initialize()
        {
            UnityContainer.RegisterType<IAuditer, Auditer>();

            UnityContainer.RegisterType<IAuditerRl, AuditerRlRepo>();

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.HsMediaAuditView));

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.RlMediaAuditView));
        }
    }
}