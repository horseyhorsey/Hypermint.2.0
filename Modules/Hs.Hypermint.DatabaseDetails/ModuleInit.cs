﻿using Hs.Hypermint.DatabaseDetails.Services;
using Hs.Hypermint.DatabaseDetails.Views;
using Hs.Hypermint.Services;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.DatabaseDetails
{
    public class ModuleInit : PrismBaseModule
    {
        public ModuleInit(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {
            
        }

        public override void Initialize()
        {
            
            UnityContainer.RegisterType<IFavoriteService,FavoriteService>();

            UnityContainer.RegisterType<IFolderExplore, FolderExplore>();            

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(MainMenuView));

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(DatabaseDetailsView));

            RegionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(DatabaseOptionsView));            


        }

    }
}