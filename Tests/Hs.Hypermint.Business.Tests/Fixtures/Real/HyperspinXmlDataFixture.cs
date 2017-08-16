using Horsesoft.Frontends.Helper.Common;
using Horsesoft.Frontends.Helper.Hyperspin;
using Hs.Hypermint.Business.Hyperspin;
using System;
using System.IO;
using Xunit;

namespace Hs.Hypermint.BusinessTests.Fixtures.Real
{
    [CollectionDefinition("HyperspinXmlDataCollection")]
    public class DatabaseCollection : ICollectionFixture<HyperspinXmlDataFixture> { }

    public class HyperspinXmlDataFixture
    {
        public IHyperspinXmlDataProvider _xmlDataProvider;
        public IFrontend _frontend;

        public HyperspinXmlDataFixture()
        {
            _xmlDataProvider = new HyperspinDataProvider();

            _frontend = new HyperspinFrontend();
            _frontend.Path = Path.Combine(Environment.CurrentDirectory, "TestData", "Hyperspin");
        }
    }
}
