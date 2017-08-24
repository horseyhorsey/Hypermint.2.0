﻿using Hs.Hypermint.Business.RocketLauncher;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Xunit;
using Horsesoft.Frontends.Helper.Common;
using System;
using System.Linq;
using Hypermint.Base.Interfaces;
using Frontends.Models.Interfaces;
using Hypermint.Base;

namespace Hs.Hypermint.RocketStats.Tests
{
    #region Setup

    [CollectionDefinition("RlStatsCollection")]
    public class FixtureCollection : ICollectionFixture<RlStatsViewModelFixture> { }

    public class Bootstrapper : UnityBootstrapper { }

    public class RlStatsViewModelFixture
    {
        public IFrontend _frontendRl;
        public IRocketLaunchStatProvider _statRepo;
        public IEventAggregator ea;
        public ISettingsHypermint settingsRepo;

        public RlStatsViewModelFixture()
        {
            _frontendRl = new HyperspinFrontend()
            {
                Executable = "",
                HasSettingsFile = true,
                Path = Environment.CurrentDirectory + "\\RocketLauncherData"
            };

            //Setup container and bootstrapper
            Bootstrapper bs = new Bootstrapper();
            IUnityContainer container = new UnityContainer();

            //Run bootstrapper to get the evtaggregator
            bs.Run();
            ea = ServiceLocator.Current.GetInstance<IEventAggregator>();

            //Register the Types needed and resolve
            container.RegisterType<IRocketLaunchStatProvider, RocketLaunchStatProvider>();
            container.RegisterType<ISettingsHypermint, SettingsRepo>();

            _statRepo = container.Resolve<IRocketLaunchStatProvider>();
            settingsRepo = container.Resolve<ISettingsHypermint>();
            settingsRepo.HypermintSettings.RlPath = _frontendRl.Path;
        }
    }

    #endregion

    [Collection("RlStatsCollection")]
    public class StatsViewModelTests
    {
        private RlStatsViewModelFixture _fixture;

        public StatsViewModelTests()
        {
            _fixture = new RlStatsViewModelFixture();
        }

        [Fact(Skip = "Needs attention")]        
        public async void InitStatsViewModel__AmstradGameStatsPopulatedGreaterThan100()
        {
            _fixture._statRepo.SetUp(_fixture._frontendRl.Path);            

            //var vm = new RocklaunchStats.ViewModels.StatsViewModel(_fixture._statRepo, _fixture.ea, _fixture.settingsRepo);

            //await vm.UpdateStatsOnSystemChanged("Amstrad CPC");

            Assert.True(_fixture._statRepo.SystemGameStats.Count() > 100);
        }


        [Fact(Skip = "Needs attention")]
        public async void InitStatsViewModel__MainMenuStats_TopTenGreaterThan9()
        {
            _fixture._statRepo.SetUp(_fixture._frontendRl.Path);

            //var vm = new RocklaunchStats.ViewModels.StatsViewModel(_fixture._statRepo, _fixture.ea, _fixture.settingsRepo);

            //await vm.UpdateStatsOnSystemChanged("Main Menu");

            Assert.True(_fixture._statRepo.GlobalStats.TopTenTimesPlayed.Count() > 9);
        }
    }
}
