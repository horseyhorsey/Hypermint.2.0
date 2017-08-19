using Hs.Hypermint.ViewModel.Tests.Fixtures;
using Xunit;

namespace Hs.Hypermint.ViewModel.Tests.Collections
{
    [CollectionDefinition("HypermintFakeCollection")]
    public class HypermintFakeCollection : ICollectionFixture<HypermintFakeFixture> { }

    [CollectionDefinition("RealCollection")]
    public class RealCollection : ICollectionFixture<RealFixture> { }

    [CollectionDefinition("SideBarViewModelsCollection")]
    public class SideBarViewModelsCollection : ICollectionFixture<HypermintFakeFixture> { }

}
