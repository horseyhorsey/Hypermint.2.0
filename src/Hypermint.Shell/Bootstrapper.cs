using Microsoft.Practices.Unity;
using Prism.Unity;
using System.Windows;
using Prism.Modularity;
using Hypermint.Base.Interfaces;
using Hypermint.Base;
using Prism.Regions;
using Hypermint.Base.Services;
using Hypermint.Base.Constants;
using Hs.Hypermint.Services;
using Hypermint.Shell.Views;
using Hs.Hypermint.MultiSystem.Views;
using Hs.Hypermint.IntroVideos.Views;
using Hs.Hypermint.Audits.Views;
using Hs.Hypermint.DatabaseDetails.Views;
using Hs.Hypermint.WheelCreator.Views;

namespace Hypermint.Shell
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Views.Shell>();                        
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            
            // Register view
            var regionManager = this.Container.Resolve<IRegionManager>();
            if (regionManager != null)
            {
                regionManager.RegisterViewWithRegion(RegionNames.FlyoutRegion, typeof(SettingsFlyout));
            }
            
            Application.Current.MainWindow = (Views.Shell)this.Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            
            moduleCatalog.AddModule(typeof(Hs.Hypermint.Services.ServicesModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.SidebarSystems.SidebarSystemsModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.DatabaseDetails.ModuleInit));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.NavBar.NavBarModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.MultiSystem.MultiSystemModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.FilesViewer.FilesViewModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.WheelCreator.WheelModuleInit));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.MediaPane.MediaPaneModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.IntroVideos.IntroVideosModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.Audits.AuditsModule));

        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IApplicationCommands, ApplicationCommandsProxy>();

            Container.RegisterInstance<ISettingsRepo>(Container.Resolve<SettingsRepo>());
            Container.RegisterInstance<IFlyoutService>(Container.Resolve<FlyoutService>());
            Container.RegisterInstance<IFindDirectoryService>(Container.Resolve<FindDirectoryService>());
            Container.RegisterInstance<ISelectedService>(Container.Resolve<SelectedService>());
            Container.RegisterInstance<IGenreRepo>(Container.Resolve<GenreRepo>());

            Container.RegisterTypeForNavigation<DatabaseDetailsView>("DatabaseDetailsView");
            Container.RegisterTypeForNavigation<MultiSystemView>("MultiSystemView");
            Container.RegisterTypeForNavigation<IntroVideosView>("IntroVideosView");
            Container.RegisterTypeForNavigation<HsMediaAuditView>("HsMediaAuditView");
            Container.RegisterTypeForNavigation<SimpleWheelView>("SimpleWheelView");

        }

    }
}
