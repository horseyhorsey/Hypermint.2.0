using Hypermint.Base.Base;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;

namespace Hs.Hypermint.BezelEdit
{
    public class BezelEditModule : PrismBaseModule
    {
        IRegionManager _regionManager;


        public BezelEditModule(IUnityContainer unityContainer, IRegionManager regionManager) : base(unityContainer, regionManager)
        {

        }

        public override void Initialize()
        {
            
        }
    }
}