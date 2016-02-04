using Microsoft.Practices.Unity;
using Prism.Unity;
using System.Windows;
using Prism.Modularity;

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

            Application.Current.MainWindow = (Views.Shell)this.Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            
            moduleCatalog.AddModule(typeof(Hs.Hypermint.Services.ServicesModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.DatabaseDetails.ModuleInit));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.SidebarSystems.SidebarSystemsModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.FilesViewer.FilesViewModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.WheelCreator.WheelModuleInit));

        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // Old implementation            
            //Container.RegisterType(typeof(object), typeof(Hs.Hypermint.DatabaseDetails.DatabaseDetailsView),"DatabaseDetailsView");
            //Container.RegisterTypeForNavigation<Hs.Hypermint.SidebarSystems.SidebarSystemsView>("SidebarSystemsView");
        }

    }
}
