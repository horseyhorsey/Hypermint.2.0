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
            moduleCatalog.AddModule(typeof(Hs.Hypermint.FilesViewer.FilesViewModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.WheelCreator.WheelModuleInit));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.MediaPane.MediaPaneModule));

        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IApplicationCommands, ApplicationCommandsProxy>();

            Container.RegisterInstance<ISettingsRepo>(Container.Resolve<SettingsRepo>());
            Container.RegisterInstance<IFlyoutService>(Container.Resolve<FlyoutService>());
            Container.RegisterInstance<IFindDirectoryService>(Container.Resolve<FindDirectoryService>());
            

        }

    }
}
