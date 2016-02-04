using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Hypermint.Base.Base
{
    public abstract class PrismBaseModule : IModule
    {
        #region Properties
        /// <summary>
        /// The Unity container
        /// </summary>
        public IUnityContainer UnityContainer { get; private set; }
        /// <summary>
        /// The region manager
        /// </summary>
        public IRegionManager RegionManager { get; private set; }
        #endregion Properties

        public virtual void Initialize()
        {

        }

        protected PrismBaseModule(IUnityContainer unityContainer, IRegionManager regionManager)
        {
            UnityContainer = unityContainer;
            RegionManager = regionManager;
        }

    }
}
