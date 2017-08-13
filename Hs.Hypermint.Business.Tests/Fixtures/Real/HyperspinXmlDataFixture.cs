using Hs.Hypermint.Business.Hyperspin;
using Xunit;

namespace Hs.Hypermint.BusinessTests.Fixtures.Real
{
    [CollectionDefinition("HyperspinXmlDataCollection")]
    public class DatabaseCollection : ICollectionFixture<HyperspinXmlDataFixture> { }

    public class HyperspinXmlDataFixture
    {
        public IHyperspinXmlDataProvider _xmlDataProvider;

        public HyperspinXmlDataFixture()
        {
            _xmlDataProvider = new HyperspinDataProvider();
        }
    }
}
