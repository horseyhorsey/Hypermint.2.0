using Hs.Hypermint.WheelCreator.Repo;
using Hs.Hypermint.WheelCreator.Services;
using Hs.Hypermint.WheelCreator.Views;
using Hypermint.Base;
using Hypermint.Base.Constants;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Hs.Hypermint.WheelCreator
{
    public class WheelModuleInit : PrismBaseModule
    {
        public WheelModuleInit(IUnityContainer container, IRegionManager manager) : base(container, manager)
        {
            //Initiate the ItextImageService with ProgramData Preset path
            SettingsRepo settings = (SettingsRepo)container.Resolve(typeof(SettingsRepo));
            var ctorParams = new InjectionConstructor(settings.HypermintSettings.PresetPath);
            container.RegisterType<ITextImageService, TextImage>(new ContainerControlledLifetimeManager(), ctorParams);

            manager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(WheelProcessView));
            manager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(SimpleWheelView));
        }
    }
}
