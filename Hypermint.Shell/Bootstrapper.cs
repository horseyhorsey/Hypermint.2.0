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
using Hs.Hypermint.GameLaunch;
using MahApps.Metro.Controls.Dialogs;
using Hs.Hypermint.Search.Views;
using Hs.Hypermint.SidebarSystems;
using Hs.Hypermint.DatabaseDetails;
using Hs.Hypermint.RocklaunchStats.Views;
using Hs.Hypermint.DatabaseDetails.Services;
using Hs.Hypermint.MediaPane.Views;
using Hs.Hypermint.Business.RocketLauncher;
using Hs.Hypermint.Business.Hyperspin;

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
                //regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(SearchView));
                regionManager.RegisterViewWithRegion(RegionNames.FilesRegion, typeof(Hs.Hypermint.DatabaseDetails.Views.MainMenuView));
            }

            Application.Current.MainWindow = (Views.Shell)this.Shell;            
            Application.Current.MainWindow.Show();
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
            moduleCatalog.AddModule(typeof(GameLaunchModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.HyperspinFile.HyperspinFileModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.Search.SearchModule));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.WheelCreator.WheelModuleInit));
            moduleCatalog.AddModule(typeof(Hs.Hypermint.RocklaunchStats.RocklaunchStatsModule));            
            moduleCatalog.AddModule(typeof(Hs.Hypermint.ImageEdit.ImageEditModule));            
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IApplicationCommands, ApplicationCommandsProxy>();
            Container.RegisterType<IDialogCoordinator, DialogCoordinator>();

#warning reenable this with new
            Container.RegisterInstance<IAuditer>(Container.Resolve<Auditer>());

            Container.RegisterInstance<ISettingsHypermint>(Container.Resolve<SettingsRepo>());
            Container.RegisterInstance<IFileFolderChecker>(Container.Resolve<FileFolderChecker>());
            Container.RegisterInstance<IFlyoutService>(Container.Resolve<FlyoutService>());
            Container.RegisterInstance<IFileDialogHelper>(Container.Resolve<Base.Services.FileFolderService>());
            Container.RegisterInstance<ISelectedService>(Container.Resolve<SelectedService>());
            Container.RegisterInstance<IGameLaunch>(Container.Resolve<GameLaunch>());

            Container.RegisterType<IMainMenuRepo, MainMenuRepo>(new ContainerControlledLifetimeManager());
            //Container.RegisterType<IHyperspinXmlDataProvider, MainMenuRepo>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IHyperspinXmlService, HyperspinXmlService>(
                new ContainerControlledLifetimeManager());

#warning this needs replacing with the helper lib
            //Container.RegisterType<IAuditerRl, Auditer>(
              //  new ContainerControlledLifetimeManager());

            Container.RegisterType<IStatsRepo, StatRepo>(
                new ContainerControlledLifetimeManager());

            Container.RegisterType<IRocketLaunchStatProvider, RocketLaunchStatProvider>(
                new ContainerControlledLifetimeManager());

            Container.RegisterType<ISearchYoutube, SearchYoutubeService>(
                new ContainerControlledLifetimeManager());

            Container.RegisterType<IGameRepo, GameRepo>(
                new ContainerControlledLifetimeManager());

            Container.RegisterType<IPdfService, PdfService>(
                new ContainerControlledLifetimeManager());

            Container.RegisterType<IImageEditService, ImageEditRepo>(
                new ContainerControlledLifetimeManager());

            Container.RegisterType<ITrashMaster, TrashMaster>(
                new ContainerControlledLifetimeManager());

            //New providers
            Container.RegisterType<IHyperspinXmlDataProvider, HyperspinDataProvider>(
                new ContainerControlledLifetimeManager());

            Container.RegisterType<IHyperspinManager, HyperspinManager>(
                new ContainerControlledLifetimeManager());

            //Move these into the business layer
            //Container.RegisterType<ISystemCreator, SystemCreator>(
            //    new ContainerControlledLifetimeManager());
            //Container.RegisterType<IFrontend, HyperspinFrontend>(
            //    new ContainerControlledLifetimeManager());

            RegisterNavigationTypes();

        }

        private void RegisterNavigationTypes()
        {
            Container.RegisterTypeForNavigation<DatabaseDetailsView>("DatabaseDetailsView");
            Container.RegisterTypeForNavigation<DatabaseOptionsView>("DatabaseOptionsView");
            Container.RegisterTypeForNavigation<MultiSystemView>("MultiSystemView");
            Container.RegisterTypeForNavigation<IntroVideosView>("IntroVideosView");
            Container.RegisterTypeForNavigation<HsMediaAuditView>("HsMediaAuditView");
            Container.RegisterTypeForNavigation<RlMediaAuditView>("RlMediaAuditView");
            Container.RegisterTypeForNavigation<SimpleWheelView>("SimpleWheelView");
            Container.RegisterTypeForNavigation<SearchView>("SearchView");
            Container.RegisterTypeForNavigation<StatsView>("StatsView");
            Container.RegisterTypeForNavigation<BezelEditView>("BezelEditView");


            //Container.RegisterTypeForNavigation<Hs.Hypermint.SidebarSystems.Views.SystemsView>("SystemView");
            //Container.RegisterTypeForNavigation(RegionNames.SystemsRegion, typeof(SystemsView));
            //Container.RegisterTypeForNavigation<Hs.Hypermint.FilesViewer.FilesView>("FilesView");
            //Container.RegisterTypeForNavigation<DatabaseOptionsView>("DatabaseOptionsView");
        }
    }
}
