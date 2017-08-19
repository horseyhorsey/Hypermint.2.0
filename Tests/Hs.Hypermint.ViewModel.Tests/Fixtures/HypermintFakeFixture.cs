using FakeItEasy;
using Hs.Hypermint.Business.Hyperspin;
using Hs.Hypermint.Services;
using Hypermint.Base;
using Hypermint.Base.Interfaces;

namespace Hs.Hypermint.ViewModel.Tests.Fixtures
{
    /// <summary>
    /// Fixture needs no attributes
    /// </summary>
    public class HypermintFakeFixture
    {
        public ISettingsHypermint _settingsRepo;
        public IMainMenuRepo _mainMenuRepo;
        public IHyperspinManager _hyperManager;

        public HypermintFakeFixture()
        {
            _settingsRepo = A.Fake<SettingsRepo>();
            _mainMenuRepo = A.Fake<MainMenuRepo>();
            _hyperManager = A.Fake<HyperspinManager>();            
        }
    }
}
