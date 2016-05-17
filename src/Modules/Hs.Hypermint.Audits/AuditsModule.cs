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

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.HsMediaAuditView));

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.RlMediaAuditView));

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(BezelEdit.Views.BezelEditView));
        }
    }
}