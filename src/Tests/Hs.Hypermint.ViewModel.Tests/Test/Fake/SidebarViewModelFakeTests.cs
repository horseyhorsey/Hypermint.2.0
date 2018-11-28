using FakeItEasy;
using Frontends.Models.Hyperspin;
using Hs.Hypermint.SidebarSystems.ViewModels;
using Hs.Hypermint.ViewModel.Tests.Fixtures;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Hs.Hypermint.ViewModel.Tests.Test.Fake
{
    [Collection("HypermintFakeCollection")]
    public class SidebarViewModelFakeTests
    {
        public HypermintFakeFixture _fixture;
        private SidebarViewModel _sidebarVm;

        public SidebarViewModelFakeTests()
        {
            _sidebarVm = A.Fake<SidebarViewModel>();
            //await A.CallTo(() => _sidebarVm.Load()).DoesNothing();
            //_sidebarVm. = new ObservableCollection<MainMenu>(A.CollectionOfFake<MainMenu>(4))

            //_sidebarVm.Load();

            //_fixture = new HypermintFakeFixture();
            //_fixture._hyperManager.Systems = new ObservableCollection<MainMenu>(A.CollectionOfFake<MainMenu>(4));
            
            //_fixture._settingsRepo.HypermintSettings.HsPath = "C:\\Hyperspin";                  
            //_fixture._hyperManager.Systems.Add(new MainMenu());
        }

        [Fact]
        public void InitSideBarViewModel()
        {
            Assert.True(_sidebarVm != null);
        }

        [Fact(Skip = "Needs sorting")]
        public void SettingsRepoIsNotNull()
        {
            Assert.True(_fixture._settingsRepo != null);

            _sidebarVm.Load();
        }
    }
}
