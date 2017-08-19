using Frontends.Models.Interfaces;
using Horsesoft.Frontends.Helper.Common;
using Hs.Hypermint.Business.Hyperspin;
using Hypermint.Base;
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
        public IHyperspinManager _hsManager;

        public HyperspinXmlDataFixture()
        {
            _xmlDataProvider = new HyperspinDataProvider();            
            _frontend = new HyperspinFrontend
            {
                Path = Path.Combine(Environment.CurrentDirectory, "TestData", "Hyperspin")
            };
        }
    }
}
