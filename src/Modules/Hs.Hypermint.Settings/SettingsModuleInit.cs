using Hs.Hypermint.Services;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.Settings
{
    public class SettingsModuleInit : PrismBaseModule
    {
        public SettingsModuleInit(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {

        }

        public override void Initialize()
        {
            UnityContainer.RegisterType<ISettingsRepo, SettingsRepo>();
            RegionManager.RegisterViewWithRegion(RegionNames.SettingsRegion, typeof(SettingsFlyoutView));
        }

    }
}
