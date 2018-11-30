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
using MahApps.Metro.Controls.Dialogs;
using Hs.Hypermint.Search.Views;
using Hs.Hypermint.SidebarSystems;
using Hs.Hypermint.DatabaseDetails;
using Hs.Hypermint.RocklaunchStats.Views;
using Hs.Hypermint.MediaPane.Views;
using Hs.Hypermint.Business.RocketLauncher;
using Hs.Hypermint.Business.Hyperspin;
using Hs.Hypermint.VideoEdit.Views;
using System;
using System.IO;
using Prism.Logging;

namespace Hypermint.Shell
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell() => Container.Resolve<Views.Shell>();

        protected override void InitializeShell()
        {
            base.InitializeShell();

            // Register view
            var regionManager = this.Container.Resolve<IRegionManager>();
            if (regionManager != null)
            {
                regionManager.RegisterViewWithRegion(RegionNames.FlyoutRegion, typeof(SettingsFlyout));
                regionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(MainMenuView));
            }

            Application.Current.MainWindow = (Views.Shell)this.Shell;            
            Application.Current.MainWindow.Show();
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new HypermintLogger();
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;

            moduleCatalog.AddModule(typeof(ServicesModule));
            moduleCatalog.AddModule(typeof(SidebarSystemsModule));
            moduleCatalog.AddModule(typeof(ModuleInit));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.NavBar.NavBarModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.MultiSystem.MultiSystemModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.FilesViewer.FilesViewModule));            
            moduleCatalog.AddModule(typeof(Hs.Hypermint.MediaPane.MediaPaneModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.IntroVideos.IntroVideosModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.Audits.AuditsModule));            
            moduleCatalog.AddModule(typeof(Hs.Hypermint.HyperspinFile.HyperspinFileModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.Search.SearchModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.WheelCreator.WheelModuleInit));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.RocklaunchStats.RocklaunchStatsModule));            
            moduleCatalog.AddModule(typeof(Hs.Hypermint.ImageEdit.ImageEditModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.VideoEdit.VideoEditModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IApplicationCommands, ApplicationCommandsProxy>();
            Container.RegisterType<IDialogCoordinator, DialogCoordinator>();

            Container.RegisterInstance<ISettingsHypermint>(Container.Resolve<SettingsRepo>());
            Container.RegisterInstance<IFlyoutService>(Container.Resolve<FlyoutService>());
            Container.RegisterInstance<IFileDialogHelper>(Container.Resolve<FileDialogHelper>());
            Container.RegisterInstance<ISelectedService>(Container.Resolve<SelectedService>());
            Container.RegisterInstance<IGameLaunch>(Container.Resolve<GameLaunch>());
            Container.RegisterInstance<IRlScan>(Container.Resolve<RlScan>());           
            
            Container.RegisterType<IImageEditService, ImageEditor>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPdfService, PdfService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISearchYoutube, SearchYoutubeService>(new ContainerControlledLifetimeManager());

            var trashParam = new InjectionConstructor(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Hypermint", "Trash"));
            Container.RegisterType<ITrashMaster, TrashMaster>(new ContainerControlledLifetimeManager(), trashParam);

            //New providers
            Container.RegisterType<IHyperspinManager, HyperspinManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IHyperspinXmlDataProvider, HyperspinDataProvider>( new ContainerControlledLifetimeManager());            
            Container.RegisterType<IRocketLaunchStatProvider, RocketLaunchStatProvider>( new ContainerControlledLifetimeManager());

            RegisterNavigationTypes();

        }

        private void RegisterNavigationTypes()
        {
            Container.RegisterTypeForNavigation<DatabaseDetailsView>("DatabaseDetailsView");
            Container.RegisterTypeForNavigation<DatabaseOptionsView>("DatabaseOptionsView");
            Container.RegisterTypeForNavigation<MultiSystemView>("MultiSystemView");
            Container.RegisterTypeForNavigation<IntroVideosView>("IntroVideosView");
            Container.RegisterTypeForNavigation<ExportVideoOptionsView>("ExportVideoOptionsView");
            Container.RegisterTypeForNavigation<HsMediaAuditView>("HsMediaAuditView");
            Container.RegisterTypeForNavigation<RlMediaAuditView>("RlMediaAuditView");
            Container.RegisterTypeForNavigation<SimpleWheelView>("SimpleWheelView");
            Container.RegisterTypeForNavigation<SearchView>("SearchView");
            Container.RegisterTypeForNavigation<StatsView>("StatsView");
            Container.RegisterTypeForNavigation<BezelEditView>("BezelEditView");
            Container.RegisterTypeForNavigation<VideoEditView>("VideoEditView");


            //Container.RegisterTypeForNavigation<Hs.Hypermint.SidebarSystems.Views.SystemsView>("SystemView");
            //Container.RegisterTypeForNavigation(RegionNames.SystemsRegion, typeof(SystemsView));
            //Container.RegisterTypeForNavigation<Hs.Hypermint.FilesViewer.FilesView>("FilesView");
            //Container.RegisterTypeForNavigation<DatabaseOptionsView>("DatabaseOptionsView");
        }
    }
}
